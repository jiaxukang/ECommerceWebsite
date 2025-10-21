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

public class PaymentsController(
    IPaymentService _paymentService,
    IUnitOfWork unitOfWork,
    ILogger<PaymentsController> logger,
    IConfiguration config,
    IHubContext<NotificationHub> hubContext
) : BaseController
{
    private readonly string _whSecret = config["StripeSettings:WhSecret"]!;

    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string basketId)
    {
        var result = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
        if (result == null) return BadRequest("Problem with payment intent");
        return Ok(result);
    }

    [HttpGet("deliveryMethods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        var methods = await unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        return Ok(methods);
    }

   [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);

            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                return BadRequest("Invalid event data.");
            }

            await HandlePaymentIntentSucceeded(intent);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
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
            var spec = new OrderSpecification(intent.Id, true);

            var order = await unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpecAsync(spec)
                        ?? throw new Exception("Order not found");

            var orderTotalInCents = (long)Math.Round(order.GetTotal() * 100,
            MidpointRounding.AwayFromZero);

            if (orderTotalInCents != intent.Amount)
            {
                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
            }

            await unitOfWork.Complete();

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification",
                    order.ToDto());
            }
        }
    }
}
