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
    public class UserController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public HttpResponseMessage Get_manager_user(int manager)
        {
            string json = "";
            int level;
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            string sql = "";
            sql = "select level from t_user where id ='" + id_user + "' ";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
            try
            {
                sql = "  SELECT id,name,level,email" +
                    " from t_user t0 where (level+2<="+level+" or "+level+"=7)";
                List<Manager_User> list_user = new List<Manager_User>();
                cmd = new MySqlCommand(sql, conn);
                adap = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adap.Fill(ds);
                Manager_User us = new Manager_User();
                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    us = new Manager_User();
                    us.level_manager = level;
                    us.username = r["name"].ToString();
                    us.email = r["email"].ToString();
                    us.id = Convert.ToInt32(r["id"].ToString());
                    us.level = Convert.ToInt32(r["level"].ToString());
                    us.listmac = new List<string>();
                    us.listtime = new List<string>();
                    sql = " select HOUR(TIMEDIFF(leavetime, createtime)) h,MINUTE(TIMEDIFF(leavetime, createtime)) m,SECOND(TIMEDIFF(leavetime, createtime)) s  " +
                        "from t_user_time where id_user="+us.id+ " order by createtime DESC ";
                    if (level < 4) { sql += " LIMIT 20"; }
                    MySqlCommand cmd2 = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adap2 = new MySqlDataAdapter(cmd2);
                    DataSet ds2 = new DataSet();
                    adap2.Fill(ds2);
                    long totaltime = 0;
                    int i = 0;
                    foreach(DataRow r2 in ds2.Tables[0].Rows)
                    {
                        if (i<20) us.listtime.Add(r2["h"].ToString() + ":" + r2["m"].ToString() + ":" + r2["s"].ToString());
                        totaltime += Convert.ToInt32(r2["h"].ToString()) * 3600 + Convert.ToInt32(r2["m"].ToString()) * 60 + Convert.ToInt32(r2["s"].ToString());
                        i++;
                    }
                    us.totaltime = ((int)(totaltime / 3600)).ToString() + ":" + ((int)(totaltime / 60)).ToString();
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                    sql = " select DISTINCT ifnull(macadress,' ') mac from t_token where id_user = '"+us.id+ "' LIMIT 20 ";
                    cmd2 = new MySqlCommand(sql, conn);
                    adap2 = new MySqlDataAdapter(cmd2);
                    ds2 = new DataSet();
                    adap2.Fill(ds2);
                    i = 0;
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        if (i < 20) us.listmac.Add(r2["mac"].ToString()+" " );
                        i++;
                    }
                    list_user.Add(us);
                }

                json = JsonConvert.SerializeObject(list_user);
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
        public HttpResponseMessage Get_user(int iduser)
        {
            string json = "";
            User us = new User();
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            string sql = "";
            try
            {
                sql = " SELECT id,name,pass,level,urlavatar,count_video,email,bri_day,bri_month,bri_year,thanhpho,diachi,hoten,gioitinh,sdt,nghenghiep,sothich  " +
                    " from t_user t0" +
                " where id=" + iduser;

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    us.name = ds.Tables[0].Rows[0]["name"].ToString();
                    us.pass = ds.Tables[0].Rows[0]["pass"].ToString();
                    us.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    us.level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
                    us.urlavatar = ds.Tables[0].Rows[0]["urlavatar"].ToString();
                    us.count_video = Convert.ToInt32(ds.Tables[0].Rows[0]["count_video"].ToString());
                    us.email = ds.Tables[0].Rows[0]["email"].ToString();
                    if(ds.Tables[0].Rows[0]["bri_day"].ToString()!="")
                        us.bri_day = Convert.ToInt32(ds.Tables[0].Rows[0]["bri_day"].ToString());
                    if (ds.Tables[0].Rows[0]["bri_month"].ToString() != "")
                        us.bri_month = Convert.ToInt32(ds.Tables[0].Rows[0]["bri_month"].ToString());
                    if (ds.Tables[0].Rows[0]["bri_year"].ToString() != "")
                        us.bri_year = Convert.ToInt32(ds.Tables[0].Rows[0]["bri_year"].ToString());
                    us.thanhpho = ds.Tables[0].Rows[0]["thanhpho"].ToString();
                    us.diachi = ds.Tables[0].Rows[0]["diachi"].ToString();
                    us.hoten = ds.Tables[0].Rows[0]["hoten"].ToString();
                    if (ds.Tables[0].Rows[0]["gioitinh"].ToString() != "")
                        us.gioitinh = Convert.ToInt32(ds.Tables[0].Rows[0]["gioitinh"].ToString());
                    us.sdt = ds.Tables[0].Rows[0]["sdt"].ToString();
                    us.nghenghiep = ds.Tables[0].Rows[0]["nghenghiep"].ToString();
                    us.sothich = ds.Tables[0].Rows[0]["sothich"].ToString();
                }

                json = JsonConvert.SerializeObject(us);
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

        //GET api/user?findusername=
        [AllowAnonymous]
        public HttpResponseMessage Get_username(string findusername)
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            string sql = "";
            try
            {
                sql = " SELECT id,name,level,urlavatar,count_video  from t_user t0" +
                " where name='" + findusername+"'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    res = Request.CreateResponse(HttpStatusCode.OK,"yes");
                }
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

        [AllowAnonymous]
        public HttpResponseMessage Post_login([FromBody]Token token, string username, string pass)
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            User_token c_user = new User_token();
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            string sql = "";
            sql = " SELECT id,level,urlavatar,count_video  from t_user t0" +
                " where name='" + username + "' and pass='" + pass + "'";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);

            if (ds.Tables[0].Rows.Count == 1)
            {
                User us = new User();
                us.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                us.level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
                us.urlavatar = ds.Tables[0].Rows[0]["urlavatar"].ToString();
                us.count_video = Convert.ToInt32(ds.Tables[0].Rows[0]["count_video"].ToString());
                c_user.user = us;

                long re = UsingFunction.creat_token(c_user.user.id, token);
                if (re == -1)
                {
                    res = Request.CreateResponse(HttpStatusCode.BadRequest);
                    c_user.status = -1;
                    c_user.content = "BadRequest";
                }
                else
                {
                    c_user.token = re;
                    c_user.status = 0;
                    c_user.content = "";
                    json = JsonConvert.SerializeObject(c_user);
                    res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                }
                return res;

            }
            else
            {
                c_user.status = 1;
                c_user.content = "Tài khoản hoặc mật khẩu không đúng";
                json = JsonConvert.SerializeObject(c_user);
                res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            }

            return res;
        }
        // POST api/values
        [AllowAnonymous]
        public HttpResponseMessage Post_token([FromBody]Token token, int gettoken ,int iduser)
        {
            var res = Request.CreateResponse(HttpStatusCode.OK);
            RETURN_TOKEN xx = new RETURN_TOKEN();
            if (gettoken == 1) { return res = Request.CreateResponse(HttpStatusCode.BadRequest); }
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            bool isgetotken = false;
            string sql = "";
            MySqlCommand cmd = new MySqlCommand();
            if (iduser>0 && token.id>0)
            {
                sql = "select *  from t_user_temp where  datediff(CURRENT_DATE,createtime)<=7 and id= '" + iduser + "' and token='" + token.id + "'";
                cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    xx.token = token.id;xx.t_user = iduser;
                    return res= Request.CreateResponse(HttpStatusCode.OK,xx); ;
                };
            }
            long re= UsingFunction.creat_token(0,token);
            if (re == -1)
            {
                res = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                while (!isgetotken)
                {
                    try
                    {
                        sql = "";
                        sql = " SELECT AUTO_INCREMENT as id FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'xemphim' AND TABLE_NAME = 't_user_temp'";

                        cmd = new MySqlCommand(sql, conn);
                        MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adap.Fill(ds);
                        long id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        string name_te = UsingFunction.randomname();
                        string url_te = UsingFunction.random_urlavatar();

                        sql = " insert into t_user_temp(id,name,createtime,urlavatar,token) values("+id+",'" + name_te + "',NOW(),'" + url_te + "'," + re + ")";
                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();

                        xx.token = re;
                        xx.t_user = id;
                        isgetotken = true;
                    }
                    catch (MySqlException e)
                    {
                        if (e.Number != 1062)
                        {
                            isgetotken = true;
                        }
                    }
                    catch (Exception e)
                    {
                        isgetotken = true;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Clone();
                        }
                    }
                }

                res = Request.CreateResponse(HttpStatusCode.OK, xx);
            }
            return res;
        }
        [AllowAnonymous]
        [HttpPost]
        //POST api/user?uploadfile
        public HttpResponseMessage uploadImage(int uploadfile)
        {

            string imagename = "";
            var httpRequest = HttpContext.Current.Request;

            var postfile = httpRequest.Files["Img"];
            imagename = new String(Path.GetFileNameWithoutExtension(postfile.FileName).Take(10).ToArray()).Replace(" ", "-");
            imagename += DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postfile.FileName);
            var filepath = HttpContext.Current.Server.MapPath("~/Img/user_avatar/" + imagename);
            postfile.SaveAs(filepath);
            int id_user;
            id_user = Convert.ToInt32(httpRequest["id_user"]);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = " ";
                string name = "http://localhost:49696/Img/user_avatar/" + imagename;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                sql = "update t_user set urlavatar=N'"+ MySqlHelper.EscapeString(name) + "' where id="+id_user+" ";
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
            //List<string> savefilePath = new List<string>();
            //if (!Request.Content.IsMimeMultipartContent())
            //{
            //    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            //}
            //string rootPath = HttpContext.Current.Server.MapPath("~/Img");
            //var provider = new MultipartFileStreamProvider(rootPath);

        }
        //POST api/user?adduser
        [AllowAnonymous]
        public HttpResponseMessage Post_user([FromBody] User u,int adduser)
        {
            string json = "";
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            try
            {
                string sql = " SELECT id,name,level,urlavatar,count_video  from t_user t0" +
                " where name='" + u.name + "'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    return res = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                sql = "";
                sql= "INSERT INTO t_user ( name, pass, level, " +
                    "count_video, urlavatar, email, bri_day, " +
                    "bri_month, bri_year, thanhpho, diachi," +
                    " hoten, gioitinh, sdt, nghenghiep, sothich) VALUES " +
                       "(N'"+u.name+"', N'"+u.pass+"', '0', " +
                       "'0', N'"+u.urlavatar+"', N'"+u.email+"', "+u.bri_day+", " +
                       u.bri_month+", '"+u.bri_year+"', N'"+u.thanhpho+"', N'"+u.diachi+"', " +
                       "N'"+u.hoten+"', '"+u.gioitinh+"', '"+u.sdt+"', ' ', N'"+u.sothich+"');";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                sql = "select id from t_user where name= '" + u.name + "'";
                cmd = new MySqlCommand(sql, conn);
                adap = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adap.Fill(ds);
                u.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                sql = "insert into t_token(id,id_user) values(0,"+ u.id + ")";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                json = JsonConvert.SerializeObject(u);
                res = Request.CreateResponse(HttpStatusCode.Created);
                res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
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
        [AllowAnonymous]
        // POST api/values?id_user=&type_user=
        public HttpResponseMessage Post_time(int id_user, int type_user)
        {
            string json = "";
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            bool isgetotken = false;
            string sql = "";
            MySqlCommand cmd = new MySqlCommand();
            long id=0;
            while (!isgetotken)
            {
                try
                {
                    sql = "";
                    sql = " SELECT AUTO_INCREMENT as id FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'xemphim' AND TABLE_NAME = 't_user_time'";

                    cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adap.Fill(ds);
                    id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());

                    sql = "INSERT INTO `T_USER_TIME` (id,createtime, leavetime, id_user, type_user) VALUES " +
                        " ('"+id+"',CURRENT_TIME(), CURRENT_TIME(), '" + id_user + "', '" + type_user + "');";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                    isgetotken = true;
                }
                catch (MySqlException e)
                {
                    if (e.Number != 1062)
                    {
                        isgetotken = true;
                    }
                }
                catch (Exception e)
                {
                    isgetotken = true;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Clone();
                    }
                }
            }
            res= Request.CreateResponse(HttpStatusCode.OK,id);
            return res;
        }

        [AllowAnonymous]
        // PUT api/values?id_user_time=
        public HttpResponseMessage Put_time(int id_user_time)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            string sql = "";
            MySqlCommand cmd = new MySqlCommand();
            try
            { 

                sql = "UPDATE `T_USER_TIME`SET leavetime=CURRENT_TIME() where id='"+ id_user_time + "' ";
                cmd = new MySqlCommand(sql, conn);
                int i = cmd.ExecuteNonQuery();
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
            res = Request.CreateResponse(HttpStatusCode.OK);
            return res;
        }

        // PUT api/values?update_user=&id=
        public HttpResponseMessage Put(int update_user, [FromBody]User key)
        {
            string json = "";
            int id_user = Convert.ToInt32(Thread.CurrentPrincipal.Identity.Name);
            if (id_user != key.id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            conn.Open();
            try
            {
                string sql = "";
                sql = "UPDATE t_user set ";
                string s = "";
                if (key.pass != "")         s += ", pass=N'" + key.pass + "'";
                if (key.level != -1)      s += ", level='" + key.level + "'";
                if (key.count_video != -1) s += ", count_video='" + key.count_video + "'";
                if (key.email != "")        s += ", email=N'" + key.email + "'";
                if (key.bri_day != -1)    s += ", bri_day='" + key.bri_day + "'";
                if (key.bri_month != -1)  s += ", bri_month='" + key.bri_month + "'";
                if (key.bri_year != -1)   s += ", bri_year='" + key.bri_year + "'";
                if (key.thanhpho != "")     s += ", thanhpho=N'" + key.thanhpho + "'";
                if (key.diachi != "")       s += ", diachi=N'" + key.diachi + "'";
                if (key.hoten != "")        s += ", hoten=N'" + key.hoten + "'";
                if (key.gioitinh != -1)   s += ", gioitinh='" + key.gioitinh + "'";
                if (key.sdt != "")          s += ", sdt='" + key.sdt + "'";
                if (key.nghenghiep != "")   s += ", nghenghiep=N'" + key.nghenghiep + "'";
                if (key.sothich != "")      s += ", sothich=N'" + key.sothich + "'";
                s = s.Substring(1);
                sql += s;
                sql += " where id=" + key.id;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();


                json = JsonConvert.SerializeObject(key);
                res = Request.CreateResponse(HttpStatusCode.Created,"creat");
                res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
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
        // PUT api/values?update_user=&id=
      

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
