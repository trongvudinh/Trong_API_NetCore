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
    public class MoviesController : ApiController
    {
        // GET api/values
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            return res;
        }

        // GET api?idmovie
        [AllowAnonymous]
        public HttpResponseMessage Get_id(int idmovie)
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);

            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql += "SELECT ";
                sql += " t0.id id, ";
                sql += " t0.name name, ";
                sql += " t0.n_view n_view, ";
                sql += " t0.n_like n_like, ";
                sql += " t0.year_movie year, ";
                sql += " t0.warning warning, ";
                sql += " t0.content content, ";
                sql += " t0.urlavatar urlavatar, ";
                sql += " t0.content_re content_re, ";
                sql += " t0.time_thoiluong time, ";
                sql += " t0.creattime creattime, ";
                sql += " t0.user_creat user_creat, ";
                sql += " t5.name user_creat_name, ";
                sql += " t2.name company, ";
                sql += " t0.type_mov type_mov, ";
                sql += " t0.is_series is_series, ";
                sql += " t3.n_movie part_num, ";
                sql += " t4.count_movie part_total, ";
                sql += " t4.name series_name ";
                sql += " FROM t_movie t0 ";
                sql += " left join t_company t2 on t0.id_company = t2.id ";
                sql += " left join t_mov_series t3 on t0.id = t3.id ";
                sql += " left join t_series t4 on t3.id_series = t4.id";
                sql += " left join t_user t5 on t5.id = t0.user_creat";
                sql += " where t0.id=" + idmovie + "";



                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    res = Request.CreateResponse(HttpStatusCode.NotFound);
                    res.Content = new StringContent("No data found", Encoding.UTF8, "application/json");

                    return res;
                }
                Movie mov = new Movie();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    mov = new Movie();
                    mov.id = Convert.ToInt32(r["id"]);
                    mov.name = r["name"].ToString();
                    mov.name_re = "";
                    mov.name_en = "";
                    mov.n_view = Convert.ToInt32(r["n_view"]);
                    mov.n_like = Convert.ToInt32(r["n_like"]);
                    mov.year_movie = Convert.ToInt32(r["year"]);
                    mov.time_thoiluong = Convert.ToInt32(r["time"]);
                    mov.warning = Convert.ToInt32(r["warning"]);
                    mov.content = r["content"].ToString();
                    mov.content_re = r["content_re"].ToString();
                    mov.urlavatar = r["urlavatar"].ToString();
                    Company company = new Company();
                    company.name = r["company"].ToString();
                    mov.company = company;
                    //---------------------------------------user_creat-------------------------------------------------
                    User user = new User();
                    user.id = Convert.ToInt32(r["user_creat"]);
                    user.name = r["user_creat_name"].ToString();
                    mov.user_creat = user;
                    mov.createtime = Convert.ToDateTime(r["creattime"].ToString());
                    //---------------------------------------series_name-------------------------------------------------
                    if (Convert.ToInt32(r["is_series"].ToString()) == 1)
                    {
                        Series series = new Series();
                        series.name = r["series_name"].ToString();
                        series.content = "Part " + r["part_num"].ToString() + "/" + r["part_total"].ToString();
                        mov.series = series;
                    }
                    mov.type_mov = Convert.ToInt32(r["type_mov"]);
                    //---------------------------------------Actor-------------------------------------------------
                    sql = "";
                    sql += "select name from t_mov_actor t0 left join t_actor t1 on t0.id_actor=t1.id where t0.id_mov=" + idmovie;
                    cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adap2 = new MySqlDataAdapter(cmd);
                    DataSet ds2 = new DataSet();
                    adap2.Fill(ds2);
                    List<Actor> lst_actor = new List<Actor>();
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Actor actor = new Actor();
                        actor.name = r2["name"].ToString();
                        lst_actor.Add(actor);
                    }
                    mov.actor = lst_actor.ToArray();
                    //---------------------------------------Tag-------------------------------------------------
                    sql = "";
                    sql += "select t1.id,t1.name from t_tag1 t0 left join t_tag t1 on t0.id_tag=t1.id where t0.id_movie=" + idmovie;
                    cmd = new MySqlCommand(sql, conn);
                    adap2 = new MySqlDataAdapter(cmd);
                    ds2 = new DataSet();
                    adap2.Fill(ds2);
                    List<Tag> lst_tagr = new List<Tag>();
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Tag tag = new Tag();
                        tag.id = Convert.ToInt32(r2["id"]);
                        tag.name = r2["name"].ToString();
                        lst_tagr.Add(tag);
                    }

                    mov.tag = lst_tagr.ToArray();
                    //---------------------------------------Catalog -------------------------------------------------
                    sql = "";
                    sql += "select t1.id,t1.name from t_mov_cata t0 left join t_catalog t1 on t0.id_catalog=t1.id where t0.id_mov=" + idmovie;
                    cmd = new MySqlCommand(sql, conn);
                    adap2 = new MySqlDataAdapter(cmd);
                    ds2 = new DataSet();
                    adap2.Fill(ds2);
                    List<Catalog> lst_catalog = new List<Catalog>();
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Catalog cata = new Catalog();
                        cata.id = Convert.ToInt32(r2["id"]);
                        cata.name = r2["id"].ToString();
                        lst_catalog.Add(cata);
                    }

                    mov.catalog = lst_catalog.ToArray();
                    //---------------------------------------URL-------------------------------------------------
                    sql = "";
                    sql += "select url from t_mov_serve where id=" + idmovie;
                    cmd = new MySqlCommand(sql, conn);
                    adap2 = new MySqlDataAdapter(cmd);
                    ds2 = new DataSet();
                    adap2.Fill(ds2);
                    List<Serve> lst_serve = new List<Serve>();
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Serve ser = new Serve();
                        ser.url = r2["url"].ToString();
                        lst_serve.Add(ser);
                    }
                    mov.serve = lst_serve.ToArray();
                }
                json = JsonConvert.SerializeObject(mov);
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

            res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            return res;
        }

        // GET api/values/5
        [AllowAnonymous]
        public HttpResponseMessage Get_catalog(int catalog)
        {
            string json = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql = " SELECT";
                sql += " t0.id id,";
                sql += " t0.name name,";
                sql += " t0.n_view n_view,";
                sql += " t0.n_like n_like,";
                sql += " t0.year_movie year,";
                sql += " t0.warning warning,";
                sql += " t0.content content,";
                sql += " t0.urlavatar urlavatar,";
                sql += " t0.content_re content_re,";
                sql += " t0.time_thoiluong time,";
                sql += " t2.name company,";
                sql += " t0.type_mov type_mov,";
                sql += " t0.is_series is_series,";
                sql += " t3.n_movie part_num,";
                sql += " t4.count_movie part_total,";
                sql += " t5.name cata_name";
                sql += " FROM t_mov_cata t1";
                sql += " left join t_movie t0 on t0.id = t1.id_mov and t1.type=0";
                sql += " left join t_company t2 on t0.id_company = t2.id";
                sql += " left join t_mov_series t3 on t0.id = t3.id";
                sql += " left join t_series t4 on t3.id_series = t4.id";
                sql += " left join t_catalog  t5 on t1.id_catalog = t5.id";
                sql += " where t1.id_catalog=" + catalog + " and t1.type = 0";



                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    res = Request.CreateResponse(HttpStatusCode.NotFound);
                    res.Content = new StringContent("No data found", Encoding.UTF8, "application/json");

                    return res;
                }
                List<Movie> lst_movie = new List<Movie>();
                Movie mov = new Movie();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    mov = new Movie();
                    if (r["id"].ToString() != "")
                        mov.id = Convert.ToInt32(r["id"]);
                    mov.name = r["name"].ToString();
                    mov.name_re = "";
                    mov.name_en = "";
                    if (r["n_view"].ToString() !="")
                        mov.n_view = Convert.ToInt32(r["n_view"].ToString());
                    if (r["n_like"].ToString() != "")
                        mov.n_like = Convert.ToInt32(r["n_like"]);
                    if (r["year"].ToString() != "")
                        mov.year_movie = Convert.ToInt32(r["year"]);
                    if (r["time"].ToString() != "")
                        mov.time_thoiluong = Convert.ToInt32(r["time"]);
                    if (r["warning"].ToString() != "")
                        mov.warning = Convert.ToInt32(r["warning"]);
                    mov.content = r["content"].ToString();
                    mov.content_re = r["content_re"].ToString();
                    mov.urlavatar = r["urlavatar"].ToString();
                    Company company = new Company();
                    company.name = r["company"].ToString();
                    mov.company = company;
                    Series series = new Series();
                    series.name = "Part " + r["part_num"].ToString() + "/" + r["part_total"].ToString();
                    mov.series = series;
                    if (r["type_mov"].ToString() != "")
                        mov.type_mov = Convert.ToInt32(r["type_mov"]);

                    sql = "";
                    sql += "select name from t_mov_actor t0 left join t_actor t1 on t0.id_actor=t1.id where t0.id_mov=" + r["id"].ToString();
                    cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adap2 = new MySqlDataAdapter(cmd);
                    DataSet ds2 = new DataSet();
                    adap2.Fill(ds2);
                    List<Actor> lst_actor = new List<Actor>();
                    foreach (DataRow r2 in ds2.Tables[0].Rows)
                    {
                        Actor actor = new Actor();
                        actor.name = r2["name"].ToString();
                        lst_actor.Add(actor);
                    }
                    mov.actor = lst_actor.ToArray();
                    lst_movie.Add(mov);
                }
                json = JsonConvert.SerializeObject(lst_movie);
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
            res.Content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            return res;
        }

        [HttpPost]
        //POST api/Movie?uploadimg
        public HttpResponseMessage uploadImage(int uploadimg)
        {

            string imagename = "";
            var httpRequest = HttpContext.Current.Request;

            var postfile = httpRequest.Files["Img"];
            imagename = new String(Path.GetFileNameWithoutExtension(postfile.FileName).Take(10).ToArray()).Replace(" ", "-");
            imagename += DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postfile.FileName);
            var filepath = HttpContext.Current.Server.MapPath("~/Img/movie_avatar/" + imagename);
            postfile.SaveAs(filepath);
            int id_user;
            id_user = Convert.ToInt32(httpRequest["id_actor"]);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = " ";
                string name = "http://localhost:49696/Img/movie_avatar/" + imagename;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                sql = "update t_movie set urlavatar=N'" + MySqlHelper.EscapeString(name) + "' where id=" + id_user + " ";
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

        [HttpPost]
        //POST api/Movie?uploadimg
        public HttpResponseMessage uploadvideo(int uploadvideo)
        {

            string imagename = "";
            var httpRequest = HttpContext.Current.Request;

            var postfile = httpRequest.Files["Video"];
            int  idmovie = Convert.ToInt32(httpRequest["idmovie"]);
            int idserve = Convert.ToInt32(httpRequest["idserve"]);
            int idquality = Convert.ToInt32(httpRequest["idquality"]);

            imagename = new String(Path.GetFileNameWithoutExtension(postfile.FileName).Take(10).ToArray()).Replace(" ", "-");
            imagename += DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postfile.FileName);
            string name_folder = "video_" + idmovie.ToString() + "_" + idserve.ToString() + "_" + idquality.ToString() + "_";
            imagename = name_folder + imagename;

            var filepath = HttpContext.Current.Server.MapPath("~/Movie/" +imagename);
            postfile.SaveAs(filepath);
            var res = Request.CreateResponse(HttpStatusCode.OK);
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = " ";
                string name = "http://localhost:49696/Movie/" + imagename;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                sql = "update t_mov_quality set url=N'" + MySqlHelper.EscapeString(name) + "' where id=" + idquality + " and id_serve='"+ idserve + "' and id_movie='"+ idmovie + "' ";
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
        [HttpPost]
        public HttpResponseMessage Post([FromBody]Movie com)
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
            bool isgetotken = false;
            int level = Convert.ToInt32(ds.Tables[0].Rows[0]["level"].ToString());
            if (level >= 3)
            {
                if (level >= 7) warning = com.warning;
                else warning = 0;
                while (!isgetotken)
                {
                    try
                    {
                        sql = "SELECT AUTO_INCREMENT s FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'xemphim' AND TABLE_NAME = 't_movie' ";
                        cmd = new MySqlCommand(sql, conn);
                        adap = new MySqlDataAdapter(cmd);
                        ds = new DataSet();
                        adap.Fill(ds);
                        int id_mov = Convert.ToInt32(ds.Tables[0].Rows[0]["s"].ToString());

                        sql = "INSERT INTO t_movie (id, name, name_re, name_en, n_view, n_like, year_movie, creattime, user_creat,  " +
                            " time_thoiluong, warning, content, content_re, urlavatar, id_company, is_series, type_mov)" +
                            " VALUES ("+ id_mov + ", N'"+com.name+"', N'"+com.name_re+"', N'"+com.name_en+"', '0', '0', '"+com.year_movie+"', CURRENT_TIME(), '"+ id_user + "'," +
                            " '"+com.time_thoiluong+"', '"+warning+"', N'"+com.content+"', N'"+com.content_re+"', ' ', '"+com.company.id+"', '0', '"+com.type_mov+"');";

                        cmd = new MySqlCommand(sql, conn);
                        int i = cmd.ExecuteNonQuery();
                        isgetotken = true;

                        com.id = id_mov;

                        res = Request.CreateResponse(HttpStatusCode.OK, com);
                        cc = 1;

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
                }
               

                if (cc == 1)
                {
                    UsingFunction.update_list_tag(com.id, com.tag.ToList(), 0, id_user);
                    UsingFunction.update_list_catalog(com.id, com.catalog.ToList(), 0, id_user);
                    UsingFunction.update_list_actor(com.id, com.actor.ToList(), 0, id_user);
                    //------------------------------------------------Thêm Serve------------------------------------------------------------------------
                    UsingFunction.update_list_serve(com.id, com.serve.ToList(), 0, id_user);

                    if (com.company.id != -1)
                    {
                        try
                        {
                            sql = "SELECT count_movie c FROM t_series WHERE id=1";
                            cmd = new MySqlCommand(sql, conn);
                            adap = new MySqlDataAdapter(cmd);
                            ds = new DataSet();
                            adap.Fill(ds);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int count_mov =Convert.ToInt32(ds.Tables[0].Rows[0]["c"].ToString())+1;
                                sql = "INSERT INTO t_mov_series (id, id_series, n_movie) VALUES ('"+com.id+"', '"+com.series.id+"', '"+count_mov+"');";
                                cmd = new MySqlCommand(sql, conn);
                                int i = cmd.ExecuteNonQuery();
                            }
                        }
                        catch(Exception e) { }
                    }
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

                            sql = "INSERT INTO t_approved_value ( id_app, table_name, filed_name,key_id, value) VALUES ( '" + id_app + "', 't_movie', 'warning','" + com.id + "', '" + com.warning + "');";
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
        [HttpPut]
        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
