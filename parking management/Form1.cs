using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfFont = iTextSharp.text.Font; // Add this using directive at the top of the file to alias iTextSharp.text.Font
namespace parking_management
{
    public partial class Form1 : Form
    {
        public string username;
		public dashboard ParentDashboard { get; set; }
		public admin ParentAdmin { get; set; }

		bool isPlaceholder = true;
        string cs = @"Data Source=localhost\SQLEXPRESS01;Initial Catalog=userDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private DateTimePicker timePicker;

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(new string[] { "Bkash", "Nagad", "Card" });
            comboBox1.SelectedIndex = 0;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Visible = false; //***
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //***
            if(textBox1.Text == "")
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
            
            if(username == "admin")
			{
				query = @"SELECT VehicleNumber, ownername
                     FROM Parking
                     WHERE (status='not paid' OR status IS NULL) 
                     AND VehicleNumber LIKE @v
                     ORDER BY EntryTime DESC";
			}
            else{
				query = @"SELECT VehicleNumber, ownername
                     FROM Parking
                     WHERE (status='not paid' OR status IS NULL) AND username = @u
                     AND VehicleNumber LIKE @v
                     ORDER BY EntryTime DESC";
			}

			using (SqlConnection con = new SqlConnection(cs))
			{
				SqlCommand cmd = new SqlCommand(query, con);

				cmd.Parameters.AddWithValue("@v", "%" + textBox1.Text + "%");
                if(username!= "admin")
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
			//***
		}

		private void enter_Click(object sender, EventArgs e)
        {
            string vehicle = textBox1.Text.ToUpper();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string query = @"SELECT TOP 1 *
                         FROM Parking
                         WHERE UPPER(VehicleNumber)=UPPER(@v)
                        AND (status='not paid' OR status IS NULL)
                         ORDER BY Id DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, con);

                da.SelectCommand.Parameters.AddWithValue("@v", vehicle);

                DataTable dt = new DataTable();

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;

					// Hide listbox after selection
                    listBox1.Visible = false;

					// Show exit time picker
					dateTimePicker1.Value = DateTime.Now;
                }
                else
                {
                    MessageBox.Show("Vehicle not found!");
                }
            }
        }


        private void UpdateExitTime()
        {
            string vehicle = textBox1.Text.ToUpper();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string query = @"UPDATE Parking
                         SET ExitTime=@exit
                        WHERE UPPER(VehicleNumber)=UPPER(@v)
                         AND (status='not paid' OR status IS NULL)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@exit", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@v", vehicle);

                cmd.ExecuteNonQuery();
            }
        }


        private void confirm_Click(object sender, EventArgs e)
        {
            string vehicle = textBox1.Text.ToUpper();

            // Remove BDT text
            string costText = textBoxTotalCost.Text.Replace("BDT", "").Trim();

			// Update DB exit time
			UpdateExitTime();

			using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string SlotQuery = @"SELECT slot from Parking WHERE UPPER(VehicleNumber)=UPPER(@v) AND (status='not paid' OR status IS NULL)";
                SqlCommand slotCmd = new SqlCommand(SlotQuery, con);
				slotCmd.Parameters.AddWithValue("@v", vehicle);
                string slot = (string)slotCmd.ExecuteScalar();

                // Decrease slot count after payment
                string decreaseSlotCntQuery = @"UPDATE ParkingSlotCounter SET VehicleCount = VehicleCount - 1 WHERE slot = @s";
                SqlCommand decreaseSlotCmd = new SqlCommand(decreaseSlotCntQuery, con);
				decreaseSlotCmd.Parameters.AddWithValue("@s", slot);
				decreaseSlotCmd.ExecuteNonQuery();

				string query = @"UPDATE Parking
                         SET TotalCost=@cost,
                             status='paid', PaymentMethod=@pm
                         WHERE UPPER(VehicleNumber)=UPPER(@v)
                         AND (status='not paid' OR status IS NULL)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@cost", costText);
                cmd.Parameters.AddWithValue("@pm", comboBox1.Text);
                cmd.Parameters.AddWithValue("@v", vehicle);

                int v = cmd.ExecuteNonQuery();

				cmd.ExecuteNonQuery();


                // Refresh Grid
                string refreshQuery = @"SELECT TOP 1 *
                                FROM Parking
                               WHERE UPPER(VehicleNumber)=UPPER(@v)
                                ORDER BY Id DESC";

                SqlDataAdapter da = new SqlDataAdapter(refreshQuery, con);

                da.SelectCommand.Parameters.AddWithValue("@v", vehicle);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }


			DialogResult result = MessageBox.Show(
					 "Payment Successful!\nDo you want to print receipt?",
					"Payment",
					  MessageBoxButtons.YesNo,
						MessageBoxIcon.Information);

			if (result == DialogResult.Yes)
			{
				GeneratePDF();
			}
			if (result == DialogResult.No)
			{

				if(username == "admin")
			    {
					admin ad = new admin();
					ad.Show();
					this.Hide();
					return;
				}
				dashboard d = new dashboard();
				d.username = username;

				d.Show();

				this.Hide();
				//this.Close();
			}


		}

        // Generate PDF Function
        private void GeneratePDF()
        {
            SaveFileDialog sfd = new SaveFileDialog(); // creates save as dialog box

            sfd.Filter = "PDF files (*.pdf)|*.pdf"; // user can only save as PDF
            sfd.FileName = "ParkingReceipt.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Document doc = new Document(PageSize.A4); // create virtual doc

                PdfWriter.GetInstance(doc,
                    new FileStream(sfd.FileName, FileMode.Create)); // convert virtual doc to real and save

                doc.Open();

                // TITLE
                Paragraph title = new Paragraph("PARKING PAYMENT RECEIPT");

                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20f;

                doc.Add(title);

                // TABLE
                PdfPTable table = new PdfPTable(2);

                table.WidthPercentage = 100;

                // USER NAME
                table.AddCell("User Name");
                table.AddCell(
                    dataGridView1.Rows[0]
                    .Cells["username"]
                    .Value.ToString());

                // Owner Name
                table.AddCell("Owner Name");
                table.AddCell(
                    dataGridView1.Rows[0]
                    .Cells["ownername"]
                    .Value.ToString());

                // VEHICLE NO
                table.AddCell("Vehicle No");
                table.AddCell(textBox1.Text);

                // SLOT
                table.AddCell("Parking Slot");
                table.AddCell(
                    dataGridView1.Rows[0]
                    .Cells["slot"]
                    .Value.ToString());

                // ENTRY TIME
                table.AddCell("Entry Time");
                table.AddCell(
                    Convert.ToDateTime(
                        dataGridView1.Rows[0]
                        .Cells["EntryTime"]
                        .Value)
                    .ToString("dd MMM yyyy hh:mm tt"));

                // EXIT TIME
                table.AddCell("Exit Time");
                table.AddCell(
                    dateTimePicker1.Value.ToString(
                        "dd MMM yyyy hh:mm tt"));

                // PAYMENT METHOD
                table.AddCell("Payment Method");
                table.AddCell(comboBox1.Text);

                // TOTAL COST
                table.AddCell("Total Cost");
                table.AddCell(textBoxTotalCost.Text + " TK");

                // PAYMENT DATE
                table.AddCell("Payment Date");
                table.AddCell(
                    DateTime.Now.ToString(
                        "dd MMM yyyy hh:mm tt"));

                doc.Add(table);

                // FOOTER
                Paragraph p = new Paragraph(
                    "\nThank You For Using Our Parking System");

                p.Alignment = Element.ALIGN_CENTER;

                doc.Add(p);

                doc.Close();

				DialogResult result = MessageBox.Show(
					"Receipt saved successfully!",
					"Receipt Saved",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);    

                if(result == DialogResult.OK)
                {
					if(username == "admin")
			        {
						admin ad = new admin();
						ad.Show();
						this.Hide();
						return;
					}
					dashboard d = new dashboard();
					d.username = username;

					d.Show();

					this.Hide();
					//this.Close();
				}
			}
        }



		// DateTimePicker value changed to set custom format
		private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            // Cast DateTimePicker before accessing Format
            DateTimePicker picker = sender as DateTimePicker;
            if (picker != null)
            {
                picker.Format = DateTimePickerFormat.Custom;
                picker.CustomFormat = "hh:mm:ss tt"; // 12 hr format with seconds
            }
        }
        private void calculate_Click(object sender, EventArgs e)
        {
            string vehicle = textBox1.Text.ToUpper();

            confirm.Enabled = true; // Enable confirm button when calculate is clicked

			using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                string query = @"SELECT TOP 1 EntryTime
                         FROM Parking
                         WHERE UPPER(VehicleNumber)=UPPER(@v)
                         AND (status='not paid' OR status IS NULL)
                         ORDER BY Id DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@v", vehicle);

                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Vehicle not found!");
                    return;
                }

                DateTime entryTime = Convert.ToDateTime(result);

                // User selected exit time
                DateTime exitTime = dateTimePicker1.Value;

                TimeSpan diff = exitTime - entryTime;

               // double hours = Math.Ceiling(diff.TotalHours);
                double hours =diff.TotalHours;

                double rate = (double)Properties.Settings.Default.ParkingRate;

                double total = hours * rate;

                //textBoxTotalCost.Text = total.ToString() + " BDT";
                textBoxTotalCost.Text = ((int)total).ToString() + " BDT";

                // Refresh grid
                string refreshQuery = @"SELECT TOP 1 *
                                FROM Parking
                                WHERE UPPER(VehicleNumber)=UPPER(@v)
                                ORDER BY Id DESC";

                SqlDataAdapter da = new SqlDataAdapter(refreshQuery, con);

                da.SelectCommand.Parameters.AddWithValue("@v", vehicle);

                DataTable dt = new DataTable();

                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Directly call the calculate_Click handler instead of trying to attach an event to 'calculate'
            calculate_Click(sender, e);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

		// Placeholder text handling for search box
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
            if (textBox1.Text == "")
            {
                textBox1.Text = "  e.g DHA-2626";
                textBox1.ForeColor = Color.Gray;
            }
        }

		// copy the suggestion to the search box when user clicks on the suggestion item in listbox
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                textBox1.Text = listBox1.SelectedItem.ToString();
			}
        }
	}
}
