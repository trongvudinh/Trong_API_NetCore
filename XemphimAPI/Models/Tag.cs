using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Tag
    {
        public int id;
        public string name;
        public string name_re;
        public int sl_movie;
        public DateTime? creattime;
        public int? user_creat;
        public DateTime? updatetime;
        public int? user_update;
        public Tag() { }
        public Tag(int id, string name,  string name_re, int sl_movie)
        {
            this.id = id;
            this.name = name;
            this.name_re = name_re;
            this.sl_movie = sl_movie;
        }
    }
}