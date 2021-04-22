using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Company
    {
        public int id;
        public string name;
        public string name_re;
        public string name_en;
        public string urlavatar;
        public string content;
        public DateTime? creattime;
        public int? user_creat;
        public DateTime? updatetime;
        public int? user_update;
        public Company() { }
        public Company(int id, string name, string name_re, string name_en, string urlavatar, string content)
        {
            this.id = id;
            this.name = name;
            this.name_re = name_re;
            this.name_en = name_en;
            this.urlavatar = urlavatar;
            this.content = content;
        }
    }
}