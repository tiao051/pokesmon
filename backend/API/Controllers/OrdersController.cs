using System.Security.Claims;
using backend.Application.Services;
using backend.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    public OrdersController(OrderService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult> GetOrders(
        [FromQuery(Name = "isPreorder")] bool? isPreorder,
        [FromQuery] string? view,
        CancellationToken ct)
    {
        var email = GetEmail();
        if (email is null) return Unauthorized(new { error = "Invalid token" });

        var listView = view?.ToLowerInvariant() switch
        {
            "active" => OrderListView.Active,
            "completed" => OrderListView.Completed,
            _ => OrderListView.All,
        };

        var orders = await _service.ListAsync(new OrderListQuery(email, isPreorder, listView), ct);
        return Ok(new { items = orders });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetOrder(string id, CancellationToken ct)
    {
        var email = GetEmail();
        if (email is null) return Unauthorized(new { error = "Invalid token" });

        var result = await _service.GetByIdAsync(id, email, ct);
        return result.Kind == OrderResultKind.Ok
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderDto dto, CancellationToken ct)
    {
        var email = GetEmail();
        if (email is null) return Unauthorized(new { error = "Invalid token" });

        var result = await _service.CreateAsync(email, dto, ct);
        return result.Kind == OrderResultKind.Ok
            ? CreatedAtAction(nameof(GetOrder), new { id = result.Value!.Id }, result.Value)
            : BadRequest(new { error = result.Error });
    }

    // TODO: gắn [Authorize(Roles = "Admin")] khi role-based auth được wire vào.
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateStatus(string id, UpdateOrderStatusDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateStatusAsync(id, dto.Status, ct);
        return result.Kind switch
        {
            OrderResultKind.Ok => Ok(result.Value),
            OrderResultKind.NotFound => NotFound(new { error = result.Error }),
            OrderResultKind.BadRequest => BadRequest(new { error = result.Error }),
            _ => StatusCode(500, new { error = "Unexpected error" })
        };
    }

    private string? GetEmail() =>
        User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
}
