using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XemphimAPI.Models;
using XemphimAPI.Providers;
using XemphimAPI.Results;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace XemphimAPI.Controllers
{
    [AllowAnonymous]
    public class CatalogController : ApiController
    {
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
                sql += " t0.id_menu menu_id";
                sql += " from t_catalog t0 ";
                sql += " order by t0.id";


                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                List<Catalog> lst_catalog = new List<Catalog>();
                int id = 0;
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    id = Convert.ToInt32(r["menu_id"]);
                    Catalog cat = new Catalog(Convert.ToInt32(r["cat_id"]), r["cat_name"].ToString(), r["cat_name_re"].ToString()
                         , r["cat_name_en"].ToString(), r["cat_urlavatar"].ToString(), id);
                    lst_catalog.Add(cat);
                }
                var data = new { catalogs = lst_catalog };
                json = JsonConvert.SerializeObject(lst_catalog);
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
        public string Get2(int vc)
        {
            return "value2";
        }

        // POST api/values
        public string Post([FromBody]string value)
        {
            return "post 1";
        }
        public string Post_vc([FromBody]string value, int vc)
        {
            return "post 2";
        }
        public string Post_3([FromBody]string value, string kk)
        {
            return "post 3";
        }

        // PUT api/values/5
        public string Put_1(int vc, [FromBody]string value)
        {
            return "put1";
        }

        public string Put_2(string kk, [FromBody]string value)
        {
            return "put2";
        }
        public string Put_3([FromBody]string value, string zz)
        {
            return "put 3";
        }
        // DELETE api/values/5
        public string Delete(int id)
        {
            return "del 1";
        }
    }
}
