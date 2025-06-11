using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Araç_Kiralama_Otomasyon
{
    public partial class Form_Users_Login : Form
    {
        private string connectionString= "Server=DESKTOP-A5GKK7M\\SQLEXPRESS;Database=Araç_Kiralama_Otomasyon;Trusted_Connection=True;";

       
        public Form_Users_Login()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["Araç_Kiralama_OtomasyonConnectionString"].ConnectionString;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Form_Register form2 = new Form_Register();
            this.Hide();
            form2.ShowDialog();
            this.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string isim = textBox1.Text.Trim();
            string soyisim = textBox2.Text.Trim();
            string mail = textBox3.Text.Trim();

            
            if (IsValidUser(isim, soyisim, mail))
            {
                
                MessageBox.Show("Giriş başarılı! Kiralama sayfasına yönlendiriliyorsunuz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

               
                Form_Rentals Form2 = new Form_Rentals();
                this.Hide();
                Form2.ShowDialog();
                this.Close(); 
            }
            else
            {
                MessageBox.Show("Kullanıcı bilgileri hatalı! Lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidUser(string isim, string soyisim, string mail)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(1) FROM Table_Customer WHERE Customer_name =@Customer_name AND Customer_surname = @Customer_surname AND Customer_mail = @Customer_mail";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Customer_name", isim);
                        cmd.Parameters.AddWithValue("@Customer_surname", soyisim);
                        cmd.Parameters.AddWithValue("@Customer_mail", mail);

                        connection.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
