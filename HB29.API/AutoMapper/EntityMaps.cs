using AutoMapper;
using hb29.API.DTOs;
using hb29.API.Models;

namespace hb29.API.AutoMapper
{
    public class ServiceSettingParaServiceSettingsDTO : global::AutoMapper.Profile
    {
        public ServiceSettingParaServiceSettingsDTO()
        {
            CreateMap<ServiceSetting, ServiceSettingDTO>(MemberList.None);
            CreateMap<ServiceSettingDTO, ServiceSetting>(MemberList.None);
        }
    }

    public class PrivacyPolicyParaPrivacyPolicyDTO : global::AutoMapper.Profile
    {
        public PrivacyPolicyParaPrivacyPolicyDTO()
        {
            CreateMap<PrivacyPolicy, PrivacyPolicyDTO>(MemberList.None);
            CreateMap<PrivacyPolicyDTO, PrivacyPolicy>(MemberList.None);
        }
    }

    public class PermissionsParaPermissionsDTO : global::AutoMapper.Profile
    {
        public PermissionsParaPermissionsDTO()
        {
            CreateMap<Permission, PermissionDTO>(MemberList.None);
            CreateMap<PermissionDTO, Permission>(MemberList.None);
        }
    }

    public class ProfilesParaProfilesDTO : global::AutoMapper.Profile
    {
        public ProfilesParaProfilesDTO()
        {
            CreateMap<Models.Profile, ProfileDTO>(MemberList.None);
            CreateMap<ProfileDTO, Models.Profile>(MemberList.None);
        }
    }
}