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
    public class CompanyController : ApiController
    {
        // GET api/values
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            List<Company> list_com = new List<Company>();
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            string sql = "";
            try
            {
                sql = " SELECT id,name,urlavatar,content,name_re,name_en," +
                    "case when user_update is null then user_creat else user_update end as user," +
                    "case when updatetime is null then creattime else updatetime end as time " +
                    " from t_company t0" +
                " where status = 0 ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Company ac = new Company();
                    ac.id = Convert.ToInt32(r["id"].ToString());
                    ac.name = r["name"].ToString();
                    ac.name_re = r["name_re"].ToString();
                    ac.name_en = r["name_en"].ToString();
                    ac.urlavatar = r["urlavatar"].ToString();
                    ac.content = r["content"].ToString();
                    if (r["user"].ToString() != "")
                        ac.user_creat = Convert.ToInt32(r["user"].ToString());
                    if (r["time"].ToString() != "")
                        ac.creattime = Convert.ToDateTime(r["time"].ToString());
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
            var filepath = HttpContext.Current.Server.MapPath("~/Img/company_avatar/" + imagename);
            postfile.SaveAs(filepath);
            int id_user;
            id_user = Convert.ToInt32(httpRequest["id_company"]);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = " ";
                string name = "http://localhost:49696/Img/company_avatar/" + imagename;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                sql = "update t_company set urlavatar=N'" + MySqlHelper.EscapeString(name) + "' where id=" + id_user + " ";
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

        // POST api/values?creattag=
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Company com)
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
            if (level >= 7)
            {
                try
                {
                    sql = "select status,id from t_company where status=0 and name='" + com.name + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        if (ds.Tables[0].Rows[0]["status"].ToString() == "1")
                        {
                            sql = " update t_company set content=N'"+MySqlHelper.EscapeString(com.content)+"',status=0,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                                "where id='" + id + "' ";
                            cmd = new MySqlCommand(sql, conn);
                            int i = cmd.ExecuteNonQuery();
                            com.id = id;
                            com.creattime = DateTime.Now;
                            com.user_creat = id_user;
                            res = Request.CreateResponse(HttpStatusCode.OK, com);
                        }
                        else res = Request.CreateResponse(HttpStatusCode.NotModified);
                    }
                    else
                    {
                        sql = "INSERT INTO t_company (name, name_re, name_en,  content, creattime, user_creat) " +
                            "VALUES ( N'"+com.name+"', N'"+com.name_re+"', N'"+com.name_en+"', N'"+ MySqlHelper.EscapeString(com.content) + "', CURRENT_DATE(), '"+id_user+"'); ";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();

                        sql = "select id from t_company where name= '" + com.name + "'";
                        cmd = new MySqlCommand(sql, conn);
                        adap = new MySqlDataAdapter(cmd);
                        ds = new DataSet();
                        adap.Fill(ds);
                        com.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        com.creattime = DateTime.Now;
                        com.user_creat = id_user;

                        res = Request.CreateResponse(HttpStatusCode.OK, com);
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
        public HttpResponseMessage Put(int id, [FromBody] Company com)
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
            if (level >= 7)
            {
                try
                {
                    sql = "select status from t_company where status=0 and name='" + com.name + "' and id <> '" + id + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return res = Request.CreateResponse(HttpStatusCode.NotModified, com);
                    }
                    else
                    {
                        sql = " update t_company set name=N'" + com.name + "',name_re=N'"+com.name_re+"',name_en=N'"+com.name_en+"',content=N'"+ MySqlHelper.EscapeString(com.content)+"'" +
                            "status=0,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                               " where id='" + id + "' ";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();
                        com.creattime = DateTime.Now;
                        com.user_creat = id_user;
                        res = Request.CreateResponse(HttpStatusCode.OK, com);
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
            sql = "select level from t_user where id ='" + id_user + "' ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            int level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
            if (level >= 7)
            {
                try
                {
                    sql = " update t_company set status=1,updatetime='" + DateTime.Now.ToString("yyyy/MM/dd") + "',user_update='" + id_user + "'" +
                            "where id='" + id + "' ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                    res = Request.CreateResponse(HttpStatusCode.OK, "yes");
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
