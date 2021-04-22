using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Test
    {
            public int idmovie;
            public int idcomment;
            public int iduser;
        public Test() { }
        public Test(int idmovie, int idcomment, int iduser)
        {
            this.idmovie = idmovie;
            this.idcomment = idcomment;
            this.iduser = iduser;

        }
    }
}