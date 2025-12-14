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
    public partial class frmReservations : Form
    {
        private const string ConnectionString = "Server=localhost; Database=PlayHouseDB; Integrated Security=True;";

        // Note: We no longer need the private DataTable to hold all data, 
        // as we reload/refilter directly from the database.

        public frmReservations()
        {
            InitializeComponent();

            // The event handler is already attached in the designer/constructor
            // We ensure it points to the correct method below.
        }

        private void frmReservations_Load(object sender, EventArgs e)
        {
            // Initial load (with an empty search string)
            LoadAllReservations();
        }

        // Method to load and filter ALL reservation data directly from the database
        private void LoadAllReservations(string search = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    string sql = @"
                        SELECT 
                            R.TicketID, 
                            R.BookingTime, 
                            U.Username,               
                            SC.MovieTitle, 
                            SC.ShowTime, 
                            SC.ScreenName, 
                            S.SeatIdentifier, 
                            R.UserID                  
                        FROM TBL_RESERVATION R
                        INNER JOIN TBL_SCREENING SC ON R.ScreeningID = SC.ScreeningID
                        INNER JOIN TBL_SEAT S ON R.SeatID = S.SeatID
                        LEFT JOIN Users U ON R.UserID = U.UserID -- Assuming your user table is TBL_USERS
                        WHERE 1=1";

                    // earch Filter Logic (SQL Injection Safe using parameters)
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        sql += @"
                            AND (
                                /* Cast TicketID (potentially GUID) to VARCHAR for LIKE search */
                                CAST(R.TicketID AS VARCHAR(50)) LIKE @Search 
                                OR U.Username LIKE @Search
                                OR SC.MovieTitle LIKE @Search
                                   OR SC.ScreenName LIKE @Search
                                OR S.SeatIdentifier LIKE @Search
                            )";
                    }

                    sql += " ORDER BY R.BookingTime DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (!string.IsNullOrWhiteSpace(search))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + search + "%");
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["Username"] == DBNull.Value)
                            {
                                row["Username"] = "Guest";
                            }
                        }

                        if (dt.Rows.Count == 0 && string.IsNullOrWhiteSpace(search))
                        {
                            MessageBox.Show("There are no reservations in the system.", "No Data Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        dataGridView1.DataSource = dt;
                        FormatGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reservations: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadAllReservations(txtSearch.Text.Trim());
        }

        private void FormatGrid()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                // Rename headers for readability
                dataGridView1.Columns["TicketID"].HeaderText = "Ticket Ref";
                dataGridView1.Columns["BookingTime"].HeaderText = "Date Booked";
                dataGridView1.Columns["Username"].HeaderText = "Customer";
                dataGridView1.Columns["MovieTitle"].HeaderText = "Movie";
                dataGridView1.Columns["ShowTime"].HeaderText = "Show Time";
                dataGridView1.Columns["ScreenName"].HeaderText = "Cinema";
                dataGridView1.Columns["SeatIdentifier"].HeaderText = "Seat No.";

                // Hide the raw UserID column 
                dataGridView1.Columns["UserID"].Visible = false;

                // Auto-size columns to fit content
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }
    }
}
