using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class User
    {
        public int id;
        public string name;
        public string pass;
        public int level;
        public int count_video;
        public string urlavatar;
        public string email;
        public int bri_day;
        public int bri_month;
        public int bri_year;
        public string thanhpho;
        public string diachi;
        public string hoten;
        public int gioitinh;
        public string sdt;
        public string nghenghiep;
        public string sothich;
        public User() { }
        public User(int id, string name, string pass, int level, string urlavatar, int count_video)
        {
            this.id = id;
            this.name = name;
            this.pass = pass;
            this.level = level;
            this.urlavatar = urlavatar;
            this.count_video = count_video;
        }
    }
}