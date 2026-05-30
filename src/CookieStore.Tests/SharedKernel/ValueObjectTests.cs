using CookieStore.Domain.SharedKernel;
using FluentAssertions;

namespace CookieStore.Tests.SharedKernel;

/// <summary>
/// Tests for the abstract ValueObject base class (DDD: Value Objects have structural equality —
/// two instances with the same component values are interchangeable and equal).
/// </summary>
public class ValueObjectTests
{
    // -----------------------------------------------------------------------
    // Concrete stubs to exercise the abstract ValueObject
    // -----------------------------------------------------------------------

    /// <summary>Single-component VO, mirrors the pattern used by PrecoTabela, Rendimento, etc.</summary>
    private sealed class MoneyVO : ValueObject
    {
        public decimal Amount { get; }

        private MoneyVO(decimal amount)
        {
            Amount = amount;
        }

        public static MoneyVO Create(decimal amount) => new(amount);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
        }
    }

    /// <summary>Multi-component VO to verify all components participate in equality.</summary>
    private sealed class RangeVO : ValueObject
    {
        public int Min { get; }
        public int Max { get; }

        private RangeVO(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public static RangeVO Create(int min, int max) => new(min, max);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Min;
            yield return Max;
        }
    }

    // -----------------------------------------------------------------------
    // Single-component VO equality
    // -----------------------------------------------------------------------

    [Fact]
    public void ValueObject_WithSameComponents_Equals_ShouldBeTrue()
    {
        // Arrange
        var vo1 = MoneyVO.Create(10.00m);
        var vo2 = MoneyVO.Create(10.00m);

        // Act & Assert
        vo1.Equals(vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_WithSameComponents_EqualityOperator_ShouldBeTrue()
    {
        // Arrange
        ValueObject vo1 = MoneyVO.Create(10.00m);
        ValueObject vo2 = MoneyVO.Create(10.00m);

        // Act & Assert
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_WithDifferentComponents_Equals_ShouldBeFalse()
    {
        // Arrange
        var vo1 = MoneyVO.Create(10.00m);
        var vo2 = MoneyVO.Create(20.00m);

        // Act & Assert
        vo1.Equals(vo2).Should().BeFalse();
    }

    [Fact]
    public void ValueObject_WithDifferentComponents_InequalityOperator_ShouldBeTrue()
    {
        // Arrange
        ValueObject vo1 = MoneyVO.Create(10.00m);
        ValueObject vo2 = MoneyVO.Create(20.00m);

        // Act & Assert
        (vo1 != vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_WithSameComponents_GetHashCode_ShouldBeEqual()
    {
        // Arrange
        var vo1 = MoneyVO.Create(10.00m);
        var vo2 = MoneyVO.Create(10.00m);

        // Act & Assert
        vo1.GetHashCode().Should().Be(vo2.GetHashCode());
    }

    [Fact]
    public void ValueObject_ComparedToNull_ShouldNotBeEqual()
    {
        // Arrange
        var vo = MoneyVO.Create(10.00m);

        // Act & Assert
        vo.Equals(null).Should().BeFalse();
    }

    // -----------------------------------------------------------------------
    // Multi-component VO equality — all components must match
    // -----------------------------------------------------------------------

    [Fact]
    public void ValueObject_MultiComponent_AllComponentsMatch_ShouldBeEqual()
    {
        // Arrange
        var vo1 = RangeVO.Create(1, 10);
        var vo2 = RangeVO.Create(1, 10);

        // Act & Assert
        vo1.Equals(vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_MultiComponent_FirstComponentDiffers_ShouldNotBeEqual()
    {
        // Arrange
        var vo1 = RangeVO.Create(1, 10);
        var vo2 = RangeVO.Create(2, 10);

        // Act & Assert
        vo1.Equals(vo2).Should().BeFalse();
    }

    [Fact]
    public void ValueObject_MultiComponent_SecondComponentDiffers_ShouldNotBeEqual()
    {
        // Arrange
        var vo1 = RangeVO.Create(1, 10);
        var vo2 = RangeVO.Create(1, 20);

        // Act & Assert
        vo1.Equals(vo2).Should().BeFalse();
    }

    [Fact]
    public void ValueObject_MultiComponent_AllComponentsMatch_GetHashCode_ShouldBeEqual()
    {
        // Arrange
        var vo1 = RangeVO.Create(5, 15);
        var vo2 = RangeVO.Create(5, 15);

        // Act & Assert
        vo1.GetHashCode().Should().Be(vo2.GetHashCode());
    }
}
