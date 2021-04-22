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

namespace XemphimAPI.Controllers
{
    public class TagController : ApiController
    {   
        // GET api/values
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            List<Tag> list_tag = new List<Tag>();
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            string sql = "";
            try
            {
                sql = " SELECT id,name,sl_movie,case when updatetime is null then creattime else updatetime end as time " +
                    " from t_tag t0" +
                " where status = 0 ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Tag ac = new Tag();
                    ac.id = Convert.ToInt32(r["id"].ToString());
                    ac.name = r["name"].ToString();
                    if (r["sl_movie"].ToString() != "")
                        ac.sl_movie = Convert.ToInt32(r["sl_movie"].ToString());
                    if (r["time"].ToString() != "")
                        ac.creattime = Convert.ToDateTime(r["time"].ToString());
                    list_tag.Add(ac);
                }
                json = JsonConvert.SerializeObject(list_tag);
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

        // POST api/values?creattag=
        [HttpPost]
        public HttpResponseMessage Post(string creattag)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            string sql = "";
            sql = "select level from t_user where id ='" + id_user + "' ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            int level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
            Tag t = new Tag();
            if (level >= 3)
            {
                try
                {
                    sql = "select status,id from t_tag where name='" + creattag + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        if (ds.Tables[0].Rows[0]["status"].ToString() == "1")
                        {
                            sql = " update t_tag set status=0,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                                "where id='" + id + "' ";
                            cmd = new MySqlCommand(sql, conn);
                            int i = cmd.ExecuteNonQuery();
                            t.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                            t.name = creattag;
                            t.creattime = DateTime.Now;
                            res = Request.CreateResponse(HttpStatusCode.OK, t);
                        }
                        else res = Request.CreateResponse(HttpStatusCode.NotModified);
                    }
                    else
                    {
                        sql = "INSERT INTO t_tag ( name, name_re, sl_movie, creattime, user_creat,  status) " +
                            "VALUES ( N'" + creattag + "', N'" + creattag + "', '', CURRENT_DATE(), '" + id_user + "', '0') ";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();

                        sql = "select id from t_tag where name= '" + creattag + "'";
                        cmd = new MySqlCommand(sql, conn);
                        adap = new MySqlDataAdapter(cmd);
                        ds = new DataSet();
                        adap.Fill(ds);
                        t.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        t.name = creattag;
                        t.creattime = DateTime.Now;

                        res = Request.CreateResponse(HttpStatusCode.OK, t);
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

        // PUT api/values/5
        [HttpPut]
        public HttpResponseMessage Put(int id, string updatetag)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            string sql = "";
            sql = "select t0.level from t_user t0 join t_tag t1 on t0.id = t1.user_creat or t0.id=t1.user_update or t0.level >= 7" +
                " where t0.id ='" + id_user + "' and t1.id = '"+id+"'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            Tag t = new Tag();
            if (ds.Tables[0].Rows.Count > 0)
            {
                try
                {
                    sql = "select status from t_tag where status=0 and name='" + updatetag + "' and id <> '" + id + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return res = Request.CreateResponse(HttpStatusCode.NotModified, updatetag);
                    }
                    else
                    {
                        sql = " update t_tag set name='"+ updatetag + "',status=0,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                               " where id='" + id + "' ";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();
                        t.id = id;
                        t.name = updatetag;
                        t.creattime = DateTime.Now;
                        res = Request.CreateResponse(HttpStatusCode.OK, t);
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
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            string sql = "";
            sql = "select t0.level from t_user t0 join t_tag t1 on t0.id = t1.user_creat or t0.id=t1.user_update or t0.level >= 7" +
                " where t0.id ='" + id_user + "' and t1.id = '" + id + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                try
                {
                    sql = " update t_tag set status=1,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                            "where id='" + id + "' ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                    res = Request.CreateResponse(HttpStatusCode.OK,"yes");
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
    }
}
