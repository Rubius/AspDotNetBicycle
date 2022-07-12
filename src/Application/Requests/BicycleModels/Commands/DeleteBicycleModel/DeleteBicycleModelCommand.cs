using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.BicycleModels.Commands.DeleteBicycleModel;

public class DeleteBicycleModelCommand : IRequest
{
    public DeleteBicycleModelCommand(ulong id)
    {
        Id = id;
    }

    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; }
}

public class DeleteBicycleModalsCommandHandler : IRequestHandler<DeleteBicycleModelCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBicycleModalsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteBicycleModelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BicycleModels.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BicycleModel), request.Id.ToString());
        }
        _context.BicycleModels.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}