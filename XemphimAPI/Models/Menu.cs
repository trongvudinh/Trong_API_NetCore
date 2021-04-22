using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Menu
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_re { get; set; }
        public string name_en { get; set; }
        public List<Catalog> list_catalog;
        public DateTime? creattime { get; set; }
        public string usercreat { get; set; }
        public DateTime? updatetime { get; set; }
        public string userupdate { get; set; }
        public Menu() { }
        public Menu(int id, string name, string name_re, string name_en, List<Catalog> list_catalog)
        {
            this.id = id;
            this.name = name;
            this.name_re = name_re;
            this.name_en = name_en;
            this.list_catalog = list_catalog;
        }

    }
}