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
            .Map(dest => dest.RoleId, src => src.RoleId);
    }
}
