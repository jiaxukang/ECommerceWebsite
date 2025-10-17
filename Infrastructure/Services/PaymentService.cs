using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(
    IConfiguration config,
    ICartService cartService,
    IUnitOfWork unitOfWork
) : IPaymentService
{

    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string basketId)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        var basket = await cartService.GetCartAsync(basketId);

        if (basket == null) return null;

        var shippingPrice = 0m;
        if (basket.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
            if (deliveryMethod == null) return null;
            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in basket.Items)
        {
            var product = await unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);
            if (product == null) return null;
            if (item.Price != product.Price)
                item.Price = product.Price;
        }

        var service = new PaymentIntentService();
        PaymentIntent? intent = null;
        if (string.IsNullOrEmpty(basket.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                Currency = "aud",
                PaymentMethodTypes = new List<string> { "card" }
            };
            intent = await service.CreateAsync(options);
            basket.PaymentIntentId = intent.Id;
            basket.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100)
            };
            await service.UpdateAsync(basket.PaymentIntentId, options);
        }

        await cartService.SetCartAsync(basket);
        return basket;
    }
}
