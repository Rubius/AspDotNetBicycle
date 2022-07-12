using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Requests.Bicycles.Commands.DeleteBicycle;

public class DeleteBicycleCommand : IRequest
{
    public DeleteBicycleCommand(ulong id)
    {
        Id = id;
    }
    [JsonProperty(Required = Required.Always)]
    public ulong Id { get; }
}

public class DeleteBicycleCommandHandler : IRequestHandler<DeleteBicycleCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBicycleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteBicycleCommand request, CancellationToken cancellationToken)
    {

        var entity = await _context.Bicycles.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Bicycle), request.Id.ToString());
        }
        _context.Bicycles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}