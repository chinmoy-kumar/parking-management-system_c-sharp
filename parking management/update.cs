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
    public partial class update : Form
    {
        public  string username;
		public dashboard ParentDashboard { get; set; }
		public admin ParentAdmin { get; set; }
		string cs = @"Data Source=localhost\SQLEXPRESS01;Initial Catalog=userDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public update()
        {
            InitializeComponent();
        }

        private void update_Load(object sender, EventArgs e)
        {
			listBox1.Visible = false;
		}

        private void button1_Click(object sender, EventArgs e) // search button
		{
            SqlConnection con = new SqlConnection(cs);
            if(username == "admin")
			{
				string adminQquery = "SELECT * FROM Parking WHERE UPPER(VehicleNumber)=UPPER(@v)";

				SqlDataAdapter da1 = new SqlDataAdapter(adminQquery, con);

				// SEARCH TEXTBOX
				da1.SelectCommand.Parameters.AddWithValue("@v", textBox4.Text);

				DataTable dt1 = new DataTable();

				da1.Fill(dt1);

				dataGridView1.DataSource = dt1;

				if (dt1.Rows.Count > 0)
				{
					// SHOW DATA IN UPDATE FIELDS
					textBox1.Text = dt1.Rows[0]["ownername"].ToString();

					dateTimePicker1.Text = dt1.Rows[0]["EntryTime"].ToString();

					//comboBox1.Text = dt1.Rows[0]["slot"].ToString();
					tbxVehicleNumber.Text = dt1.Rows[0]["VehicleNumber"].ToString();
				}
				else
				{
					MessageBox.Show("Vehicle Not Found");
				}
				
			}
            else{
				string query = "SELECT * FROM Parking WHERE UPPER(VehicleNumber)=UPPER(@v) AND username = @usr";

				SqlDataAdapter da = new SqlDataAdapter(query, con);

				// SEARCH TEXTBOX
				da.SelectCommand.Parameters.AddWithValue("@v", textBox4.Text);
				da.SelectCommand.Parameters.AddWithValue("@usr", username);

				DataTable dt = new DataTable();

				da.Fill(dt);

				dataGridView1.DataSource = dt;

				if (dt.Rows.Count > 0)
				{
					// SHOW DATA IN UPDATE FIELDS
					textBox1.Text = dt.Rows[0]["ownername"].ToString();

					dateTimePicker1.Text = dt.Rows[0]["EntryTime"].ToString();

					//comboBox1.Text = dt.Rows[0]["slot"].ToString();
					tbxVehicleNumber.Text = dt.Rows[0]["VehicleNumber"].ToString();

					// Hide listbox after selection
					listBox1.Visible = false;
				}
				else
				{
					MessageBox.Show("Vehicle Not Found");
				}
			}


        }

        private void button2_Click(object sender, EventArgs e) // update button
		{
            if (textBox1.Text == "" || dateTimePicker1.Text == "" ||
         tbxVehicleNumber.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Please Fill All Fields");
            }

            else
            {
                SqlConnection con = new SqlConnection(cs);

                con.Open();

                string query = "UPDATE Parking SET ownername = @u, VehicleNumber = @vn, EntryTime = @e, ExitTime = NULL, TotalCost = NULL, status = NULL WHERE VehicleNumber = @v";

                SqlCommand cmd = new SqlCommand(query, con);

                // OWNER NAME
                cmd.Parameters.AddWithValue("@u", textBox1.Text);

                // ENTRY TIME
                cmd.Parameters.AddWithValue("@e", dateTimePicker1.Value);

                // SLOT
                cmd.Parameters.AddWithValue("@vn", tbxVehicleNumber.Text);

                // SEARCH VEHICLE
                cmd.Parameters.AddWithValue("@v", textBox4.Text);

                cmd.ExecuteNonQuery();

				DialogResult result = MessageBox.Show(
					"Data Updated Successfully",
					"Data Updated",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);

				if (result == DialogResult.OK)
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

				// SHOW UPDATED DATA
				string showQuery = "SELECT * FROM Parking WHERE VehicleNumber=@v";

                SqlDataAdapter da = new SqlDataAdapter(showQuery, con);

                da.SelectCommand.Parameters.AddWithValue("@v", textBox4.Text);
                da.SelectCommand.Parameters.AddWithValue("@u", username);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dataGridView1.DataSource = dt;

                con.Close();
            }

        }

        private void button3_Click(object sender, EventArgs e) // back button
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

		private void textBox4_TextChanged(object sender, EventArgs e)
		{
			if (textBox4.Text == "")
			{
				// clear listbox and hide
				listBox1.Items.Clear();
				listBox1.Visible = false;
				return;

			}

			// show listbox
			listBox1.Visible = true;
			listBox1.Items.Clear();

			string query = "";

			if (username == "admin")
			{
				query = @"SELECT VehicleNumber, ownername
                     FROM Parking
                     WHERE (status='not paid' OR status IS NULL) 
                     AND VehicleNumber LIKE @v
                     ORDER BY EntryTime DESC";
			}
			else
			{
				query = @"SELECT VehicleNumber, ownername
                     FROM Parking
                     WHERE (status='not paid' OR status IS NULL) AND username = @u
                     AND VehicleNumber LIKE @v
                     ORDER BY EntryTime DESC";
			}

			using (SqlConnection con = new SqlConnection(cs))
			{
				SqlCommand cmd = new SqlCommand(query, con);

				cmd.Parameters.AddWithValue("@v", "%" + textBox4.Text + "%");
				if (username != "admin")
				{
					cmd.Parameters.AddWithValue("@u", username);
				}
				con.Open();

				SqlDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					listBox1.Items.Add(
						dr["VehicleNumber"].ToString() + " - " + dr["ownername"].ToString()
					);
				}
			}

			if (listBox1.Items.Count > 0)
			{
				listBox1.Visible = true;
			}
			else
			{
				listBox1.Visible = false;
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedItem != null)
			{
				textBox1.Text = listBox1.SelectedItem.ToString();
			}
		}
	}
}
