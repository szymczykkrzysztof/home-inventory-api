using HomeInventory.Application.Contracts;
using HomeInventory.Domain.Aggregates.House;
using MediatR;

namespace HomeInventory.Application.Houses.Commands.Manage.Register;

public class RegisterHouseCommandHandler(IHouseRepository houseRepository)
    : IRequestHandler<RegisterHouserCommand, Guid>
{
    public async Task<Guid> Handle(RegisterHouserCommand request, CancellationToken cancellationToken)
    {
        var house = House.Create(request.Name);
        await houseRepository.Add(house, cancellationToken);
        await houseRepository.SaveChanges(cancellationToken);
        return house.Id;
    }
}