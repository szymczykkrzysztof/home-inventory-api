using MediatR;

namespace HomeInventory.Application.Houses.Commands.Manage.Register;

public sealed record RegisterHouseCommand(string Name) : IRequest<Guid>;