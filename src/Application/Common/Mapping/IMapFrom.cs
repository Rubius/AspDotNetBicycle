using AutoMapper;

namespace Application.Common.Mapping;

public interface IMapFrom<T>
{
    void MappingFrom(Profile profile)
    {
        profile.CreateMap(typeof(T), GetType());
    }
}