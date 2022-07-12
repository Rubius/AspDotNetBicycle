using AutoMapper;
using Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Common.Behaviours;

internal class MappingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const ushort FINDING_DEPTH = 4;
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (AutoMapperMappingException ex)
        {
            var innerMappingValidationException = FindInnerMappingValidationException(ex);
            if (innerMappingValidationException is not null)
            {
                throw NewValidationException(innerMappingValidationException);
            }
            throw ex;
        }
        catch (MappingValidationException ex)
        {
            throw NewValidationException(ex);
        }
    }

    private static ValidationException NewValidationException(MappingValidationException ex)
    {
        return new ValidationException(new List<ValidationFailure>()
        {
            new (ex.PropertyName, ex.Message)
        });
    }

    private MappingValidationException? FindInnerMappingValidationException(Exception ex, int i = 0)
    {
        if (ex.InnerException is null || i++ > FINDING_DEPTH)
        {
            return null;
        }

        if (ex.InnerException is MappingValidationException mappingEx)
        {
            return mappingEx;
        }

        return FindInnerMappingValidationException(ex.InnerException);
    }
}