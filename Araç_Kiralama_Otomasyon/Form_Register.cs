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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Araç_Kiralama_Otomasyon
{
    public partial class Form_Register : Form
    {

        private string connectionString = "Server=DESKTOP-A5GKK7M\\SQLEXPRESS;Database=Araç_Kiralama_Otomasyon;Trusted_Connection=True;";
       

        public Form_Register()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["Araç_Kiralama_OtomasyonConnectionString"].ConnectionString;
        }

        private void Form_Register_Load(object sender, EventArgs e)
        {
            ComboLoadData();

            comboBox1.SelectedIndex = -1;

            
        }

        private void ComboLoadData()
        {
            try
            {
                string query = "SELECT Customer_gender FROM Table_Gender";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    SqlDataAdapter da1 = new SqlDataAdapter(query, connection);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);

                    comboBox1.DataSource = dt1;
                    comboBox1.DisplayMember = "Customer_gender";
                    comboBox1.ValueMember = "Customer_gender";
                }
            }

            catch (Exception ex)
            {

                MessageBox.Show("Hata = " + ex);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string c_name = textBox1.Text.ToString();
            string c_surname = textBox2.Text.ToString();
            int c_age = int.Parse(textBox4.Text.ToString());
            string c_gender = comboBox1.SelectedValue.ToString();
            int c_gender_id = comboBox1.SelectedIndex + 1;

            string c_tel = maskedTextBox1.Text.ToString();

            string c_mail = textBox3.Text.ToString();

            c_tel = c_tel.Replace("(", "").Replace(")", "").Replace("_", "").Replace(" ", "");

            if (!c_tel.StartsWith("0"))
            {
                c_tel = "0" + c_tel;
            }

            if (IsCustomerDuplicate(c_mail, c_tel))
            {
                MessageBox.Show("Bu telefon ve mail adresi zaten kayıtlı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insert_into_Table_Customer", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Customer_name", c_name);
                        cmd.Parameters.AddWithValue("@Customer_surname", c_surname);
                        cmd.Parameters.AddWithValue("@Customer_gender_id", c_gender_id);
                        cmd.Parameters.AddWithValue("@Customer_tel", c_tel);
                        cmd.Parameters.AddWithValue("@Customer_mail", c_mail);
                        cmd.Parameters.AddWithValue("@Customer_age", c_age);
                        connection.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Kayıt işlemi yapıldı", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearComponents();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Kayıt işlemi esnasında bir hata oluştu: " + ex.Message);
            }

        }

        private void ClearComponents()
        {
            
            textBox3.Clear();
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = -1;
            maskedTextBox1.Clear();
            textBox4.Clear();
        }


        private bool IsCustomerDuplicate(string mail, string tel)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Table_Customer WHERE Customer_mail = @Customer_mail and Customer_tel = @Customer_tel";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Customer_mail", mail);
                command.Parameters.AddWithValue("@Customer_tel", tel);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                int selected_id = int.Parse(selectedRow.Cells[0].Value.ToString());

                string c_name = selectedRow.Cells[1].Value.ToString();
                string c_surnname = selectedRow.Cells[2].Value.ToString();
                string c_age = selectedRow.Cells[3].Value.ToString();
                string c_gender = selectedRow.Cells[4].Value.ToString();
               
                string c_tel = selectedRow.Cells[5].Value.ToString();
                string c_mail = selectedRow.Cells[6].Value.ToString();

                c_tel = c_tel.Replace("(", "").Replace(")", "").Replace("_", "").Replace(" ", "");

                if (c_tel.StartsWith("0"))
                {
                    c_tel = c_tel.Remove(0, 1);
                }

                textBox1.Text = c_name;
                textBox2.Text = c_surnname;
                textBox4.Text = c_age;
                textBox3.Text = c_mail;

                maskedTextBox1.Text = c_tel;

               
                comboBox1.SelectedValue = c_gender;

            }

            else
            {
                MessageBox.Show("Lütfen güncelleme işlemi için bir kayıt seçiniz!");
            }


        }


        private void Listele_Click(object sender, EventArgs e)
        {
          
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_select_Table_Customer", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        DataTable dt = new DataTable();

                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                        connection.Open();
                        da.Fill(dt);

                        dataGridView2.DataSource = dt;

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Kayıt işlemi esnasında bir hata oluştu: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int selected_id = int.Parse(dataGridView2.SelectedRows[0].Cells[0].Value.ToString());

            string c_name = textBox1.Text;
            string c_surname = textBox2.Text;
            int c_age = int.Parse(textBox4.Text);

            string c_gender_value = comboBox1.SelectedValue.ToString();
            int c_gender_id = comboBox1.SelectedIndex + 1;

            string c_tel = maskedTextBox1.Text;
            string c_mail = textBox3.Text;

            c_tel = c_tel.Replace("(", "").Replace(")", "").Replace("_", "").Replace(" ", "");

            if (!c_tel.StartsWith("0"))
            {
                c_tel = "0" + c_tel;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_update_Table_Customer", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Customer_id", selected_id);
                        cmd.Parameters.AddWithValue("@Customer_name", c_name);
                        cmd.Parameters.AddWithValue("@Customer_surname", c_surname);
                        cmd.Parameters.AddWithValue("@Customer_age", c_age);
                        cmd.Parameters.AddWithValue("@Customer_gender_id", c_gender_id);
                       
                        cmd.Parameters.AddWithValue("@Customer_tel", c_tel);
                        cmd.Parameters.AddWithValue("@Customer_mail", c_mail);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Güncelleme işlemi yapıldı", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearComponents();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Güncelleme işleminde hata oluştu: " + ex.Message);
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (dataGridView2.SelectedRows.Count > 0)
            {

                int selected_id = int.Parse(dataGridView2.SelectedRows[0].Cells[0].Value.ToString());

                DialogResult result = MessageBox.Show("Bu kaydı silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection connnection = new SqlConnection(connectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_delete_Table_Customer", connnection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Customer_id", selected_id);

                                connnection.Open();
                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Silme işlemi gerçekleştirildi", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Silme işlemi yapılamadı. Hata = " + ex.Message);
                    }
                }

            }

            else
            {
                MessageBox.Show("Lütfen silmek için bir değer seçiniz");
            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form_Users_Login Form2 = new Form_Users_Login();
            this.Hide();
            Form2.ShowDialog();
            this.Close();
        }
    }
}
