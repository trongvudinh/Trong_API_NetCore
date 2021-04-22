using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Serve
    {
        public int id;
        public int id_movie;
        public int id_serve;
        public string url;
        public List<Quality> list_quality;
        public Serve() { }
        public Serve(int id, int id_serve, string url)
        {
            this.id = id;
            this.id_serve = id_serve;
            this.url = url;
        }
    }
}