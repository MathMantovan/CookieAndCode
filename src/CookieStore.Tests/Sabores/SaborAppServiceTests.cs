using CookieStore.Application.Sabores;
using CookieStore.Domain;
using CookieStore.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CookieStore.Tests.Sabores;

public class SaborAppServiceTests
{
    private readonly ISaborRepository _repository = Substitute.For<ISaborRepository>();
    private readonly SaborAppService _sut;

    public SaborAppServiceTests() => _sut = new SaborAppService(_repository);

    // -----------------------------------------------------------------------
    // CreateAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaborAppService_CreateAsync_WithValidData_ShouldCallAddAsync()
    {
        // Act
        await _sut.CreateAsync("Chocolate", catalogPrice: 10.00m);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Any<Sabor>());
    }

    [Fact]
    public async Task SaborAppService_CreateAsync_ShouldReturnGeneratedSaborId()
    {
        // Act
        var id = await _sut.CreateAsync("Chocolate", catalogPrice: 10.00m);

        // Assert
        id.Should().NotBeNull();
        id.Value.Should().NotBeEmpty();
    }

    // -----------------------------------------------------------------------
    // UpdatePriceAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaborAppService_UpdatePriceAsync_WithExistingSabor_ShouldCallUpdateAsync()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        _repository.GetByIdAsync(saborId).Returns(sabor);

        // Act
        await _sut.UpdatePriceAsync(saborId, newPrice: 15.00m);

        // Assert
        await _repository.Received(1).UpdateAsync(sabor);
    }

    [Fact]
    public async Task SaborAppService_UpdatePriceAsync_WithNonExistentSabor_ShouldThrowDomainException()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        _repository.GetByIdAsync(saborId).ReturnsNull();

        // Act
        var act = async () => await _sut.UpdatePriceAsync(saborId, newPrice: 15.00m);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    // -----------------------------------------------------------------------
    // DeactivateAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaborAppService_DeactivateAsync_WithExistingSabor_ShouldCallUpdateAsync()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        _repository.GetByIdAsync(saborId).Returns(sabor);

        // Act
        await _sut.DeactivateAsync(saborId);

        // Assert
        await _repository.Received(1).UpdateAsync(sabor);
    }

    [Fact]
    public async Task SaborAppService_DeactivateAsync_WithNonExistentSabor_ShouldThrowDomainException()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        _repository.GetByIdAsync(saborId).ReturnsNull();

        // Act
        var act = async () => await _sut.DeactivateAsync(saborId);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    // -----------------------------------------------------------------------
    // GetAllActiveAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaborAppService_GetAllActiveAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var sabores = new List<Sabor>
        {
            new("Chocolate", PrecoTabela.Create(10.00m)),
            new("Morango", PrecoTabela.Create(8.00m))
        };
        _repository.GetAllActiveAsync().Returns(sabores);

        // Act
        var result = (await _sut.GetAllActiveAsync()).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Chocolate");
        result[0].CatalogPrice.Should().Be(10.00m);
        result[0].IsActive.Should().BeTrue();
        result[1].Name.Should().Be("Morango");
    }

    [Fact]
    public async Task SaborAppService_GetAllActiveAsync_WithNoActiveSabores_ShouldReturnEmptyList()
    {
        // Arrange
        _repository.GetAllActiveAsync().Returns([]);

        // Act
        var result = await _sut.GetAllActiveAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
