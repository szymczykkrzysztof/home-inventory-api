using FluentAssertions;
using HomeInventory.Application.Houses.Queries.GetDetail;
using HomeInventory.Application.Tests.TestDoubles;
using HomeInventory.Domain.Exceptions;

namespace HomeInventory.Application.Tests.Houses.Queries;

public class GetHouseDetailTests
{
    [Fact]
    public async Task ShouldReturnHouseDetailWhenExists()
    {
        var repository = new FakeHouseReadRepository
        {
            HouseDetail = new HouseDetailDto
            {
                HouseId = Guid.NewGuid(),
                Name = "Test House",
                Locations = []
            }
        };
        var handler = new GetHouseDetailQueryHandler(repository);
        var query = new GetHouseDetailQuery(repository.HouseDetail.HouseId);

        var result = await handler.Handle(query, default);

        result.Should().NotBeNull();
        result.Name.Should().Be(repository.HouseDetail.Name);
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHouseDoesNotExist()
    {
        var repository = new FakeHouseReadRepository();
        var handler = new GetHouseDetailQueryHandler(repository);
        var query = new GetHouseDetailQuery(Guid.NewGuid());

        var act = async () => await handler.Handle(query, default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}