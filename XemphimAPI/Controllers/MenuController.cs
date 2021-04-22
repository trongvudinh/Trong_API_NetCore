using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text;
using XemphimAPI.Models;
using System.Threading;

namespace XemphimAPI.Controllers
{
    public class MenuController : ApiController
    {
        [AllowAnonymous]

        public HttpResponseMessage Get()
        {
            string json = "";
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql = " select t0.id cat_id,";
                sql += " t0.name cat_name,";
                sql += " t0.name_re cat_name_re,";
                sql += " t0.name_en cat_name_en,";
                sql += " t0.urlavatar cat_urlavatar,";
                sql += " t1.id menu_id,";
                sql += " t1.name menu_name,";
                sql += " t1.name_re menu_name_re,";
                sql += " t1.name_en menu_name_en,";

                sql += " t1.creattime menu_creattime,";
                sql += " t1.creat_user menu_creat_user,";
                sql += " t1.updatetime menu_updatetime,";
                sql += " t1.update_user menu_update_user";

                sql += " from t_catalog t0 left";
                sql += " join t_menu t1 on t0.id_menu = t1.id";
                sql += " where t1.status=0 ";
                sql += " order by t1.id";


                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                List<Menu> lst_menu = new List<Menu>();

                Menu m = new Menu();
                List<Catalog> lst_catalog = new List<Catalog>();
                int id = 0;
                string name = "", name_re = "", name_en = "";
                DateTime? creattime = null;
                 DateTime?   updatetime =null;
                string user_creat = "", user_update ="";
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(r["menu_id"]) == id)
                    {
                        Catalog cat = new Catalog(Convert.ToInt32(r["cat_id"]), r["cat_name"].ToString(), r["cat_name_re"].ToString()
                         , r["cat_name_en"].ToString(), r["cat_urlavatar"].ToString(), id);
                        lst_catalog.Add(cat);
                    }
                    else
                    {
                        if (id != 0)
                        {
                            m = new Menu(id, name, name_re, name_en, lst_catalog);
                            m.creattime = creattime;
                            m.usercreat = user_creat;
                            m.updatetime = updatetime;
                            m.userupdate = user_update;
                            lst_menu.Add(m);
                        }
                        id = Convert.ToInt32(r["menu_id"]);
                        name = r["menu_name"].ToString();
                        name_re = r["menu_name_re"].ToString();
                        name_en = r["menu_name_en"].ToString();
                        if (r["menu_creattime"].ToString() != "")
                            creattime = Convert.ToDateTime(r["menu_creattime"].ToString());
                        if (r["menu_creat_user"].ToString() != "")
                            user_creat = (r["menu_creat_user"].ToString());
                        if (r["menu_updatetime"].ToString() != "")
                            updatetime = Convert.ToDateTime(r["menu_updatetime"].ToString());
                        if (r["menu_update_user"].ToString() != "")
                            user_update = r["menu_update_user"].ToString();
                        lst_catalog = new List<Catalog>();
                        Catalog cat = new Catalog(Convert.ToInt32(r["cat_id"]), r["cat_name"].ToString(), r["cat_name_re"].ToString()
                        , r["cat_name_en"].ToString(), r["cat_urlavatar"].ToString(), id);
                        lst_catalog.Add(cat);
                    }
                }
                m = new Menu(id, name, name_re, name_en, lst_catalog);
                lst_menu.Add(m);
                var data = new { menus = lst_menu };
                json = JsonConvert.SerializeObject(lst_menu);
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
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        public string Get(int id, string name)
        {
            return "value2";
        }

        // POST api/values
        public HttpResponseMessage Post([FromBody]Menu val)
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
            if (level >= 7)
            {
                try
                {
                    sql = " INSERT INTO t_menu (name, name_re, name_en, creattime, creat_user, status) " +
                        "VALUES (N'"+val.name+"', N'"+val.name_re+"', N'"+val.name_en+"', CURRENT_TIME(), '"+id_user+"', '0');  ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();

                    res = Request.CreateResponse(HttpStatusCode.OK,val);
                }
                catch (MySqlException e)
                {
                    if (e.Number != 1062)
                    {
                        int id;
                        sql = "select status,id from t_menu where name='" + val.name + "'";
                        cmd = new MySqlCommand(sql, conn);
                        adap = new MySqlDataAdapter(cmd);
                        ds = new DataSet();
                        adap.Fill(ds);
                        id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        if (Convert.ToInt32(ds.Tables[0].Rows[0]["status"].ToString()) == 1)
                        {
                            sql = " update t_menu set status=0 where id='" + id + "' ";
                            cmd = new MySqlCommand(sql, conn);
                            int i = cmd.ExecuteNonQuery();

                            res = Request.CreateResponse(HttpStatusCode.OK, val);
                        }
                        else res= Request.CreateResponse(HttpStatusCode.NotModified);
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
        public HttpResponseMessage Put(int id, [FromBody]Menu val)
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
            if (level >= 7)
            {
                try
                {
                    sql = " update t_menu set name='"+val.name+"',updatetime=NOW(),update_user='"+id_user+"' where id='" + id + "' ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();

                    res = Request.CreateResponse(HttpStatusCode.OK);
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
            if (level >= 7)
            {
                try
                {
                    sql = " update t_menu set status=1 where id='" + id + "' ";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();

                    res = Request.CreateResponse(HttpStatusCode.OK);
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
