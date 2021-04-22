using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XemphimAPI.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace XemphimAPI.Controllers
{
    [AllowAnonymous]
    public class CommentController : ApiController
    {
        List<Comment> lst_cmt_temp = new List<Comment>();
        int n = 0;
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        public void swap_cmttemp(long id, int level)
        {
            List<Comment> lst_select;

            lst_select = lst_cmt_temp.Where(c => c.id_cmt_parent == id).OrderByDescending(o => o.creattime).ToList();
            foreach (Comment cmt in lst_select)
            {
                int index = lst_cmt_temp.IndexOf(cmt);
                Comment temp = lst_cmt_temp[n];
                lst_cmt_temp[n] = lst_cmt_temp[index];
                lst_cmt_temp[index] = temp;
                lst_cmt_temp[n].level = level;
                n = n + 1;
                long z = cmt.id;
                swap_cmttemp(z, level + 1);
            }
        }
        // GET api/values/5
        public HttpResponseMessage Get(int idmovie)
        {
            string json = "";
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql += "select  ";
                sql += "t0.id			id, ";
                sql += "t0.id_movie		id_movie, ";
                sql += "t0.content		content, ";
                sql += "t0.creattime	creattime, ";
                sql += "t0.id_user		id_user, ";
                sql += "t0.id_comment_cha	id_comment_cha, ";
                sql += "t1.urlavatar		urlavatar, ";
                sql += "t1.name				user_name ";
                sql += "from t_comment t0  ";
                sql += "left join  t_user t1 on t0.id_user=t1.id ";
                sql += "order by id_comment_cha asc,creattime desc";


                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                lst_cmt_temp = new List<Comment>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Comment cmt_temp = new Comment();
                    cmt_temp.id = Convert.ToInt32(r["id"].ToString());
                    cmt_temp.id_movie = Convert.ToInt32(r["id_movie"].ToString());
                    cmt_temp.content = r["content"].ToString();
                    if (r["creattime"].ToString() != "")
                        cmt_temp.creattime = Convert.ToDateTime(r["creattime"].ToString());
                    if (r["id_user"].ToString() != "")
                        cmt_temp.id_user = Convert.ToInt32(r["id_user"].ToString());
                    if (r["id_comment_cha"].ToString() != "")
                        cmt_temp.id_cmt_parent = Convert.ToInt32(r["id_comment_cha"].ToString());
                    lst_cmt_temp.Add(cmt_temp);
                    cmt_temp.user_name = r["id_comment_cha"].ToString();
                    cmt_temp.urlavatar = r["urlavatar"].ToString();
                }
                swap_cmttemp(0, 0);

                var data = new { catalogs = lst_cmt_temp };
                json = JsonConvert.SerializeObject(lst_cmt_temp);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }
            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            return res;
        }
        public HttpResponseMessage Get_byid(int idmovie, long idcomment)
        {
            string json = "";
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql += "select  ";
                sql += "t0.id			id, ";
                sql += "t0.id_movie		id_movie, ";
                sql += "t0.content		content, ";
                sql += "t0.creattime	creattime, ";
                sql += "t0.id_user		id_user, ";
                sql += "t0.id_comment_cha	id_comment_cha, ";
                sql += "t1.urlavatar		urlavatar, ";
                sql += "t1.name				user_name, ";
                sql += " IFNULL((select COUNT(t11.id) from t_comment t11 where t0.id=t11.id_comment_cha group by t11.id_comment_cha LIMIT 1),0) has_child, ";
                sql += " (select count(t21.id_user) from t_mov_coment_like t21 where t21.id_movie = t0.id_movie and t21.id_comment = t0.id)  n_like, ";
                sql += " (select count(t31.id_user) from t_mov_com_dislike t31 where t31.id_movie = 0 and t31.id_comment = 0)  n_dislike, ";
                sql += " t0.type_comment		type ";

                sql += "from t_comment t0  ";
                sql += "left join  t_user t1 on t0.id_user=t1.id ";
                sql += " where t0.id_comment_cha=" + idcomment;
                sql += " order by id_comment_cha asc,creattime desc";


                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                lst_cmt_temp = new List<Comment>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Comment cmt_temp = new Comment();
                    cmt_temp.id = Convert.ToInt32(r["id"].ToString());
                    cmt_temp.id_movie = Convert.ToInt32(r["id_movie"].ToString());
                    cmt_temp.content = r["content"].ToString();
                    if (r["creattime"].ToString() != "")
                        cmt_temp.creattime = Convert.ToDateTime(r["creattime"].ToString());
                    if (r["id_user"].ToString() != "")
                        cmt_temp.id_user = Convert.ToInt32(r["id_user"].ToString());
                    if (r["id_comment_cha"].ToString() != "")
                        cmt_temp.id_cmt_parent = Convert.ToInt32(r["id_comment_cha"].ToString());
                    lst_cmt_temp.Add(cmt_temp);
                    cmt_temp.user_name = r["id_comment_cha"].ToString();
                    cmt_temp.urlavatar = r["urlavatar"].ToString();
                    cmt_temp.has_child = Convert.ToInt32(r["has_child"].ToString());
                    cmt_temp.n_like = Convert.ToInt32(r["n_like"].ToString());
                    cmt_temp.n_dislike = Convert.ToInt32(r["n_dislike"].ToString());
                    cmt_temp.type = Convert.ToInt32(r["type"].ToString());
                }

                var data = new { catalogs = lst_cmt_temp };
                json = JsonConvert.SerializeObject(lst_cmt_temp);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }
            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            return res;
        }

        // POST api/values
        public HttpResponseMessage Post_com_like([FromBody] string val, int idmovie, long idcomment, int iduser, int com_like)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            var res = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                string sql = "";

                if (com_like == 0) sql += "select type from t_mov_coment_like where id_movie=" + idmovie + " and id_comment=" + idcomment + " and id_user=" + iduser;
                else sql += "select type from t_mov_com_dislike where id_movie=" + idmovie + " and id_comment=" + idcomment + " and id_user=" + iduser;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    if (com_like == 0) sql = "insert into t_mov_coment_like value(" + idmovie + "," + idcomment + "," + iduser + ",NOW(),0)  ";
                    else sql = "insert into t_mov_com_dislike value(" + idmovie + "," + idcomment + "," + iduser + ",NOW(),0)  ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                    if (i != 0)
                    {
                        if (com_like == 0)
                            sql = "update t_mov_com_dislike set type=1,createtime = NOW() where id_movie=" + idmovie + " and id_comment=" + idcomment + " and id_user=" + iduser;
                        else
                            sql = "update t_mov_coment_like set type=1,createtime = NOW() where id_movie=" + idmovie + " and id_comment=" + idcomment + " and id_user=" + iduser;
                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    if (ds.Tables[0].Rows[0]["type"].ToString() == "2")
                    {
                        if (com_like == 0)
                        {
                            sql = "";
                            sql += "update ";
                            sql += " t_mov_com_dislike t0 left join  t_mov_coment_like t1 on t0.id_movie=t1.id_movie and t0.id_comment=t1.id_comment and t0.id_user=t1.id_user ";
                            sql += "set t0.type = 1 ,t1.type = 0 ";
                            sql += "where t0.id_movie = " + idmovie + " and t0.id_comment = " + idcomment + " and t0.id_user = " + idcomment + "";
                        }
                        else
                        {
                            sql = "";
                            sql += "update ";
                            sql += "t_mov_coment_like t0 left join  t_mov_com_dislike t1 on t0.id_movie=t1.id_movie and t0.id_comment=t1.id_comment and t0.id_user=t1.id_user ";
                            sql += "set t0.type = 1 ,t1.type = 0 ";
                            sql += "where t0.id_movie = " + idmovie + " and t0.id_comment = " + idcomment + " and t0.id_user = " + idcomment + "";
                        }

                        cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                res = Request.CreateResponse(HttpStatusCode.Created, idcomment);
                return res;
            }
            catch (Exception e)
            {
                res = Request.CreateResponse(HttpStatusCode.BadRequest, e);
                return res;

            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }

        }
        public HttpResponseMessage Post_comment([FromBody] Comment com)
        {
            string json = "";
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            try
            {
                string sql = "";
                sql += "INSERT INTO t_comment (id_movie, content, creattime, id_user, id_comment_cha, type_comment)";
                sql += " VALUES ('" + com.id_movie + "', '" + com.content + "', NOW(), '" + com.id_user + "', '" + com.id_cmt_parent + "', '0');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                sql = " SELECT AUTO_INCREMENT as id FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'xemphim' AND TABLE_NAME = 't_comment'";

                cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                com.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString()) - 1;
                com.creattime = DateTime.Now;

                sql = " SELECT name FROM t_user WHERE id =" + com.id_user;

                cmd = new MySqlCommand(sql, conn);
                adap = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adap.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                    com.user_name = ds.Tables[0].Rows[0]["name"].ToString();
                com.urlavatar = "";
                com.has_child = 0;
                com.n_like = 0;
                com.n_dislike = 0;
                res = Request.CreateResponse(HttpStatusCode.Created, com);
                return res;

            }
            catch (Exception e)
            {
                res = Request.CreateResponse(HttpStatusCode.BadRequest, e);
                return res;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }
        }

        // PUT api/values/5
        public HttpResponseMessage Put(int idcomment, [FromBody]string value)
        {
            string json = "";
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            try
            {
                string sql = "";
                sql += "UPDATE t_comment SET content = '" + value + "',type_comment = 1 ";
                sql += " where  id = " + idcomment;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();
                res = Request.CreateResponse(HttpStatusCode.OK, i);
                return res;
            }
            catch (Exception e)
            {
                res = Request.CreateResponse(HttpStatusCode.BadRequest, e);
                return res;
            }
        }

        // DELETE api/values/5
        public HttpResponseMessage Delete(int idcomment)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            try
            {
                string sql = "";
                sql += " ";
                sql += " update t_comment t0 ";
                sql += " join (select t11.id,t11.content, getpath(t11.id) AS path  ";
                sql += " FROM t_comment t11  ";
                sql += " HAVING (path like '%/" + idcomment + "/%' or path like '%/" + idcomment + "' or path like '" + idcomment + "/%')) t1 ";
                sql += " on t0.id = t1.id ";
                sql += " set type_comment =2";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();
                res = Request.CreateResponse(HttpStatusCode.OK, i);
                return res;
            }
            catch (Exception e)
            {
                res = Request.CreateResponse(HttpStatusCode.BadRequest, e);
                return res;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }
        }
    }
}
