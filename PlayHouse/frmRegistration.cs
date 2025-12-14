using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayHouse
{
    public partial class frmRegistration : Form
    {
        private const string ConnectionString = "Server=localhost; Database=PlayHouseDB; Integrated Security=True;";
        private string _currentUserRole;

        // Validation Constants
        private const int MAX_PASSWORD_LENGTH = 18;
        private const int MIN_AGE = 12;
        private const int MAX_AGE = 120;
        private const string USERNAME_PATTERN = @"^[a-zA-Z0-9._-]+$";

        // CONSTRUCTOR
        public frmRegistration(string currentUserRole = "Guest")
        {
            InitializeComponent();
            _currentUserRole = currentUserRole;


            SetupRoleVisibility();
        }

        private void SetupRoleVisibility()
        {
            //Reset Items
            cmbRoles.Items.Clear();
            cmbRoles.Items.Add("Customer");
            cmbRoles.Items.Add("Admin");
            cmbRoles.SelectedIndex = 0;

            // CHECK WHO OPENS THE FORM
            if (_currentUserRole == "Admin")
            {
                // Only Admin can see this
                cmbRoles.Visible = true;
                cmbRoles.Enabled = true;
                lblRole.Visible = true;
                this.Text = "Admin User Creation";
            }
            else
            {
                lblRole.Visible = false;
                cmbRoles.Visible = false;
                cmbRoles.Enabled = false;
                this.Text = "New Member Registration";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput()) return;

            // LOGIC FOR ROLE ASSIGNMENT
            int roleIDToInsert = 2;

            if (_currentUserRole == "Admin")
            {
                if (cmbRoles.SelectedItem.ToString() == "Admin")
                    roleIDToInsert = 1;
            }

            RegisterUser(roleIDToInsert);
        }


        private bool ValidateInput()
        {
            if (txtPassword.Text.Length > MAX_PASSWORD_LENGTH) { MessageBox.Show("Password too long"); return false; }
            if (txtPassword.Text != txtPassword2.Text) { MessageBox.Show("Passwords mismatch"); return false; }
            if (!Regex.IsMatch(txtUsername.Text, USERNAME_PATTERN)) { MessageBox.Show("Invalid username"); return false; }
            if (string.IsNullOrWhiteSpace(txtAge.Text)) { MessageBox.Show("Age required"); return false; }
            if (int.TryParse(txtAge.Text, out int age))
            {
                if (age < MIN_AGE || age > MAX_AGE) { MessageBox.Show("Invalid Age"); return false; }
            }
            else { MessageBox.Show("Invalid Age"); return false; }
            return true;
        }

        private void RegisterUser(int roleID)
        {
            try
            {
                int userAge = int.Parse(txtAge.Text);
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    string query = @"INSERT INTO Users (RoleID, FirstName, LastName, UserAge, Username, Password, Email, AccountCreatedDate) 
                                     VALUES (@role, @fname, @lname, @age, @user, @pass, @email, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@role", roleID);
                        cmd.Parameters.AddWithValue("@fname", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@lname", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@age", userAge);
                        cmd.Parameters.AddWithValue("@user", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                        if (!string.IsNullOrWhiteSpace(txtEmail.Text)) cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        else cmd.Parameters.AddWithValue("@email", DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Account successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}