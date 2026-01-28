using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(AppUser user);
    (string rawToken, string tokenHash, DateTime expiresAtUtc) CreateRefreshToken();
    string HashToken(string rawToken);
}
