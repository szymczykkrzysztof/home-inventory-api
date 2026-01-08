using MediatR;

namespace HomeInventory.Application.Houses.Commands.Manage.Register;

public sealed record RegisterHouserCommand(string Name) : IRequest<Guid>;