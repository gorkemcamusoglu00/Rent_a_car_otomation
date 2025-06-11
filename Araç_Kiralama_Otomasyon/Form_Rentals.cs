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
using static Araç_Kiralama_Otomasyon.Form_Users_Login;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace Araç_Kiralama_Otomasyon
{
    public partial class Form_Rentals : Form
    {
        private string connectionString = "DESKTOP-A5GKK7M\\SQLEXPRESS;Database=Araç_Kiralama_Otomasyon;Trusted_Connection=True;";
        public Form_Rentals()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["Araç_Kiralama_OtomasyonConnectionString"].ConnectionString;
        }

        private void Form_Rentals_Load(object sender, EventArgs e)
        {

            LoadAvailableCars();
           

        }

        private void LoadAvailableCars()
        {
            comboBox1.Items.Clear(); 

            string query = "SELECT Car_id, Car_name, Car_model, Model_year FROM Table_Car WHERE Is_available = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // ComboBox'ta gösterilecek metni oluşturmak için
                    string displayText = $"{reader["Car_name"]} - {reader["Car_model"]} ({reader["Model_year"]})";

                    comboBox1.Items.Add(new ComboBoxItem
                    {
                       Car_id = Convert.ToInt32(reader["Car_id"]) // Daha güvenli dönüşüm
,
                        DisplayText = displayText
                    });
                }
                reader.Close();
            }

            comboBox1.DisplayMember = "DisplayText"; 
            comboBox1.ValueMember = "Car_id";        
            comboBox1.SelectedIndex = -1;          

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CalculateTotalCost();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            CalculateTotalCost();
        }

        private void CalculateTotalCost()
        {
            if (comboBox1.SelectedItem == null || dateTimePicker2.Value <= dateTimePicker1.Value)
            {
                textBox1.Text = "0";
                return;
            }

            ComboBoxItem selectedItem = (ComboBoxItem)comboBox1.SelectedItem;
            int Car_id = selectedItem.Car_id;
            decimal pricePerDay = GetPricePerDay(Car_id);
            int rentalDays = (dateTimePicker2.Value - dateTimePicker1.Value).Days;

            decimal totalCost = rentalDays * pricePerDay;
            textBox1.Text = totalCost.ToString("F2");
        }

        private decimal GetPricePerDay(int car_id)
        {
            decimal pricePerDay = 0;
            string query = "SELECT Price_Per_Day FROM Table_Car WHERE Car_id = @Car_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Car_id", car_id);

                connection.Open();
                pricePerDay = Convert.ToDecimal(command.ExecuteScalar());
            }

            return pricePerDay;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || dateTimePicker2.Value <= dateTimePicker1.Value)
            {
                MessageBox.Show("Lütfen geçerli bir araç ve tarih seçiniz.");
                return;
            }

            ComboBoxItem selectedItem = (ComboBoxItem)comboBox1.SelectedItem;
            int car_id = selectedItem.Car_id;
            DateTime rentalDate = dateTimePicker1.Value;
            DateTime returnDate = dateTimePicker2.Value;
            decimal totalCost = Convert.ToDecimal(textBox1.Text);

           
            // Kiralama kaydı

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("sp_insert_into_Table_Rentals", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Car_id", car_id);               
                command.Parameters.AddWithValue("@Rental_Date", rentalDate);
                command.Parameters.AddWithValue("@Return_Date", returnDate);
                command.Parameters.AddWithValue("@Total_Cost", totalCost);
                SqlParameter outputParam = new SqlParameter("@NewRentalId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);

                connection.Open();
                command.ExecuteNonQuery();
                int newRentalId = Convert.ToInt32(outputParam.Value);
            }


            MessageBox.Show("Kiralama işlemi başarıyla kaydedildi.");
            ResetForm();
        }

        private void ResetForm()
        {
            comboBox1.Items.Clear();
            LoadAvailableCars();
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now.AddDays(1);
            textBox1.Text = "0";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query = "SELECT Car_id, Car_name, Car_model, Model_year, Price_Per_Day, Is_available FROM Table_Car";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;
            }
        }
    }
    
}
