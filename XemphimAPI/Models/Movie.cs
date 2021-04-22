using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Movie
    {
        public int id;
        public string name;
        public string name_re;
        public string name_en;
        public int n_view;
        public int n_like;
        public int year_movie;
        public DateTime? createtime;
        public User user_creat;
        public DateTime? updatetime;
        public User user_update;
        public int time_thoiluong;
        public int warning;
        public string content;
        public string content_re;
        public string urlavatar;
        public Company company;
        public Series series;
        public int type_mov;
        public Actor[] actor;
        public Serve[] serve;
        public Tag[] tag;
        public Catalog[] catalog;
        public Movie() { }
    }
}