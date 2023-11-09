using src.Core.Domains;
using src.Web.Common.Models.UserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Web.Common.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {

            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.AuthorizedRoleIds,
                    mo => mo.MapFrom(src =>
                        src.UserRoles != null ? src.UserRoles.Select(r => r.RoleId).ToList() : new List<int>()));

            CreateMap<User, UserCreateUpdateViewModel>();
            CreateMap<UserCreateUpdateViewModel, User>()
                .ForMember(dest => dest.UserName, mo => mo.MapFrom(src => src.UserName.ToLowerInvariant()))
                .ForMember(dest => dest.LastLoginDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedOn, opt => opt.Ignore());
        }

    }
}
