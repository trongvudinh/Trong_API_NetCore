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
    [BasicAuthentication]
    public class ActorController : ApiController
    {
        // GET api/values
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            List<Actor> list_actor = new List<Actor>();
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            string sql = "";
            try
            {
                sql = " SELECT id,name,birtday,content,urlavatar,famail " +
                    " from t_actor t0" +
                " where status = 0 and type = 0" ;

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    Actor ac = new Actor();
                    if (r["id"].ToString() != "")
                        ac.id = Convert.ToInt32(r["id"].ToString());
                    ac.name = r["name"].ToString();
                    if (r["birtday"].ToString() != "")
                        ac.birtday = Convert.ToDateTime(r["birtday"].ToString());
                    ac.urlavatar = r["urlavatar"].ToString();
                    if (r["famail"].ToString() != "")
                        ac.famail = Convert.ToInt32(r["famail"].ToString());
                    ac.type = 0;
                    list_actor.Add(ac);
                }
                json = JsonConvert.SerializeObject(list_actor);
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

        // GET api/values?findname=
        [AllowAnonymous]
        public HttpResponseMessage Get_us(string findname)
        {
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            string sql = "";
            try
            {
                sql = " SELECT id,name,birtday,content,urlavatar,famail " +
                    " from t_actor t0" +
                " where status = 0  and t0.name = '"+findname+"'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                if(ds.Tables[0].Rows.Count >0 )
                    res= Request.CreateResponse(HttpStatusCode.OK,"yes");
                else res = Request.CreateResponse(HttpStatusCode.OK, "no");
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
        [HttpPost]
        //POST api/Actor?uploadfile
        public HttpResponseMessage uploadImage(int uploadfile)
        {

            string imagename = "";
            var httpRequest = HttpContext.Current.Request;

            var postfile = httpRequest.Files["Img"];
            imagename = new String(Path.GetFileNameWithoutExtension(postfile.FileName).Take(10).ToArray()).Replace(" ", "-");
            imagename += DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postfile.FileName);
            var filepath = HttpContext.Current.Server.MapPath("~/Img/actor_avatar/" + imagename);
            postfile.SaveAs(filepath);
            int id_user;
            id_user = Convert.ToInt32(httpRequest["id_actor"]);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = " ";
                string name = "http://localhost:49696/Img/actor_avatar/" + imagename;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                sql = "update t_actor set urlavatar=N'" + MySqlHelper.EscapeString(name) + "' where id=" + id_user + " ";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                res = Request.CreateResponse(HttpStatusCode.Created);
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

        // POST api/values
        public HttpResponseMessage Post([FromBody]Actor ac)          
        {
            string json = "";
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
            if (level >= 3)
            {
                try
                {
                    sql = "INSERT INTO t_actor (name, birtday, content, famail, type) " +
                        "VALUES (N'"+ac.name+"', '"+ac.birtday.ToString("yyyy/MM/dd") +"', N'"+ac.content+"', '"+ac.famail+"', '"+ac.type+"'); ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();

                    sql = "select id from t_actor where name= '" + ac.name + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    ac.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());

                    res = Request.CreateResponse(HttpStatusCode.OK, ac);
                }
                catch (MySqlException e)
                {
                    if (e.Number == 1062)
                    {
                        int id;
                        sql = "select status,id from t_actor where name='" + ac.name + "'";
                        cmd = new MySqlCommand(sql, conn);
                        adap = new MySqlDataAdapter(cmd);
                        ds = new DataSet();
                        adap.Fill(ds);
                        id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["status"].ToString()) == 1)
                        {
                            sql = " update t_actor set status=0,birtday='"+ ac.birtday.ToString("yyyy/MM/dd") + "',content=N'"+ac.content+"',famail='"+ac.famail+"',type='"+ac.type+"' " +
                                "where id='" + id + "' ";
                            cmd = new MySqlCommand(sql, conn);
                            int i = cmd.ExecuteNonQuery();

                            res = Request.CreateResponse(HttpStatusCode.OK, ac);
                        }
                        else res = Request.CreateResponse(HttpStatusCode.NotModified);
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
        public HttpResponseMessage Put(int id, [FromBody]Actor ac)
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
            if (level >= 3)
            {
                try
                {
                    sql = "select status from t_actor where status=0 and name='"+ac.name+"' and id <> '" + id + "'";
                    cmd = new MySqlCommand(sql, conn);
                    adap = new MySqlDataAdapter(cmd);
                    ds = new DataSet();
                    adap.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return res= Request.CreateResponse(HttpStatusCode.NotModified, ac);
                    }
                    else
                    {
                        sql = "update t_actor set name=N'"+ac.name+"', birtday ='"+ ac.birtday.ToString("yyyy/MM/dd") + "', content='"+ac.content+"', " +
                            "famail='"+ac.famail+"', type='"+ac.type+"',status='0' " +
                            "where id='" + id + "' ";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();
                        res = Request.CreateResponse(HttpStatusCode.OK, ac);
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
        public void Delete(int id)
        {
        }
    }
}
