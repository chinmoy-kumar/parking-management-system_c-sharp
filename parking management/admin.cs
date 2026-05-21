using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace parking_management
{
    public partial class admin : Form
    {   
        public string username;
        string cs= @"Data Source=localhost\SQLEXPRESS01;Initial Catalog=userDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private Label labelUsername;

        public admin()
        {
            InitializeComponent();

        }

        // LOAD ALL DATA
        void LoadData()
        {
            SqlConnection con = new SqlConnection(cs);

            string query = "SELECT * FROM Parking";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

			DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;

        }
        private void admin_Load(object sender, EventArgs e)
        {
            LoadData();
            SlotCalculation();
        }

        private void button1_Click(object sender, EventArgs e)//Add booking
        {
            add a = new add();

            a.username = username;

            a.Show();

            this.Hide();
           // this.Close();

        }

        private void button2_Click(object sender, EventArgs e)//update booking
        {
          update u= new update();
           u.username = username;
            u.Show();
            this.Hide();
              //  this.Close();
        }

        private void button3_Click(object sender, EventArgs e)//delete booking
        {
            delete d = new delete();
			d.ParentAdmin = this;
			d.username = username;
            d.Show();

           this.Hide();
            //this.Close();

        }

        private void button4_Click(object sender, EventArgs e)//payment
        {
            Form1 f = new Form1();

            f.username = username;
            f.Show();

           this.Hide();
          //  this.Close();


        }

        private void button5_Click(object sender, EventArgs e)//search booking
        {
            SqlConnection con = new SqlConnection(cs);

            string query = "SELECT * FROM Parking WHERE UPPER(VehicleNumber) LIKE UPPER(@v) OR UPPER(ownername) LIKE UPPER(@v)";

            SqlDataAdapter da = new SqlDataAdapter(query, con);

            da.SelectCommand.Parameters.AddWithValue("@v", "%" + textBox1.Text + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;

            if (dt.Rows.Count <= 0)
            {
                MessageBox.Show("Vehicle Not Found");
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            login l = new login();

            l.Show();

           this.Hide();
            //this.Close();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        //slot calculation
        void SlotCalculation()
        {
            SqlConnection con = new SqlConnection(cs);

            con.Open();

            string query = "SELECT COUNT(*) FROM Parking";

            SqlCommand cmd = new SqlCommand(query, con);

            int occupied = (int)cmd.ExecuteScalar();

            int total = 100;

            int available = total - occupied;

            // SHOW VALUES IN LABELS
            label7.Text = total.ToString();

            label8.Text = occupied.ToString();

            label9.Text = available.ToString();

            con.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            LoadData();
        }


		// Placeholder text for search box
		private void textBox1_Enter(object sender, EventArgs e)
		{
            if(textBox1.Text == "  e.g DHA-2626")
            {
				textBox1.Text = "";
				textBox1.ForeColor = Color.Black;
			}
		}

		private void textBox1_Leave(object sender, EventArgs e)
		{
            if(textBox1.Text =="")
            {
				textBox1.Text = "  e.g DHA-2626";
				textBox1.ForeColor = Color.Gray;
			}
		}

		// Update parking rate
		private void btnUpdateRate_Click(object sender, EventArgs e)
		{
			// 1. Check if the input is a valid decimal number
			if (decimal.TryParse(tbxParkingRate.Text, out decimal newRate))
			{
				// 2. Save the value to the Application Settings grid
				Properties.Settings.Default.ParkingRate = newRate;

				// 3. This writes the change permanently to the local hard drive
				Properties.Settings.Default.Save();

				MessageBox.Show("Rate updated and saved permanently!", "Success");
			}
			else
			{
				MessageBox.Show("Please enter a valid number.", "Error");
			}
		}
	}
}
