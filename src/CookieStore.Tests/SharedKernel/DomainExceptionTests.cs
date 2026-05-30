using CookieStore.Domain;
using FluentAssertions;

namespace CookieStore.Tests.SharedKernel;

public class DomainExceptionTests
{
    [Fact]
    public void DomainException_WhenCreated_ShouldHaveCorrectMessage()
    {
        var ex = new DomainException("Price must be positive.");

        ex.Message.Should().Be("Price must be positive.");
    }

    [Fact]
    public void DomainException_ShouldInheritFromException()
    {
        var ex = new DomainException("Error.");

        ex.Should().BeAssignableTo<Exception>();
    }
}
