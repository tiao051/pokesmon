using System.Security.Claims;
using backend.Application.Services;
using backend.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly FavoritesService _service;

    public FavoritesController(FavoritesService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult> GetFavorites([FromQuery] string? view, CancellationToken ct)
    {
        var email = GetEmail();
        if (email is null) return Unauthorized(new { error = "Invalid token" });

        if (view == "ids")
        {
            var ids = await _service.ListIdsAsync(email, ct);
            return Ok(new { ids });
        }

        var items = await _service.ListAsync(email, ct);
        return Ok(new { items });
    }

    [HttpPost]
    public async Task<ActionResult> AddFavorite(AddFavoriteDto dto, CancellationToken ct)
    {
        var email = GetEmail();
        if (email is null) return Unauthorized(new { error = "Invalid token" });

        var result = await _service.AddAsync(email, dto.ProductId, ct);
        return result.Kind switch
        {
            FavoriteResultKind.Ok => NoContent(),
            FavoriteResultKind.BadRequest => BadRequest(new { error = result.Error }),
            _ => StatusCode(500, new { error = "Unexpected error" })
        };
    }

    [HttpDelete("{productId:int}")]
    public async Task<ActionResult> RemoveFavorite(int productId, CancellationToken ct)
    {
        var email = GetEmail();
        if (email is null) return Unauthorized(new { error = "Invalid token" });

        var result = await _service.RemoveAsync(email, productId, ct);
        return result.Kind switch
        {
            FavoriteResultKind.Ok => NoContent(),
            FavoriteResultKind.NotFound => NotFound(new { error = result.Error }),
            _ => StatusCode(500, new { error = "Unexpected error" })
        };
    }

    private string? GetEmail() =>
        User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
}
