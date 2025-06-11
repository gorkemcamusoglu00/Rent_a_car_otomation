using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;


namespace Araç_Kiralama_Otomasyon
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=DESKTOP-R428UL5\\SQLEXPRESS;Database=Araç_Kiralama_Otomasyon;Trusted_Connection=True;";
        public Form1()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["Araç_Kiralama_OtomasyonConnectionString"].ConnectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        MessageBox.Show("1 Saniye içerisinde yönlendirileceksiniz", "Bağlantınız Açık",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                Thread.Sleep(1000);
                Form_Users_Login form2 = new Form_Users_Login();
                this.Hide();
                form2.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Üzgünüz, bağlantı sağlanamadı" + ex);
            }
        }
    }
}
