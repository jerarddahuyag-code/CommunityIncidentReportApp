using Account.Domain;
using Common.Database;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Account.Services;

public interface IAuthService
{
    string GenerateRefreshToken();

    string GetToken(List<Claim> claims);

    Task RevokeRefreshToken(string refreshToken);

    Task SaveRefreshToken(RefreshTokenDomain refreshToken);

    Task<RefreshTokenDomain?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken);
    List<Claim> GenerateClaimsFromUser(UserDomain user);
}
public class AuthService(IDbConnectionFactory connectionFactory, IOptions<JwtOption> jwtOptions) : IAuthService
{
    private readonly JwtOption jwtOption = jwtOptions.Value;

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GetToken(List<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SecretKey));
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = jwtOption.ValidIssuer,
            Audience = jwtOption.ValidAudience,
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public List<Claim> GenerateClaimsFromUser(UserDomain user) => [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.DisplayName),
            new(ClaimTypes.Role, user.Role.ToString())
        ];

    public async Task SaveRefreshToken(RefreshTokenDomain refreshToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync();

        await conn.ExecuteAsync(
            """
                insert into refreshTokens (Token, UserId, ExpiresOn, CreatedOn, RevokedOn)
                values (@Token, @UserId, @ExpiresOn, @CreatedOn, @RevokedOn)
            """, refreshToken);
    }

    public async Task<RefreshTokenDomain?> GetRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var validToken = await conn.QuerySingleOrDefaultAsync<RefreshTokenDomain>(
            """
                select * from refreshTokens
                where Token = @Token
                and RevokedOn is null
                and ExpiresOn > CURRENT_TIMESTAMP
            """, new { Token = refreshToken });

        return validToken;
    }

    public async Task RevokeRefreshToken(string refreshToken)
    {
        using var conn = await connectionFactory.CreateConnectionAsync();

        await conn.ExecuteAsync(
            """
                update refreshTokens
                set RevokedOn = CURRENT_TIMESTAMP
                where Token = @Token
            """, new { Token = refreshToken });
    }
}
