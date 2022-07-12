using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.BicycleBrands.Commands.DeleteBicycleBrand;

public class DeleteBicycleBrandCommand : IRequest
{
    public DeleteBicycleBrandCommand(ulong id)
    {
        Id = id;
    }

    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; }
}

public class DeleteBicycleModalsCommandHandler : IRequestHandler<DeleteBicycleBrandCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBicycleModalsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteBicycleBrandCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BicycleBrands.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BicycleBrand), request.Id.ToString());
        }
        _context.BicycleBrands.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}