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
    public partial class frmHistory : Form
    {
        private const string ConnectionString = "Server=localhost; Database=PlayHouseDB; Integrated Security=True;";

        private int _currentUserID;
        private string _currentUserRole;

        public frmHistory(int userID, string role)
        {
            InitializeComponent();
            _currentUserID = userID;
            _currentUserRole = role;
        }

        private void LoadReservationHistory()
        {
            if (_currentUserID == 3)
            {
                // Use a more appropriate message and close the form, as a Guest shouldn't access history
                MessageBox.Show("History is not available for Guest accounts. Please log in or register.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    // SQL QUERY EXPLANATION:
                    // 1. Select specific columns we want to show.
                    // 2. FROM TBL_RESERVATION (Base table).
                    // 3. INNER JOIN TBL_SCREENING (To get Movie Title, ShowTime, ScreenName).
                    // 4. INNER JOIN TBL_SEAT (To get the SeatIdentifier like "A1").
                    // 5. WHERE UserID matches the logged-in user.

                    string query = @"
                        SELECT 
                            R.BookingTime, 
                            R.TicketID, 
                            SC.MovieTitle, 
                            SC.ShowTime, 
                            SC.ScreenName, 
                            S.SeatIdentifier
                        FROM TBL_RESERVATION R
                        INNER JOIN TBL_SCREENING SC ON R.ScreeningID = SC.ScreeningID
                        INNER JOIN TBL_SEAT S ON R.SeatID = S.SeatID
                        WHERE R.UserID = @uid
                        ORDER BY R.BookingTime DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@uid", _currentUserID);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = dt;
                            FormatGrid();
                        }
                        else
                        {
                            dataGridView1.DataSource = null;
                            MessageBox.Show("You have no reservations yet.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading history: " + ex.Message);
            }
        }

        private void FormatGrid()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                //Better readability
                dataGridView1.Columns["BookingTime"].HeaderText = "Date Booked";
                dataGridView1.Columns["TicketID"].HeaderText = "Ticket Ref";
                dataGridView1.Columns["MovieTitle"].HeaderText = "Movie";
                dataGridView1.Columns["ShowTime"].HeaderText = "Show Time";
                dataGridView1.Columns["ScreenName"].HeaderText = "Cinema";
                dataGridView1.Columns["SeatIdentifier"].HeaderText = "Seat No.";

                // Auto-sized columns to fit content
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void frmHistory_Load(object sender, EventArgs e)
        {
            LoadReservationHistory();
        }
    }
}
