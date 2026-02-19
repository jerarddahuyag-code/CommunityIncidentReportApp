using Account.Entities;

namespace Account.Domain;

public class UserDomain
{
    public required Guid Id { get; init; }

    public required string DisplayName { get; init; }

    public required string Username { get; init; }

    public required string PasswordHash { get; init; }

    public required Role Role { get; init; }

    public required DateTime CreatedAt { get; init; }
}
