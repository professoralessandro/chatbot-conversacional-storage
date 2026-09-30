#region IMPORT
using ChatbotConversacionalStorage.Application.Dtos.File.Request;
using ChatbotConversacionalStorage.Domain.Entities;
using AutoMapper;
#endregion

namespace ChatbotConversacionalStorage.Infrastructure.Mappers
{
    public class MapperDtoToEntity : Profile
    {
        public MapperDtoToEntity()
        {
            #region USER
            //CreateMap<UserDto, Usuario>()
            //.ForMember(memb => memb.UsuarioId, m => m.MapFrom(a => a.Identifier))
            //.ForMember(memb => memb.Senha, m => m.MapFrom(a => a.Password))
            //.ForMember(memb => memb.Login, m => m.MapFrom(a => a.UserName))
            //.ForMember(memb => memb.TipoDocumentoId, m => m.MapFrom(a => a.TipoDocumento))
            //.ReverseMap();

            //CreateMap<UserResponseDto, Usuario>()
            //.ForMember(memb => memb.UsuarioId, m => m.MapFrom(a => a.Identifier))
            //.ReverseMap();

            //CreateMap<UserResponseDto, UserDto>()
            //.ReverseMap();
            #endregion USER

            #region FILE
            CreateMap<FileRequestPostDto, StorageFile>()
            .ReverseMap();

            CreateMap<FileResponseDto, StorageFile>()
            .ReverseMap();
            #endregion FILE
        }
    }
}
