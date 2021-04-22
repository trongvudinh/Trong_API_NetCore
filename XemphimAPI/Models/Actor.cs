using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Actor
    {
        public int id;
        public string name;
        public DateTime birtday;
        public string content;
        public string urlavatar;
        public int? famail;
        public int? type;
        public Actor() { }
        public Actor(int id, string name, DateTime britday, string content, string urlavatar, int famail)
        {
            this.id = id;
            this.name = name;
            this.birtday = britday;
            this.content = content;
            this.urlavatar = urlavatar;
            this.famail = famail;
        }
    }
}