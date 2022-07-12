using AutoMapper;

namespace Application.Common.Mapping;

public interface IMapTo<T>
{
    void MappingTo(Profile profile)
    {
        profile.CreateMap(GetType(), typeof(T));
    }
}