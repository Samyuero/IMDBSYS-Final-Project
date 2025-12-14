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

namespace PlayHouse
{
    public partial class frmSystem : Form
    {
        private const string ConnectionString = "Server=localhost; Database=PlayHouseDB; Integrated Security=True;";

        // User Details
        private int CurrentUserID = 0;
        private string CurrentUserRole = "Guest";

        // Active Movie Details
        private int ActiveScreeningID = 0;
        private string ActiveMovieTitle = "";
        private DateTime ActiveShowTime;

        public frmSystem(int userID, string roleName)
        {
            InitializeComponent();
            this.CurrentUserID = userID;
            this.CurrentUserRole = roleName;
            this.Text = $"PlayHouse - {roleName}" + (userID != 0 ? $" (User ID: {userID})" : "");
        }

        private void LoadActiveMovie()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    // Read the saved ActiveScreeningID from the configuration table
                    string configQuery = "SELECT ConfigValue FROM TBL_CONFIG WHERE ConfigKey = 'ActiveScreeningID'";
                    using (SqlCommand cmd = new SqlCommand(configQuery, con))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int savedID))
                        {
                            ActiveScreeningID = savedID;
                        }
                        else
                        {
                            // Fallback if the config key is missing (set to 1)
                            ActiveScreeningID = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading active movie configuration: " + ex.Message);
                ActiveScreeningID = 1; // Safety fallback
            }

            LoadActiveMovieData(ActiveScreeningID);
        }

        private void frmSystem_Load(object sender, EventArgs e)
        {
            // Setup based on Role
            if (CurrentUserRole == "Admin")
            {
                MessageBox.Show("Welcome Admin!");
                // Show the admin tools you added in the designer
                cmbAdminMovies.Visible = true;
                btnAdminUpdate.Visible = true;
                addAccountToolStripMenuItem.Visible = true;
                reservationsToolStripMenuItem.Visible=true;
                LoadMoviesIntoAdminDropdown();
            }
            else
            {
                // Hide admin tools for Customers/Guests
                cmbAdminMovies.Visible = false;
                btnAdminUpdate.Visible = false;
                addAccountToolStripMenuItem.Visible = false;
                reservationsToolStripMenuItem.Visible = false;

                if (CurrentUserRole == "Customer")
                {
                    MessageBox.Show("Welcome Customer!");
                    if (historyToolStripMenuItem != null) historyToolStripMenuItem.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Welcome Guest!");
                    if (historyToolStripMenuItem != null) historyToolStripMenuItem.Visible = false;
                }
            }

            // Load the default/current movie (Starts with the first one found)
            LoadActiveMovie();
        }

        // ADMIN FUNCTIONALITY
        private void LoadMoviesIntoAdminDropdown()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT ScreeningID, MovieTitle FROM TBL_SCREENING", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbAdminMovies.DataSource = dt;
                    cmbAdminMovies.DisplayMember = "MovieTitle";
                    cmbAdminMovies.ValueMember = "ScreeningID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading movie list: " + ex.Message);
            }
        }

        private void btnAdminUpdate_Click(object sender, EventArgs e)
        {
            if (cmbAdminMovies.SelectedValue == null) return;

            int selectedID = Convert.ToInt32(cmbAdminMovies.SelectedValue);
            string selectedTitle = cmbAdminMovies.Text;

            DialogResult result = MessageBox.Show(
                $"Change active movie to '{selectedTitle}'?\n\nWARNING: This will RESET (delete) all reservations for this movie!",
                "Confirm Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ResetReservations(selectedID);

                // PERSIST THE CHANGE TO THE DATABASE
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string updateQuery = @"
                                        UPDATE TBL_CONFIG 
                                        SET ConfigValue = @id 
                                        WHERE ConfigKey = 'ActiveScreeningID'";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedID);
                        cmd.ExecuteNonQuery();
                    }
                }

                ActiveScreeningID = selectedID; //Update memory variable
                ActiveMovieTitle = selectedTitle;

                LoadActiveMovieData(selectedID); // Refresh UI
                MessageBox.Show("Movie updated and seats reset.");
            }
        }

        private void ResetReservations(int screeningID)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string query = "DELETE FROM TBL_RESERVATION WHERE ScreeningID = @id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", screeningID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // CORE SYSTEM FUNCTIONALITY
        private void LoadActiveMovieData(int id)
        {
            if (id == 0) return;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string query = "SELECT MovieTitle, ShowTime FROM TBL_SCREENING WHERE ScreeningID = @id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ActiveMovieTitle = reader["MovieTitle"].ToString();
                        ActiveShowTime = Convert.ToDateTime(reader["ShowTime"]);

                        // Update UI Labels
                        lblName.Text = ActiveMovieTitle;
                        label1.Text = ActiveShowTime.ToString("dd MMM yyyy, h:mm tt");

                        // Update Image
                        UpdateMovieImage(ActiveMovieTitle);
                    }
                }
            }
        }

        private void UpdateMovieImage(string title)
        {
            try
            {
                if (title == "Les Misérables")
                {
                    picMoviePoster.Image = Properties.Resources.LesMis;
                    // Uncomment above line after adding image
                    picMoviePoster.BackColor = Color.Blue; // Placeholder
                }
                else if (title == "Aladdin")
                {
                    picMoviePoster.Image = Properties.Resources.Aladdin;
                    picMoviePoster.BackColor = Color.Gold; // Placeholder
                }
                else if (title == "The Phantom of the Opera")
                {
                    picMoviePoster.Image = Properties.Resources.Phantom;
                    picMoviePoster.BackColor = Color.Black; // Placeholder
                }
                else
                {
                    picMoviePoster.Image = null;
                    picMoviePoster.BackColor = Color.Gray;
                }
            }
            catch
            {
                // Fallback if image fails
                picMoviePoster.BackColor = Color.Red;
            }
        }

        // 'Book Now' button
        private void button1_Click(object sender, EventArgs e)
        {
            if (ActiveScreeningID == 0)
            {
                MessageBox.Show("System Error: No active movie selected.");
                return;
            }

            // Open the booking form with the CURRENT active movie ID
            frmMov1 movieForm = new frmMov1(ActiveScreeningID, CurrentUserID);
            movieForm.ShowDialog();
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHistory historyForm = new frmHistory(CurrentUserID, CurrentUserRole);
            historyForm.ShowDialog();
        }

        private void addAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRegistration reg = new frmRegistration("Admin");
            reg.ShowDialog();
        }

        private void reservationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReservations reservationsForm = new frmReservations();
            reservationsForm.ShowDialog();
        }

    }
}