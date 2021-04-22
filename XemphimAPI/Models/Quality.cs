using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Quality
    {
        public int id;
        public int id_serve;
        public int id_movie;
        public string name;
        public string url;
        public int width;
        public int height;
        public string type_file;
        public int size_MB;
        public DateTime? creattime;
        public int creat_user;
        public DateTime?  updatetime;
        public int update_user;
        Quality() { }
    }
}