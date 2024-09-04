using hb29.API.Helpers.PaginationUri;
using hb29.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hb29.API.Helpers
{
    public static class ServiceSettingHelper
    {
        public static T GetValue<T>(this ServiceSetting setting)
        {
            if (setting.Type != typeof(T).Name)
                throw new InvalidCastException($"Setting {setting.Name} type is {setting.Type}.");

            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }
    }
}
