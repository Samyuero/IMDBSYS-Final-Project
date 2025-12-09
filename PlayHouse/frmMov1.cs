using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayHouse
{
    public partial class frmMov1 : Form
    {
        private const string ConnectionString = "Server=localhost; Database=PlayHouseDB; Integrated Security=True;";

        private decimal ticketPrice = 0.00m;
        private string movieTitle = "";
        private List<string> selectedSeats = new List<string>();

        public int CurrentUserID { get; private set; }
        private readonly int screeningID;

        public frmMov1(int id, int userID)
        {
            InitializeComponent();
            this.screeningID = id;
            this.CurrentUserID = userID;
        }

        private void frmMov1_Load(object sender, EventArgs e)
        {
            LoadMovieInfo();
            LoadReservedSeats();
            lblPrice.Text = $"Movie: {movieTitle} | Total: ₱{0.00:N2}";
        }

        private void LoadMovieInfo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string query = "SELECT MovieTitle, TicketPrice FROM TBL_SCREENING WHERE ScreeningID = @id";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", screeningID);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            movieTitle = reader["MovieTitle"].ToString();
                            ticketPrice = Convert.ToDecimal(reader["TicketPrice"]);
                        }
                        else
                        {
                            MessageBox.Show("Error: Invalid Screening ID provided.");
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading movie info: " + ex.Message);
            }
        }

        private void LoadReservedSeats()
        {
            List<string> takenSeats = new List<string>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string query = @"SELECT S.SeatIdentifier 
                                     FROM TBL_RESERVATION R
                                     INNER JOIN TBL_SEAT S ON R.SeatID = S.SeatID
                                     WHERE R.ScreeningID = @scrID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@scrID", screeningID);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            takenSeats.Add(reader["SeatIdentifier"].ToString());
                        }
                    }
                }

                foreach (Control c in this.Controls)
                {
                    // Check if the control is a button and its text contains the seat identifier pattern
                    if (c is Button btn && btn.Text.Contains("-"))
                    {
                        btn.Enabled = true;
                        btn.BackColor = SystemColors.Control;
                        if (takenSeats.Contains(btn.Text))
                        {
                            btn.Enabled = false;
                            btn.BackColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading seats: " + ex.Message);
            }
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || !btn.Enabled) return;

            string seatLabel = btn.Text;

            if (selectedSeats.Contains(seatLabel))
            {
                selectedSeats.Remove(seatLabel);
                btn.BackColor = SystemColors.Control;
            }
            else
            {
                selectedSeats.Add(seatLabel);
                btn.BackColor = Color.Green;
            }

            CalculateTotal();
        }

        private void CalculateTotal()
        {
            decimal totalAmount = selectedSeats.Count * ticketPrice;
            lblPrice.Text = $"Movie: {movieTitle} | Seats: {selectedSeats.Count} | Total: ₱{totalAmount:N2}";
        }

        private void reserveBtn_Click(object sender, EventArgs e)
        {
            if (selectedSeats.Count == 0)
            {
                MessageBox.Show("Please select at least one seat.");
                return;
            }

            // List to hold the generated GUIDs for messaging
            List<Guid> ticketRefs = new List<Guid>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    foreach (string seatLabel in selectedSeats)
                    {
                        Guid ticketID = Guid.NewGuid(); // Generate unique reference for reservation
                        ticketRefs.Add(ticketID);

                        string query = @"INSERT INTO TBL_RESERVATION (ScreeningID, SeatID, UserID, TicketID)
                                         VALUES (
                                             @scrID, 
                                             (SELECT SeatID FROM TBL_SEAT WHERE SeatIdentifier = @seatLabel), 
                                             @uID, 
                                             @ticketID
                                         )";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@scrID", screeningID);
                            cmd.Parameters.AddWithValue("@seatLabel", seatLabel);
                            cmd.Parameters.AddWithValue("@ticketID", ticketID);

                            // MODIFIED LOGIC: A UserID of 0 indicates a Guest/unregistered user.
                            if (CurrentUserID == 0)
                                cmd.Parameters.AddWithValue("@uID", DBNull.Value);
                            else
                                cmd.Parameters.AddWithValue("@uID", CurrentUserID);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Conditional messaging for Guest vs. Logged-in User
                string referenceMsg;
                if (CurrentUserID == 3)
                {
                    // Guest message showing all ticket IDs
                    referenceMsg = $"Guest Reservation Complete!\n\nReference Numbers:\n{string.Join("\n", ticketRefs)}\n\n" +
                                   $"Total Paid: ₱{(selectedSeats.Count * ticketPrice):N2}\n" +
                                   "*** PLEASE SCREENSHOT THESE IDs for entry. ***";
                }
                else
                {
                    // Logged-in user message
                    referenceMsg = $"Success! You booked {selectedSeats.Count} seats for {movieTitle}.\nTotal Paid: ₱{(selectedSeats.Count * ticketPrice):N2}";
                }

                MessageBox.Show(referenceMsg, "Reservation Complete");

                // Refresh the form state
                selectedSeats.Clear();
                LoadReservedSeats();
                lblPrice.Text = $"Movie: {movieTitle} | Total: ₱{0.00:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reservation Failed: " + ex.Message);
            }
        }

        //private void lblPrice_Click(object sender, EventArgs e)
        //{

        //}
    }
}