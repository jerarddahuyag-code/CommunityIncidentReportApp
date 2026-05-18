using Moq;

namespace Unit_Tests;
public static class DefaultMocks
{
    public static Mock<T> Create<T>() where T : class => new();
}
