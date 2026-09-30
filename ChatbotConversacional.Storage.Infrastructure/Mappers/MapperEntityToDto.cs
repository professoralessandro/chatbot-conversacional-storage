#region IMPORTS
using ChatbotConversacionalStorage.Application.Dtos.File.Request;
using ChatbotConversacionalStorage.Domain.Entities;
using AutoMapper;
#endregion

namespace ChatbotConversacionalStorage.Infrastructure.Mappers
{
    public class MapperEntityToDto : Profile
    {
        public MapperEntityToDto()
        {
            #region USER
            //CreateMap<Usuario, UserDto>()
            //.ForMember(memb => memb.Identifier, m => m.MapFrom(a => a.UsuarioId))
            //.ForMember(memb => memb.Password, m => m.MapFrom(a => a.Senha))
            //.ForMember(memb => memb.UserName, m => m.MapFrom(a => a.Login))
            //.ForMember(memb => memb.TipoDocumento, m => m.MapFrom(a => a.TipoDocumentoId));

            //CreateMap<Usuario, UserResponseDto>()
            //.ForMember(memb => memb.Identifier, m => m.MapFrom(a => a.UsuarioId));

            //CreateMap<UserDto, UserResponseDto>();
            #endregion USER

            #region FILE
            CreateMap<StorageFile, FileRequestPostDto>();

            CreateMap<StorageFile, FileResponseDto>();
            #endregion FILE
        }
    }
}
