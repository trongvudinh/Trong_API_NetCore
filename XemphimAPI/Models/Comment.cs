using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XemphimAPI.Models
{
    public class Comment
    {
        public long id;
        public int id_movie;
        public string content;
        public DateTime? creattime;
        public int id_user;
        public string user_name;
        public long id_cmt_parent;
        public int level;
        public string urlavatar;
        public int has_child;
        public int? n_like;
        public int? n_dislike;
        public int type;

        public Comment() { }
        public Comment(long id, int id_movie, string content, DateTime creattime, int id_user, string user_name, long id_cmt_parent, int level, string urlavatar, int has_child
                        , int n_like, int n_dislike, int type)
        {
            this.id = id;
            this.id_movie = id_movie;
            this.content = content;
            this.creattime = creattime;
            this.id_user = id_user;
            this.user_name = user_name;
            this.id_cmt_parent = id_cmt_parent;
            this.level = level;
            this.urlavatar = urlavatar;
            this.has_child = has_child;
            this.n_like = n_like;
            this.n_dislike = n_dislike;
            this.type = type;
        }
    }
}