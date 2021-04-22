using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XemphimAPI.Models;

namespace XemphimAPI.Controllers
{
    public class UsingFunction
    {
        public static long creat_token( int iduser, Token token)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);

            long re = 0;
            conn.Open();
            bool isgetotken = false;
            while (!isgetotken)
            {
                try
                {
                    string sql = "";
                    sql = " select  ifnull(max(id),0) id from t_token where id_user = " + iduser;

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adap.Fill(ds);
                    long id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString()) + 1;
                    sql = " insert into t_token(id,ipaddress,geo_1,geo_2,macadress,id_user) value("
                        + id + ",'" + token.ipaddress + "'," + token.geo_1 + "," + token.geo_2 + ",'" + token.macadress + "'," + iduser + ")";

                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                    isgetotken = true;
                    re = id;
                }
                catch (MySqlException e)
                {
                    if (e.Number != 1062)
                    {
                        isgetotken = true;
                    }
                    else re = -1;
                }
                catch (Exception e)
                {
                    isgetotken = true;
                    re = -1;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Clone();
                    }
                }
            }
            return re;
        }
        public static string randomname()
        {
            string[] ho = { "Nguyễn","Trần","Lê","Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ","Võ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Lý","Cao","Lồ","Cút","Vàng","Tạ","Vùi",
            "Bồi","Công Tằng Tôn Nữ","Giang","Giáng","Giao","Giáp","Gương","Buồi","Lồn","Cặc","Dái","Chim","Địt","Cái","Con chó","Thần thánh","Tuyệt Vời","Đẹp trai"};
            string[] sub_name = {"Mỹ","Duệ","Tăng","Cường","Tráng","Liên","Huy","Phát","Bội","Hương","Linh","Nghi","Hàm","Tốn","Thuận","Vỹ","Vọng","Biểu","Không","Quang","Hoài",
                "Thuỳ","Tú","Ngọc","Bảo","Nguyệt","Minh","Vân","Lam","Ngân","Buồi","Lồn","Cặc","Dái","Chim","Địt","Cái","Óc vật","Đầu đất","Thần thánh","Tuyệt Vời","Đẹp trai"
            };
            string[] name ={"An","Ân","Anh","Bắc","Bách","Bằng","Bảo","Biên","Bình","Cảnh","Cát","Chấn","Chiến","Công","Cương","Cường","","Đại","Đan","Đăng","Đạo","Đạt","Đông","Điền","Điệp","Định","Đô","Đoàn","Đức","Diện","Du","Doanh","Dũng","Dương","Duy","Gia","Giang","Giáp","Hà","Hải","Hạnh","Hào","Hậu","Hiển","Hiệp","Hiếu","Hoan","Hoàng","Hoạt","Huân","Hùng","Hưng","Hưởng","Huy","Huỳnh","Khải","Khang","Khanh","Khánh","Khoa","Khôi","Kiên","Kiện","Kiệt","Kỳ","Lâm","Lập","Lễ","Liêm","Linh","Lĩnh","Lộc","Lợi","Long","Luận","Luật","Mạnh","Minh","Nam","Nghị","Nghĩa","Ngọc","Nguyên","Nhân","Nhật","Ninh","Phát","Phi","Phong","Phú","Phúc","Phương","Quân","Quang","Quảng","Quý","Quyền","Quyết","Sang","Sáng","Sơn","Tài","Tâm","Tân","Thái","Thắng","Thành","Thiên","Thiện","Thịnh","Thọ","Thông","Toàn","Trí","Trình","Trọng","Tú","Tuấn","Vương","An","Anh","Anh","Anh","Anh","Anh","Băng","Băng","Băng","Bằng","Bích","Bình","Ca","Ca","Cát","Châu","Châu","Châu","Chi","Chi","Chi","Chi","Chi","Chi","Chi","Chung","Cúc","Dạ","Dao","Diệu","Diệu","Diệu","Du","Du","Dung","Dung","Dung","Duyên","Dương","Dương","Dương","Đan","Đan","Đan","Đào","Đăng","Giang","Giang","Giang","Giang","Giang","Giang","Hà","Hà","Hà","Hà","Hà","Hà","Hà","Hạ","Hạ","Hạ","Hạnh","Hằng","Hằng","Hằng","Hiền","Hiền","Hoa","Hoa","Hòa","Hoàn","Hồng","Huyền","Huyền","Hương","Hương","Hương","Hương","Hưởng","Khanh","Khê","Kê","Khôi","Khuê","Khuê","Khuyên","Khuyên","Kim","Kim","Kim","Lam","Lam","Lam","Lam","Lam","Lam","Lan","Lan","Lan","Lan","Lan","Lan","Lan","Lăng","Lâm","Lâm","Lâm","Lâm","Lệ","Liên","Liên","Linh","Linh","Linh","Linh","Linh","Linh","Ly","Ly","Ly","Mai","Mai","Mai","Mai","Mai","Mai","Mai","Mai","Mẫn","Mi","Miên","Miên","Minh","My","My","Mỹ","Mỹ","Mỹ","Nga","Nga","Nga","Ngân","Ngân","Nghi","Nghi","Nghi","Ngọc","Ngọc","Ngọc","Ngọc","Ngọc","Ngôn","Nguyên","Nguyên","Nguyệt","Nguyệt","Nguyệt","Nguyệt","Nhàn","Nhạn","Nhạn","Nhân","Nhi","Nhi","Nhi","Nhi","Nhi","Nhi","Nhi","Nhi","Nhiên","Nhiên","Nhiên","Nhơn","Oanh","Oanh","Oanh","Oanh","Phi","Phong","Phương","Phương","Phương","Phương","Phương","Phương","Phương","Quân","Quế","Quyên","Quyên","Quyên","Quỳnh","Quỳnh","Quỳnh","Quỳnh","Quỳnh","Quỳnh","Quỳnh","Sa","San","Tâm","Tâm","Tâm","Tâm","Tâm","Tâm","Tâm","Tâm","Thanh","Thanh","Thanh","Thanh","Thanh","Thảo","Thảo","Thảo","BạchThảo","Thảo","Thảo","Thảo","Thi","Giang","Hoa","Thanh","Thoa","Thoa","Thoại","Thông","Thu","Thu","Thu","Thu","Thu","Thuần","Trang","Thùy","Thủy","Thủy","Thủy","Thủy","Thụy","Thư","Thư","Thư","Thương","Thương","Thường","Tiên","Tiên","Tiên","Trang","Trang","Trang","Trang","Trang","Trang","Tranh","Trà","Trung","Trâm","Trâm","Trâm","Trâm","Trân","Trúc","Tú","Tuyền","Tuyền","Tuyến","Tường","Tuyết","Tuyết","Uyên","Uyên","Uyển","Uyển","Vân","Vân","Vọng","Vũ","Vy","Vy","Vy","Vy","Vỹ","Vỹ","xanh","Xuân","Xuân","Xuân","Xuân","Xuân","Yên","Yến","Yến"
            };

            string re = "";
            Random r = new Random();
            int i=r.Next(0, 39);re = re + ho[i]+" ";
            i = r.Next(0, 41); re = re + sub_name[i]+" ";
            i = r.Next(0, 415); re = re + name[i];
            return re;
        }

        public static string random_urlavatar()
        {
            string[] ho = { "1","2","3","4", "5", "6", "7", "8","9", "10"};

            string re = "";
            Random r = new Random();
            int i = r.Next(0, 9); re = re + ho[i];
            return re;
        }
        public static string update_list_tag(int id_mov ,List<Tag> list_tag,int type,int id_user)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql = " select  t0.id_tag,t0.id_movie,t0.type from t_tag1 t0 where t0.id_movie =  " + id_mov+" and type = '"+type+"' " ;

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                List<Tag> list_temp = new List<Tag>();
                foreach(DataRow r in ds.Tables[0].Rows)
                {
                    Tag t = new Tag();
                    t.id = Convert.ToInt32(r["id_tag"].ToString());
                    list_temp.Add(t);
                }
                HashSet<int> writerIds = new HashSet<int>(list_temp.Select(x => x.id));
                List<Tag> temp1= list_tag.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach(Tag t in temp1)
                {
                    if (t.id == -1)
                    {
                        sql = "select status,id from t_tag where  name='" + t.name + "'";
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
                                cmd.ExecuteNonQuery();
                                t.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                            }
                        }
                        else
                        {
                            sql = "INSERT INTO t_tag ( name, name_re, sl_movie, creattime, user_creat,  status) " +
                                "VALUES ( N'" + t.name + "', N'" + t.name + "', '', CURRENT_DATE(), '" + id_user + "', '0') ";
                            cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();

                            sql = "select id from t_tag where name= '" + t.name + "'";
                            cmd = new MySqlCommand(sql, conn);
                            adap = new MySqlDataAdapter(cmd);
                            ds = new DataSet();
                            adap.Fill(ds);
                            t.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                        }
                    }
                    sql = "INSERT INTO t_tag1 (id_tag, id_movie, type, creattime) VALUES ('" + t.id+"', '"+id_mov+"', '"+type+ "',CURRENT_TIME());";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                }

                writerIds = new HashSet<int>(list_tag.Select(x => x.id));
                temp1 = list_temp.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach (Tag t in temp1)
                {
                    sql = "delete from t_tag1 where id_tag='"+t.id+"' and type='"+type+"' and id_movie='"+id_mov+"'";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }

            return "";

        }

        public static string update_list_catalog(int id_mov, List<Catalog> list_cata, int type,int id_user)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql = " select  t0.id_catalog,t0.id_mov,t0.type from t_mov_cata t0 where t0.id_mov =  " + id_mov + " and type = '" + type + "' ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                List<Catalog> list_temp = new List<Catalog>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Catalog t = new Catalog();
                    t.id = Convert.ToInt32(r["id_catalog"].ToString());
                    list_temp.Add(t);
                }
                HashSet<int> writerIds = new HashSet<int>(list_temp.Select(x => x.id));
                List<Catalog> temp1 = list_cata.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach (Catalog t in temp1)
                {
                    sql = "INSERT INTO t_mov_cata (id_mov, id_catalog, createtime, user_creat, type)" +
                        " VALUES ('"+id_mov+"', '"+t.id+"', CURRENT_DATE(), '"+ id_user + "', '"+type+"');";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                }

                writerIds = new HashSet<int>(list_cata.Select(x => x.id));
                temp1 = list_temp.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach (Catalog t in temp1)
                {
                    sql = "delete from t_mov_cata where id_catalog='" + t.id + "' and type='" + type + "' and id_mov='" + id_mov + "'";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }

            return "";

        }

        public static string update_list_actor(int id_mov, List<Actor> list_actor, int type, int id_user)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql = " select  t0.id_actor,t0.id_mov,t0.type_mov from t_mov_actor t0 where t0.id_mov =  " + id_mov + " and type_mov = '" + type + "' ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                List<Actor> list_temp = new List<Actor>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Actor t = new Actor();
                    t.id = Convert.ToInt32(r["id_actor"].ToString());
                    list_temp.Add(t);
                }
                HashSet<int> writerIds = new HashSet<int>(list_temp.Select(x => x.id));
                List<Actor> temp1 = list_actor.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach (Actor t in temp1)
                {
                    sql = "INSERT INTO t_mov_actor (id_mov, id_actor, creattime, user_creat, type, type_mov) " +
                        "VALUES ('"+id_mov+"', '"+t.id+"', CURRENT_DATE(), '"+id_user+"', '0', '"+type+"');";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                }

                writerIds = new HashSet<int>(list_actor.Select(x => x.id));
                temp1 = list_temp.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach (Actor t in temp1)
                {
                    sql = "delete from t_mov_actor where id_actor='" + t.id + "' and type=0 and  type_mov='" + type + "' and id_mov='" + id_mov + "'";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }

            return "";

        }

        public static string update_list_serve(int id_mov, List<Serve> list_serve, int type, int id_user)
        {
            MySqlConnection conn = new MySqlConnection(ConnnectData.connectionString);
            conn.Open();
            try
            {
                string sql = "";
                sql = " select  t0.id,t0.id_movie from t_mov_serve t0 where t0.id_movie =  " + id_mov + " ";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adap.Fill(ds);
                List<Serve> list_temp = new List<Serve>();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    Serve t = new Serve();
                    t.id = Convert.ToInt32(r["id"].ToString());
                    list_temp.Add(t);
                }
                HashSet<int> writerIds = new HashSet<int>(list_temp.Select(x => x.id));
                List<Serve> temp1 = list_serve.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach (Serve t in temp1)
                {
                    sql = "INSERT INTO t_mov_serve (id, id_movie,url,id_serve, creattime, user_creat) " +
                        "VALUES ('" + t.id + "', '" + id_mov + "',N'"+t.url+"', '"+t.id_serve+"' , CURRENT_DATE(), '" + id_user + "');";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                    foreach(Quality q in t.list_quality)
                    {
                        sql = "INSERT INTO t_mov_quality (id, id_serve, id_movie, name, url, width, height, type_file, size_MB, creattime, creat_user)" +
                            " VALUES ('"+q.id+"', '"+ t.id + "', '"+ id_mov + "', '"+q.name+"', ' ', '"+q.width+"', '"+q.height+"', N'"+ MySqlHelper.EscapeString(q.type_file)+"' , " +
                            "'"+q.size_MB+"', CURRENT_TIME(), '"+id_user+"');";
                        cmd = new MySqlCommand(sql, conn);
                        i = cmd.ExecuteNonQuery();
                    }
                }

                writerIds = new HashSet<int>(list_serve.Select(x => x.id));
                temp1 = list_temp.Where(x => !writerIds.Contains(x.id)).ToList();
                foreach (Serve t in temp1)
                {
                    sql = "delete from t_mov_serve where id='" + t.id + "' and id_movie='" + id_mov + "'";
                    cmd = new MySqlCommand(sql, conn);
                    int i = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Clone();
                }
            }

            return "";

        }
    }
}