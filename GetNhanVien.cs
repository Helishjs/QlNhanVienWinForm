﻿using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Threading;
namespace QuanLyNhanSu
{
    class GetNhanVien
    {
        string sqlstring = @"Data Source=DESKTOP-B4J24OU\MSSQLSERVER01;Initial Catalog=QLNhanVien;Integrated Security=True;TrustServerCertificate=True";
        public void LoadNhanVien(DataGridView dataGridViewNhanVien)
        {
            try
            {
                using (SqlConnection sqlconnect = new SqlConnection(sqlstring))
                {
                    sqlconnect.Open();
                    string query = @"SELECT NhanVien.ID_NhanVien, NhanVien.HoTen, NhanVien.NgaySinh, 
                        NhanVien.GioiTinh, NhanVien.QueQuan, NhanVien.Email, 
                        NhanVien.SDT, NhanVien.SoCCCD, NhanVien.DiaChi, 
                        ChucVu.Ten_ChucVu, PhongBan.Ten_PhongBan 
                 FROM NhanVien 
                 JOIN ChucVu ON ChucVu.ID_ChucVu = NhanVien.ID_ChucVu 
                 JOIN PhongBan ON PhongBan.ID_PhongBan = NhanVien.ID_PhongBan";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, sqlconnect);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Gán dữ liệu vào DataGridView
                    dataGridViewNhanVien.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        public void SearchNhanVien(string search,DataGridView dataGridView)
        {
            try
            {
                using(SqlConnection sqlconnection = new SqlConnection(sqlstring))
                {
                    sqlconnection.Open();
                    string result = "%" + search + "%";
                    string query = "SELECT NhanVien.ID_NhanVien, NhanVien.HoTen, NhanVien.NgaySinh,NhanVien.GioiTinh, NhanVien.QueQuan, NhanVien.Email,NhanVien.SDT, NhanVien.SoCCCD, NhanVien.DiaChi,ChucVu.Ten_ChucVu, PhongBan.Ten_PhongBan FROM NhanVien JOIN ChucVu ON ChucVu.ID_ChucVu = NhanVien.ID_ChucVu JOIN PhongBan ON PhongBan.ID_PhongBan = NhanVien.ID_PhongBan WHERE HoTen LIKE @Search";
                    using(SqlCommand cmd = new SqlCommand(query, sqlconnection))
                    {
                        cmd.Parameters.AddWithValue("@Search", result);
                        using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView.DataSource = dt;
                        }
                    }
                    
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Có lỗi: " + ex);
            }
        }
        public bool ChangePassword(string Password, string Username, string newPassword)
        {
            try
            {
                using (SqlConnection sqlconnect = new SqlConnection(sqlstring))
                {
                    sqlconnect.Open();
                    string query_1 = @"SELECT COUNT(*) FROM NguoiDung WHERE Password = @password AND Username = @username";
                    using (SqlCommand cmd = new SqlCommand(query_1, sqlconnect))
                    {
                        cmd.Parameters.AddWithValue("@password", Password);
                        cmd.Parameters.AddWithValue("@username", Username);
                        int count = (int)(cmd.ExecuteScalar() ?? 0);
                        if (count == 0)
                        {
                            MessageBox.Show("Mật khẩu cũ không đúng!");
                            return false;
                        }
                    }

                    string query_2 = @"UPDATE NguoiDung SET Password = @newPassword WHERE Username = @username";
                    using (SqlCommand cmd = new SqlCommand(query_2, sqlconnect))
                    {
                        cmd.Parameters.AddWithValue("@newPassword", newPassword);
                        cmd.Parameters.AddWithValue("@username", Username);
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Đổi mật khẩu thành công!");
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật mật khẩu thất bại!");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                return false;
            }
        }
        public bool ResetPassword(string Username,string CCCD)
        {
            try
            {
                using (SqlConnection sqlconnect = new SqlConnection(sqlstring))
                {
                    sqlconnect.Open();
                    string query_1 = @"SELECT COUNT(*) FROM NhanVien WHERE SoCCCD = @cccd AND Username = @username";
                    using (SqlCommand cmd = new SqlCommand(query_1, sqlconnect))
                    {
                        cmd.Parameters.AddWithValue("@cccd", CCCD);
                        cmd.Parameters.AddWithValue("@username", Username);
                        int count = (int)(cmd.ExecuteScalar() ?? 0);
                        if (count == 0)
                        {
                            MessageBox.Show("Tên đăng nhập hoặc cccd không chính xác");
                            return false;
                        }
                    }

                    string query_2 = @"UPDATE NguoiDung SET Password = @newPassword WHERE Username = @username";
                    using (SqlCommand cmd = new SqlCommand(query_2, sqlconnect))
                    {
                        cmd.Parameters.AddWithValue("@newPassword",CCCD);
                        cmd.Parameters.AddWithValue("@username", Username);
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Đổi mật khẩu thành công!");
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật mật khẩu thất bại!");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                return false;
            }
        }

        public void LogOut()
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                User.Username = null;
                User.ID_NhanVien = 0;
                User.Password = null;
                User.Role = null;
            }
        }

        public void UpdateNhanVien(string id, string hoTen, DateTime ngaySinh, string gioiTinh,
                           string queQuan, string email, string sdt, string soCCCD,
                           string diaChi, string phongBan, string chucVu)
        {
            try
            {
                using (SqlConnection sqlconnect = new SqlConnection(sqlstring))
                {
                    sqlconnect.Open();
                    string query = @"UPDATE NhanVien 
                 SET HoTen = @HoTen, 
                     NgaySinh = @NgaySinh, 
                     GioiTinh = @GioiTinh, 
                     QueQuan = @QueQuan, 
                     Email = @Email, 
                     SDT = @SDT, 
                     SoCCCD = @SoCCCD, 
                     DiaChi = @DiaChi,  -- ĐÃ SỬA LỖI
                     ID_PhongBan = @PhongBan, 
                     ID_ChucVu = @ChucVu 
                 WHERE ID_NhanVien = @ID";
                    using (SqlCommand cmd = new SqlCommand(query, sqlconnect))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@HoTen", hoTen);
                        cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                        cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                        cmd.Parameters.AddWithValue("@QueQuan", queQuan);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@SDT", sdt);
                        cmd.Parameters.AddWithValue("@SoCCCD", soCCCD);
                        cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                        cmd.Parameters.AddWithValue("@PhongBan", Convert.ToInt32(phongBan));
                        cmd.Parameters.AddWithValue("@ChucVu", Convert.ToInt32(chucVu));
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không có dữ liệu được cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex);
            }
        }
    }
}


