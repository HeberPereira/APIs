using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hb29.API.Controllers.CustomRoles
{
    public static class PrivacyPolicyCustomRoles
    {        
        public const string UPDATE_PRIVACY_POLICY = "UPDATE_PRIVACY_POLICY";
        public const string SAVE_PRIVACY_POLICY = "SAVE_PRIVACY_POLICY";
        public const string REMOVE_PRIVACY_POLICY = "REMOVE_PRIVACY_POLICY";
        
        public const string ADMIN = UPDATE_PRIVACY_POLICY + "," +
                                    SAVE_PRIVACY_POLICY + "," +
                                    REMOVE_PRIVACY_POLICY;

    }
}
