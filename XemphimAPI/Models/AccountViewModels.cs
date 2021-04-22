using System;
using System.Collections.Generic;

namespace XemphimAPI.Models
{
    public class Manager_User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public int level { get; set; }
        public string totaltime { get; set; }
        public List<string> listtime { get; set; }
        public List<string> listmac { get; set; }
        public int level_manager { get; set; }

        public Manager_User() { }
    }
    public class RETURN_TOKEN
    {
        public long token { get; set; }
        public long t_user { get; set; }
        public RETURN_TOKEN() { }
    }
    public class KEYUSERUPDATE
    {
        public string pass { get; set; }
        public int? level { get; set; }
        public int? count_video { get; set; }
        public string urlavatar { get; set; }
        public string email { get; set; }
        public int? bri_day { get; set; }
        public int? bri_month { get; set; }
        public int? bri_year { get; set; }
        public string thanhpho { get; set; }
        public string diachi { get; set; }
        public string hoten { get; set; }
        public int? gioitinh { get; set; }
        public string sdt { get; set; }
        public string nghenghiep { get; set; }
        public string sothich { get; set; }
        public KEYUSERUPDATE() { }
    }
    public class User_token
    {
        public User user { get; set; }

        public long token { get; set; }
        public int status { get; set; }
        public string content { get; set; }
        public User_token() { }
    }
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
