using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentsController(
    IPaymentService _paymentService,
    IGenericRepository<DeliveryMethod> _deliveryMethodRepo
) : BaseController
{
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
        var methods = await _deliveryMethodRepo.ListAllAsync();
        return Ok(methods);
    }
}
