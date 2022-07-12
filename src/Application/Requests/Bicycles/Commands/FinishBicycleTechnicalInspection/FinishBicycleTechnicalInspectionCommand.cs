using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SwaggerIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Application.Requests.Bicycles.Commands.FinishBicycleTechnicalInspection
{
    public class FinishBicycleTechnicalInspectionCommand : IRequest
    {
        [SwaggerIgnore]
        public ulong Id { get; set; }

        /// <summary>
        /// Результат тех осмотра
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public BicycleTechnicalStatus TechnicalStatus { get; set; }
    }

    public class FinishBicycleTechnicalInspectionCommandHandler : IRequestHandler<FinishBicycleTechnicalInspectionCommand>
    {
        private readonly IApplicationDbContext _context;

        public FinishBicycleTechnicalInspectionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(FinishBicycleTechnicalInspectionCommand request, CancellationToken cancellationToken)
        {
            var bicycle = await _context.Bicycles.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (bicycle is null)
            {
                throw new NotFoundException(nameof(Bicycle), request.Id.ToString());
            }

            bicycle.FinishTechnicalInspection(request.TechnicalStatus);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
