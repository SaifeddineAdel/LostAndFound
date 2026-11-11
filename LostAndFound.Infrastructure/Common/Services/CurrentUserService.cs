using LostAndFound.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LostAndFound.Infrastructure.Common.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userId =
                user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue("sub");

            if (!int.TryParse(userId, out var id))
            {
                throw new InvalidOperationException(
                    "User is not authenticated.");
            }

            return id;
        }
    }
}