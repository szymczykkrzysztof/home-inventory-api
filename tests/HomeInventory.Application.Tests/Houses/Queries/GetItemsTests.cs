using FluentAssertions;
using HomeInventory.Application.Houses.Queries.GetItems;
using HomeInventory.Application.Tests.TestDoubles;

namespace HomeInventory.Application.Tests.Houses.Queries;

public class GetItemsTests
{
    [Fact]
    public async Task ShouldReturnItems()
    {
        var repository = new FakeHouseReadRepository
        {
            Items =
            [
                new ItemDto
                {
                    ItemId = Guid.NewGuid(),
                    Name = "Test Item",
                    ImageUrl = "https://example.com/test.jpg",
                    RoomName = "Living Room",
                    ContainerName = null
                }
            ]
        };

        var handler = new GetItemsQueryHandler(repository);
        var query = new GetItemsQuery(
            Guid.NewGuid(), null, null, null);
        var result = await handler.Handle(query, default);

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Test Item");
    }

    [Fact]
    public async Task ShouldReturnEmptyList()
    {
        var repository = new FakeHouseReadRepository {Items = []};
        var handler = new GetItemsQueryHandler(repository);
        var query = new GetItemsQuery(
            Guid.NewGuid(), null, null, null);
        var result = await handler.Handle(query, default);

        result.Should().BeEmpty();
    }
}