using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayHouse
{
    public partial class frmSystem : Form
    {
        private const string ConnectionString = "Server=localhost; Database=PlayHouseDB; Integrated Security=True;";

        private int CurrentUserID = 0;
        private string CurrentUserRole = "Guest";

        private int LesMiserablesID = 0;
        private int AladdinID = 0;
        private DateTime LesMiserablesShowTime;
        private DateTime AladdinShowTime;

        public frmSystem(int userID, string roleName)
        {
            InitializeComponent();
            this.CurrentUserID = userID;
            this.CurrentUserRole = roleName;

            this.Text = $"PlayHouse - {roleName}" + (userID != 0 ? $" (User ID: {userID})" : "");
        }

        private void frmSystem_Load(object sender, EventArgs e)
        {
            LoadScreeningIDs();

            // Role-specific behavior
            if (CurrentUserRole == "Admin")
            {
                MessageBox.Show("Welcome Admin!");
                reportsToolStripMenuItem.Visible = true;
                reportsToolStripMenuItem.Enabled = true;
            }
            else if (CurrentUserRole == "Customer")
            {
                MessageBox.Show("Welcome Customer!");
                historyToolStripMenuItem.Enabled = true;
            }
            else // Guest
            {
                MessageBox.Show("Welcome Guest!");
            }
        }

        private void LoadScreeningIDs()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    string query = @"SELECT MovieTitle, ScreeningID, ShowTime 
                                     FROM TBL_SCREENING 
                                     WHERE MovieTitle IN ('Les Misérables', 'Aladdin')";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string title = reader["MovieTitle"].ToString();
                            int id = Convert.ToInt32(reader["ScreeningID"]);
                            DateTime showTime = Convert.ToDateTime(reader["ShowTime"]);

                            if (title == "Les Misérables")
                            {
                                LesMiserablesID = id;
                                LesMiserablesShowTime = showTime;
                            }
                            else if (title == "Aladdin")
                            {
                                AladdinID = id;
                                AladdinShowTime = showTime;
                            }
                        }
                    }
                }

                if (LesMiserablesID == 0 || AladdinID == 0)
                {
                    MessageBox.Show("Error: Could not find all movie screenings in the database.", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                UpdateMovieLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error during System Load: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateMovieLabels()
        {
            label1.Text = LesMiserablesID != 0 ? LesMiserablesShowTime.ToString("dd MMM yyyy, h:mm tt") : "Showtime N/A";
            lblName.Text = "Les Misérables";
        }

        private void button1_Click(object sender, EventArgs e) // Les Misérables
        {
            if (LesMiserablesID == 0)
            {
                MessageBox.Show("Screening data not available. Please restart.", "Error");
                return;
            }

            frmMov1 movieForm = new frmMov1(LesMiserablesID, CurrentUserID);
            movieForm.ShowDialog();

            LoadScreeningIDs();
        }

        private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}