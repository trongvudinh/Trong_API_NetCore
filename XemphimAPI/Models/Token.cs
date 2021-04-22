using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Token
    {
        public long id;
        public string ipaddress;
        public float geo_1;
        public float geo_2;
        public string macadress;
        public Token() { }
        public Token(long id,string ipaddress,float geo_1,float geo_2,string macadress)
        {
            this.id = id;
            this.ipaddress = ipaddress;
            this.geo_1 = geo_1;
            this.geo_2 = geo_2;
            this.macadress = macadress;
        }
    }
}