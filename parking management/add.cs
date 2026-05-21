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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace parking_management
{
    public partial class add : Form
    {
        public string username;
		public dashboard ParentDashboard { get; set; }
		public admin ParentAdmin { get; set; }
		private DateTimePicker timePicker;

        string cs = @"Data Source=localhost\SQLEXPRESS01;Initial Catalog=userDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        public add()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            SqlConnection con = new SqlConnection(cs);

            string showQuery = "SELECT * FROM Parking WHERE VehicleNumber=@v";

            SqlDataAdapter da = new SqlDataAdapter(showQuery, con);

            da.SelectCommand.Parameters.AddWithValue("@v", textBox2.Text);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" ||
		 dateTimePickerStart.Text == "" || comboBox1.Text == "")
            {
                MessageBox.Show("Please Fill All Fields");
            }

            else
            {
                SqlConnection con = new SqlConnection(cs);

                con.Open();


				// Check if the vehicle number already exists in the database
				string checkQuery = "SELECT COUNT(*) FROM Parking WHERE UPPER(VehicleNumber)=UPPER(@v) AND status='not paid'";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@v", textBox2.Text);
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Vehicle already added in database");
                    return;
				}


				// string query = "INSERT INTO Parking(username, VehicleNumber, EntryTime, slot) VALUES(@u,@v,@e,@s)";
				string query = "INSERT INTO Parking(username, ownername, VehicleNumber, EntryTime, slot, status) VALUES(@currentUser,@o,@v,@e,@s,@st)";
                string slotQuery = "SELECT VehicleCount FROM ParkingSlotCounter WHERE slot=@slot";

                SqlCommand slotCmd = new SqlCommand(slotQuery, con);
                slotCmd.Parameters.AddWithValue("@slot", comboBox1.Text);
                int slotCount = (int)slotCmd.ExecuteScalar();
                if(slotCount >= 20)
                {
                    MessageBox.Show("This slot is full");
                    return;
                }


				SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@o", textBox1.Text);

                cmd.Parameters.AddWithValue("@v", textBox2.Text.ToUpper());

                cmd.Parameters.AddWithValue("@e", DateTime.Now);

                cmd.Parameters.AddWithValue("@s", comboBox1.Text);

                cmd.Parameters.AddWithValue("@st", "not paid");

				cmd.Parameters.AddWithValue("@currentUser", username);

                string increaseSlotCountQuery = "Update ParkingSlotCounter SET VehicleCount = VehicleCount+1 Where slot = 'E'";
                SqlCommand slotIncreaseCmd = new SqlCommand(increaseSlotCountQuery, con);
                slotIncreaseCmd.ExecuteNonQuery();


				try
                {
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data Added Successfully");
                }
                catch (SqlException ex)
                {
					//MessageBox.Show("Vehicle already added in database");
					MessageBox.Show("Database Error: " + ex.Message);
				}


                // SHOW ONLY NEWLY ADDED DATA
                string showQuery = "SELECT * FROM Parking WHERE UPPER(VehicleNumber)=UPPER(@v) AND status='not paid'";

                SqlDataAdapter da = new SqlDataAdapter(showQuery, con);

                da.SelectCommand.Parameters.AddWithValue("@v", textBox2.Text);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dataGridView1.DataSource = dt;

                textBox1.Clear();
                textBox2.Clear();

                comboBox1.SelectedIndex = -1;

                con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e) // back button
        {
			if (username == "admin")
			{
				if (ParentAdmin != null)
				{
					ParentAdmin.Show(); // reuse existing instance
				}
				else
				{
					admin ad = new admin();
					ad.username = username;
					ad.Show();
				}
			}
			else
			{
				if (ParentDashboard != null)
				{
					ParentDashboard.Show(); // reuse existing instance
				}
				else
				{
					dashboard d = new dashboard();
					d.username = username;
					d.Show();
				}
			}

			this.Hide();
		}

		private void add_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("A");
            comboBox1.Items.Add("B");
            comboBox1.Items.Add("C");
            comboBox1.Items.Add("D");
            comboBox1.Items.Add("E");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
      

            
        }
    }
}
