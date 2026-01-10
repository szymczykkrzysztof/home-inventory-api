using HomeInventory.API.Dtos.Houses;
using HomeInventory.Application.Houses.Commands.Items.AddItem;
using HomeInventory.Application.Houses.Commands.Items.MoveItem;
using HomeInventory.Application.Houses.Commands.Items.RemoveItem;
using HomeInventory.Application.Houses.Commands.Items.UpdateItem;
using HomeInventory.Application.Houses.Commands.Locations.AddLocation;
using HomeInventory.Application.Houses.Commands.Locations.RemoveLocation;
using HomeInventory.Application.Houses.Commands.Locations.RenameLocation;
using HomeInventory.Application.Houses.Commands.Manage.Register;
using HomeInventory.Application.Houses.Queries.GetDetail;
using HomeInventory.Application.Houses.Queries.GetHouses;
using HomeInventory.Application.Houses.Queries.GetItems;
using HomeInventory.Application.Houses.Queries.GetLocations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.API.Controllers;

[ApiController]
[Route("api/houses")]
public class HousesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<HouseLookupDto>>> GetHouses(
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetHousesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{houseId}")]
    public async Task<ActionResult<HouseDetailDto>> GetHouseDetail(
        Guid houseId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetHouseDetailQuery(houseId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{houseId:guid}/locations")]
    public async Task<ActionResult<List<LocationLookupDto>>> GetLocations(
        Guid houseId,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetLocationsQuery(houseId), ct);
        return Ok(result);
    }

    [HttpGet("{houseId:guid}/items")]
    public async Task<IActionResult> GetItems(
        Guid houseId,
        [FromQuery] string? search,
        [FromQuery] string? room,
        [FromQuery] string? container,
        CancellationToken cancellationToken)
    {
        var query = new GetItemsQuery(
            houseId,
            search,
            room,
            container);

        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }


    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterHouse(
        [FromBody] RegisterHouseCommand command,
        CancellationToken ct)
    {
        var houseId = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetHouseDetail), new { houseId }, houseId);
    }

    [HttpPost("{houseId:guid}/locations")]
    public async Task<ActionResult<Guid>> AddLocation(
        Guid houseId,
        [FromBody] AddLocationRequest request,
        CancellationToken ct)
    {
        var command = new AddLocationCommand(
            houseId,
            request.RoomName,
            request.ContainerName);

        var locationId = await mediator.Send(command, ct);

        return CreatedAtAction(
            nameof(GetHouseDetail),
            new { houseId },
            locationId);
    }

    [HttpPost("{houseId}/locations/{locationId}/items")]
    public async Task<ActionResult<Guid>> AddItem(
        Guid houseId,
        Guid locationId,
        AddItemRequest request,
        CancellationToken cancellationToken)
    {
        var itemId = await mediator.Send(
            new AddItemCommand(
                houseId,
                locationId,
                request.Name,
                request.ImageUrl),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetHouseDetail),
            new { houseId },
            itemId);
    }

    [HttpPost("{houseId:guid}/items/{itemId:guid}/move")]
    public async Task<IActionResult> MoveItem(
        Guid houseId,
        Guid itemId,
        [FromBody] MoveItemRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new MoveItemCommand(
            houseId,
            itemId,
            request.FromLocationId,
            request.ToLocationId), ct);

        return NoContent();
    }

    [HttpPut("{houseId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> RenameLocation(
        Guid houseId,
        Guid locationId,
        RenameLocationRequest request,
        CancellationToken ct)
    {
        await mediator.Send(
            new RenameLocationCommand(
                houseId,
                locationId,
                request.RoomName,
                request.ContainerName),
            ct);

        return NoContent();
    }

    [HttpDelete("{houseId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> RemoveLocation(
        Guid houseId,
        Guid locationId,
        CancellationToken ct)
    {
        await mediator.Send(
            new RemoveLocationCommand(houseId, locationId),
            ct);

        return NoContent();
    }

    [HttpPut("{houseId:guid}/locations/{locationId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid houseId,
        Guid locationId,
        Guid itemId,
        [FromBody] UpdateItemRequest request,
        CancellationToken ct)
    {
        await mediator.Send(
            new UpdateItemCommand(
                houseId,
                locationId,
                itemId,
                request.Name,
                request.ImageUrl),
            ct);

        return NoContent();
    }

    [HttpDelete("{houseId:guid}/locations/{locationId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid houseId,
        Guid locationId,
        Guid itemId,
        CancellationToken ct)
    {
        await mediator.Send(
            new RemoveItemCommand(houseId, locationId, itemId),
            ct);

        return NoContent();
    }
}