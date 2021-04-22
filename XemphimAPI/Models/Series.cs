using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Series
    {
        public int id;
        public string name;
        public int count_movie;
        public string urlavatar;
        public int year_str;
        public int year_end;
        public string content;
        public string content_re;
        public int warning;
        public Company company;
        public DateTime? creattime;
        public int? user_creat;
        public DateTime? updatetime;
        public int? user_update;
        public Catalog[]list_cata;
        public Tag[] list_tag;
        public Actor[] list_actor;

        public Series() { }
        public Series(int id, string name, int count_movie, string urlavatar, int year_str, int year_end,
            string content,string content_re,int warning,Company company)
        {
            this.id = id;
            this.name = name;
            this.count_movie = count_movie;
            this.urlavatar = urlavatar;
            this.year_str = year_str;
            this.year_end = year_end;
            this.content = content;
            this.content_re = content_re;
            this.warning = warning;
            this.company = company;
        }
    }
}