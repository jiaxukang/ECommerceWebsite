using System;
using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentsController(IPaymentService paymentService,
    IUnitOfWork unit,
    IHubContext<NotificationHub> hubContext,
    ILogger<PaymentsController> logger,
    IConfiguration config) : BaseController
{
    private readonly string _whSecret = config["StripeSettings:WhSecret"]!;

    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

        if (cart == null) return BadRequest("Problem with your cart on the API");

        return Ok(cart);
    }

    [HttpGet("deliveryMethods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await unit.Repository<DeliveryMethod>().ListAllAsync());
    }

    [HttpGet("webhook-config")]
    public IActionResult GetWebhookConfig()
    {
        var hasSecret = !string.IsNullOrEmpty(_whSecret);
        return Ok(new
        {
            webhookSecretConfigured = hasSecret,
            webhookSecretPrefix = hasSecret ? _whSecret.Substring(0, Math.Min(10, _whSecret.Length)) : "NOT SET",
            webhookUrl = $"{Request.Scheme}://{Request.Host}/api/payments/webhook"
        });
    }

    [HttpPost("webhook")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        logger.LogInformation("Received Stripe webhook request");

        try
        {
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
            logger.LogInformation("Stripe-Signature header: {SignatureExists}", !string.IsNullOrEmpty(stripeSignature) ? "Present" : "Missing");

            var stripeEvent = ConstructStripeEvent(json);
            logger.LogInformation("Stripe event type: {EventType}", stripeEvent.Type);

            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                logger.LogWarning("Invalid event data - not a PaymentIntent");
                return BadRequest("Invalid event data.");
            }

            logger.LogInformation("Processing PaymentIntent: {PaymentIntentId}, Status: {Status}", intent.Id, intent.Status);
            await HandlePaymentIntentSucceeded(intent);

            logger.LogInformation("Webhook processed successfully");
            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook error: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret, throwOnApiVersionMismatch: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to construct Stripe event");
            throw new StripeException("Invalid signature");
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            logger.LogInformation("Payment succeeded for PaymentIntent: {PaymentIntentId}", intent.Id);

            var spec = new OrderSpecification(intent.Id, true);

            var order = await unit.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpecAsync(spec);

            if (order == null)
            {
                logger.LogError("Order not found for PaymentIntent: {PaymentIntentId}", intent.Id);
                throw new Exception("Order not found");
            }

            logger.LogInformation("Found order: {OrderId} for PaymentIntent: {PaymentIntentId}", order.Id, intent.Id);

            var orderTotalInCents = (long)Math.Round(order.GetTotal() * 100,
            MidpointRounding.AwayFromZero);

            if (orderTotalInCents != intent.Amount)
            {
                logger.LogWarning("Payment amount mismatch for order {OrderId}. Expected: {Expected}, Received: {Received}",
                    order.Id, orderTotalInCents, intent.Amount);
                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                logger.LogInformation("Payment amount verified for order {OrderId}. Updating status to PaymentReceived", order.Id);
                order.Status = OrderStatus.PaymentReceived;
            }

            await unit.Complete();
            logger.LogInformation("Order {OrderId} status updated to {Status}", order.Id, order.Status);

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectionId))
            {
                logger.LogInformation("Sending notification to client for order {OrderId}", order.Id);
                await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification",
                    order.ToDto());
            }
            else
            {
                logger.LogInformation("No active connection found for buyer email: {Email}", order.BuyerEmail);
            }
        }
        else
        {
            logger.LogInformation("PaymentIntent {PaymentIntentId} status is {Status}, not processing", intent.Id, intent.Status);
        }
    }
}