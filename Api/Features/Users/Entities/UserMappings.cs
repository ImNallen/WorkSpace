using Mapster;

namespace Api.Features.Users.Entities;

public class UserMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<User, UserDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email.Value)
            .Map(dest => dest.FirstName, src => src.FirstName.Value)
            .Map(dest => dest.LastName, src => src.LastName.Value)
            .Map(dest => dest.Role, src => src.Role.Name.Value)
            .Map(dest => dest.CreatedOnUtc, src => src.CreatedOnUtc);
    }
}
