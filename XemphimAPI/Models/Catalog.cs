using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Catalog
    {
        public int id;
        public string name;
        public string name_re;
        public string name_en;
        public string urlavatar;
        public int id_menu;
        public Catalog() { }
        public Catalog(int id, string name, string name_re, string name_en, string urlavatar, int id_menu)
        {
            this.id = id;
            this.name = name;
            this.name_re = name_re;
            this.name_en = name_en;
            this.urlavatar = urlavatar;
            this.id_menu = id_menu;
        }
    }
}