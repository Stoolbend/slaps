using AutoMapper;
using BCI.SLAPS.Server.Model.Chat;
using BCI.SLAPS.Server.Model.Media;
using BCI.SLAPS.Server.Model.User;

namespace BCI.SLAPS.Server.Services
{
    public class DefaultMapperProfile : Profile
    {
        public DefaultMapperProfile()
        {
            #region Users
            CreateMap<User, UserDTO>();
            CreateMap<User, UserLoginResponseDTO>();
            CreateMap<UserLoginRequestDTO, User>();
            #endregion

            #region Slides
            CreateMap<Slide, SlideClientDTO>();
            CreateMap<Slide, SlideAdminDTO>()
                .ReverseMap();
            CreateMap<Slide, SlideClientContentDTO>();
            CreateMap<SlideAdminCreateDTO, Slide>();

            CreateMap<SlideSet, SlideSetDTO>()
                .ForMember(dest => dest.Slides, opt => opt.MapFrom(src => src.Slides));
            CreateMap<SlideSet, SlideSetAdminDTO>()
                .ReverseMap();
            CreateMap<SlideSetAdminCreateDTO, SlideSet>();
            #endregion

            #region Posts
            CreateMap<Post, PostDTO>();
            CreateMap<PostDTOBase, Post>();
            #endregion
        }
    }
}