using System;
using Core.Entities;

namespace Core.Interfaces;

public interface IPaymentService
{
    Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string basketId);
    Task<string> RefundPayment(string paymentIntentId);
}
