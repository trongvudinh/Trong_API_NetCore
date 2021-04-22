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
using System.Web;
using System.Net.Http.Headers;
using System.Web.Http.Cors;
using XemphimAPI.Controllers;
using XemphimAPI.Filter;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;

namespace XemphimAPI.Controllers
{
    public class SeriesController : ApiController
    {
        // GET api/values
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            List<Series> list_com = new List<Series>();
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            string sql = "";
            try
            {
                sql = " SELECT t0.id,t0.name,t0.count_movie,t0.urlavatar,year_str,year_end,t0.content,t0.warning,t0.id_company,t1.name com_name," +
                    "case when t0.user_update is null then t0.user_creat else t0.user_update end as user," +
                    "case when t0.updatetime is null then t0.creattime else t0.updatetime end as time " +
                    " from t_series t0 left join t_company t1 on t0.id_company=t1.id" +
                " where t0.status = 0 ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Series ac = new Series();
                    ac.id = Convert.ToInt32(r["id"].ToString());
                    ac.name = r["name"].ToString();
                    ac.count_movie = Convert.ToInt32(r["count_movie"].ToString());
                    ac.urlavatar = r["urlavatar"].ToString();
                    ac.content = r["content"].ToString();
                    if (r["year_str"].ToString() != "")
                        ac.year_str = Convert.ToInt32(r["year_str"].ToString());
                    if (r["year_end"].ToString() != "")
                        ac.year_end = Convert.ToInt32(r["year_end"].ToString());
                    if (r["warning"].ToString() != "")
                        ac.warning = Convert.ToInt32(r["warning"].ToString());

                    Company c = new Company();
                    if (r["id_company"].ToString() != "")
                        c.id = Convert.ToInt32(r["id_company"].ToString());
                    c.name = r["com_name"].ToString();
                    ac.company = c;


                    if (r["user"].ToString() != "")
                        ac.user_creat = Convert.ToInt32(r["user"].ToString());
                    if (r["time"].ToString() != "")
                        ac.creattime = Convert.ToDateTime(r["time"].ToString());
                    
                    //---------------------------list-tag----------------------------------------------
                    List<Tag> list_tag = new List<Tag>();
                    sql = " select t1.id,t1.name from t_tag1 t0 left join t_tag t1 on t0.id_tag = t1.id and t0.type = 1 where id_movie='" + ac.id + "' ";


                    MySqlCommand cmd2 = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adap2 = new MySqlDataAdapter(cmd2);
                    DataSet ds2 = new DataSet();
                    adap2.Fill(ds2);
                    foreach(DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Tag tag_temp = new Tag();
                        if (r2["id"].ToString() != "")
                            tag_temp.id = Convert.ToInt32(r2["id"].ToString());
                        tag_temp.name = r2["name"].ToString();
                        list_tag.Add(tag_temp);
                    }
                    ac.list_tag = list_tag.ToArray();

                    //---------------------------list-cata----------------------------------------------
                    List<Catalog> list_cata = new List<Catalog>();
                    sql = " select t1.id,t1.name from t_mov_cata t0 left join t_catalog t1 on t0.id_catalog = t1.id and t0.type = 1 where id_mov='" + ac.id + "' ";


                    cmd2 = new MySqlCommand(sql, conn);
                    adap2 = new MySqlDataAdapter(cmd2);
                    ds2 = new DataSet();
                    adap2.Fill(ds2);
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Catalog cata_temp = new Catalog();
                        if (r2["id"].ToString() != "")
                            cata_temp.id = Convert.ToInt32(r2["id"].ToString());
                        cata_temp.name = r2["name"].ToString();
                        list_cata.Add(cata_temp);
                    }
                    ac.list_cata = list_cata.ToArray();

                    //---------------------------list-actor----------------------------------------------
                    List<Actor> list_actor = new List<Actor>();
                    sql = " select t1.id,t1.name from t_mov_actor t0 left join t_actor t1 on t0.id_actor = t1.id and t0.type_mov = 1 where id_mov='"+ac.id+"' ";


                    cmd2 = new MySqlCommand(sql, conn);
                    adap2 = new MySqlDataAdapter(cmd2);
                    ds2 = new DataSet();
                    adap2.Fill(ds2);
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Actor actor_temp = new Actor();
                        if(r2["id"].ToString()!="")
                            actor_temp.id = Convert.ToInt32(r2["id"].ToString());
                        actor_temp.name = r2["name"].ToString();
                        list_actor.Add(actor_temp);
                    }
                    ac.list_actor = list_actor.ToArray();


                    list_com.Add(ac);
                }
                json = JsonConvert.SerializeObject(list_com);
                res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            }
            catch (Exception e)
            {
                res = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }
            return res;
        }
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [AllowAnonymous]
        [HttpPost]
        //POST api/Actor?uploadfile
        public HttpResponseMessage uploadImage(int uploadfile)
        {

            string imagename = "";
            var httpRequest = HttpContext.Current.Request;

            var postfile = httpRequest.Files["Img"];
            imagename = new String(Path.GetFileNameWithoutExtension(postfile.FileName).Take(10).ToArray()).Replace(" ", "-");
            imagename += DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postfile.FileName);
            var filepath = HttpContext.Current.Server.MapPath("~/Img/series_avatar/" + imagename);
            postfile.SaveAs(filepath);
            int id_user;
            id_user = Convert.ToInt32(httpRequest["id_series"]);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = " ";
                string name = "http://localhost:49696/Img/series_avatar/" + imagename;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                sql = "update t_series  set urlavatar=N'" + MySqlHelper.EscapeString(name) + "' where id=" + id_user + " ";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                res = Request.CreateResponse(HttpStatusCode.Created, name);
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

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Series com)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            int cc = 0;
            string sql = "";
            sql = "select level from t_user where id ='" + id_user + "' ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            int warning = 0;
            int level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
            if (level >= 3)
            {
                if (level >= 7) warning = com.warning;
                else warning = 0;
                try
                {
                    sql = "select status,id from t_series where status=0 and name='" + com.name + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        if (ds.Tables[0].Rows[0]["status"].ToString() == "1")
                        {
                            sql = " update t_series set content=N'" + MySqlHelper.EscapeString(com.content) + "',year_str='"+com.year_str+"',year_end='"+com.year_end+"'" +
                                ",warning='"+warning+"',id_company='"+com.company.id+"'" +
                                ",status=0,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                                "where id='" + id + "' ";
                            cmd = new MySqlCommand(sql, conn);
                            int i = cmd.ExecuteNonQuery();
                            com.id = id;
                            com.creattime = DateTime.Now;
                            com.user_creat = id_user;
                            res = Request.CreateResponse(HttpStatusCode.OK, com);
                            cc = 1;
                        }
                        else res = Request.CreateResponse(HttpStatusCode.NotModified);
                    }
                    else
                    {
                        sql = "INSERT INTO t_series ( name, count_movie,  year_str, year_end, content, content_re, warning, id_company, creattime, user_creat,status) " +
                            "VALUES (N'"+com.name+"', '0', '"+com.year_str+"', "+com.year_end+", N'"+com.content+"', '', '"+warning+"', '"+com.company.id+"', CURRENT_DATE(), '15','0');";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();

                        sql = "select id from t_series where name= '" + com.name + "'";
                        cmd = new MySqlCommand(sql, conn);
                        adap = new MySqlDataAdapter(cmd);
                        ds = new DataSet();
                        adap.Fill(ds);
                        com.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        com.creattime = DateTime.Now;
                        com.user_creat = id_user;

                        res = Request.CreateResponse(HttpStatusCode.OK, com);
                        cc = 1;
                    }
                }

                catch (Exception e)
                {
                    res = Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (cc == 1)
                {
                    UsingFunction.update_list_tag(com.id, com.list_tag.ToList(), 1, id_user);
                    UsingFunction.update_list_catalog(com.id, com.list_cata.ToList(), 1,id_user);
                    UsingFunction.update_list_actor(com.id, com.list_actor.ToList(), 1, id_user);
                    if (com.warning != 0 && level < 7)
                    {
                        MySqlCommand cmd2 = conn.CreateCommand();
                        MySqlTransaction myTrans;
                        myTrans = conn.BeginTransaction();
                        cmd2.Connection = conn;
                        cmd2.Transaction = myTrans;
                        try
                        {
                            sql = "SELECT AUTO_INCREMENT s FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'xemphim' AND TABLE_NAME = 't_approved' ";
                            cmd2 = new MySqlCommand(sql, conn);
                            adap = new MySqlDataAdapter(cmd2);
                            ds = new DataSet();
                            adap.Fill(ds);
                            int id_app = Convert.ToInt32(ds.Tables[0].Rows[0]["s"].ToString());
                            sql = "INSERT INTO t_approved (id, name, content, user_creat, createtime,  type, status) " +
                                "VALUES ('" + id_app + "', N'Phê duyệt warning', '', '" + id_user + "', CURRENT_TIME(),'0', '0');";
                            cmd2 = new MySqlCommand(sql, conn);
                            int i = cmd2.ExecuteNonQuery();

                            sql = "INSERT INTO t_approved_value ( id_app, table_name, filed_name,key_id, value) VALUES ( '" + id_app + "', 't_series', 'warning','" + com.id + "', '" + com.warning + "');";
                            cmd2 = new MySqlCommand(sql, conn);
                            i = cmd2.ExecuteNonQuery();

                            myTrans.Commit();
                        }
                        catch (Exception e)
                        {
                            myTrans.Rollback();
                        }
                    }           
                }
            }
            else
                res = Request.CreateResponse(HttpStatusCode.Unauthorized);
            return res;
        }

        // PUT api/values/5
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody] Series com)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            string sql = "select t0.level from t_user t0 join t_series t1 on t0.id = t1.user_creat or t0.id=t1.user_update or t0.level >= 7" +
                " where t0.id ='" + id_user + "' and t1.id = '" + id + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            Tag t = new Tag();           
            if (ds.Tables[0].Rows.Count > 0)
            {
                int level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
                try
                {
                    int cc = 0;
                    sql = "select status,warning,id from t_series where status=0 and name='" + com.name + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    int warning = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if ((Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString()) != id))
                            return res = Request.CreateResponse(HttpStatusCode.NotModified, com);
                        else cc = 1;
                        warning = Convert.ToInt32(ds.Tables[0].Rows[0]["warning"].ToString());
                    }
                    else cc = 1;
                    if (cc==1)
                    {

                        UsingFunction.update_list_tag(com.id, com.list_tag.ToList(), 1, id_user);
                        UsingFunction.update_list_catalog(com.id, com.list_cata.ToList(), 1, id_user);
                        UsingFunction.update_list_actor(com.id, com.list_actor.ToList(), 1, id_user);
                        if (level >= 7) warning = com.warning;

                        sql = " update t_series set name=N'"+com.name+"',content=N'" + MySqlHelper.EscapeString(com.content) + "',count_movie = 0,year_str='" + com.year_str + "',year_end='" + com.year_end + "'" +
                                ",warning='" + warning + "',id_company='" + com.company.id + "'" +
                                ",status=0,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                                "where id='" + id + "' ";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();
                        com.creattime = DateTime.Now;
                        com.user_creat = id_user;
                        res = Request.CreateResponse(HttpStatusCode.OK, com);
                        if (warning != com.warning)
                        {
                            MySqlCommand cmd2 = conn.CreateCommand();
                            MySqlTransaction myTrans;
                            myTrans = conn.BeginTransaction();
                            cmd2.Connection = conn;
                            cmd2.Transaction = myTrans;
                            try
                            {
                                sql = "SELECT AUTO_INCREMENT s FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'xemphim' AND TABLE_NAME = 't_approved' ";
                                cmd2 = new MySqlCommand(sql, conn);
                                adap = new MySqlDataAdapter(cmd2);
                                ds = new DataSet();
                                adap.Fill(ds);
                                int id_app = Convert.ToInt32(ds.Tables[0].Rows[0]["s"].ToString());
                                sql = "INSERT INTO t_approved (id, name, content, user_creat, createtime,  type, status) " +
                                    "VALUES ('" + id_app + "', N'Phê duyệt warning', '', '" + id_user + "', CURRENT_TIME(),'0', '0');";
                                cmd2 = new MySqlCommand(sql, conn);
                                i = cmd2.ExecuteNonQuery();

                                sql = "INSERT INTO t_approved_value ( id_app, table_name, filed_name,key_id, value) VALUES ( '" + id_app + "', 't_series', 'warning','" + com.id + "', '" + com.warning + "');";
                                cmd2 = new MySqlCommand(sql, conn);
                                i = cmd2.ExecuteNonQuery();

                                myTrans.Commit();
                            }
                            catch (Exception e)
                            {
                                myTrans.Rollback();
                            }
                        }                        
                    }
                }
                catch (Exception e)
                {
                    res = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
                res = Request.CreateResponse(HttpStatusCode.Unauthorized);
            return res;
        }


        // DELETE api/values/5
        public HttpResponseMessage Delete(int id)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            string sql = "select t0.level from t_user t0 join t_series t1 on t0.id = t1.user_creat or t0.id=t1.user_update or t0.level >= 7" +
                " where t0.id ='" + id_user + "' and t1.id = '" + id + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            Tag t = new Tag();
            if (ds.Tables[0].Rows.Count > 0)
            {
                try
                {
                    sql = " update t_series set status=1,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                               "where id='" + id + "' ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                    res = Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    res = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                 finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Clone();
                    }
                }
               
            }
            return res;
        }
    }
}
