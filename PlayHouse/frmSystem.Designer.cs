namespace PlayHouse
{
    partial class frmSystem
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cmbAdminMovies = new System.Windows.Forms.ComboBox();
            this.btnAdminUpdate = new System.Windows.Forms.Button();
            this.picMoviePoster = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMoviePoster)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "Time";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Times New Roman", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(12, 389);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(49, 24);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Title";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(262, 213);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 31);
            this.button1.TabIndex = 0;
            this.button1.Text = "Book Now";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.Enabled = false;
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            this.historyToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.historyToolStripMenuItem.Text = "History";
            this.historyToolStripMenuItem.Click += new System.EventHandler(this.historyToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historyToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(384, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cmbAdminMovies
            // 
            this.cmbAdminMovies.FormattingEnabled = true;
            this.cmbAdminMovies.Location = new System.Drawing.Point(248, 95);
            this.cmbAdminMovies.Name = "cmbAdminMovies";
            this.cmbAdminMovies.Size = new System.Drawing.Size(120, 21);
            this.cmbAdminMovies.TabIndex = 9;
            // 
            // btnAdminUpdate
            // 
            this.btnAdminUpdate.Location = new System.Drawing.Point(256, 130);
            this.btnAdminUpdate.Name = "btnAdminUpdate";
            this.btnAdminUpdate.Size = new System.Drawing.Size(103, 27);
            this.btnAdminUpdate.TabIndex = 10;
            this.btnAdminUpdate.Text = "CONFIRM";
            this.btnAdminUpdate.UseVisualStyleBackColor = true;
            this.btnAdminUpdate.Click += new System.EventHandler(this.btnAdminUpdate_Click);
            // 
            // picMoviePoster
            // 
            this.picMoviePoster.Location = new System.Drawing.Point(12, 66);
            this.picMoviePoster.Name = "picMoviePoster";
            this.picMoviePoster.Size = new System.Drawing.Size(218, 308);
            this.picMoviePoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picMoviePoster.TabIndex = 3;
            this.picMoviePoster.TabStop = false;
            // 
            // frmSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(384, 426);
            this.Controls.Add(this.btnAdminUpdate);
            this.Controls.Add(this.cmbAdminMovies);
            this.Controls.Add(this.picMoviePoster);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MaximumSize = new System.Drawing.Size(400, 465);
            this.MinimumSize = new System.Drawing.Size(400, 465);
            this.Name = "frmSystem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PlayHouse";
            this.Load += new System.EventHandler(this.frmSystem_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMoviePoster)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox picMoviePoster;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ComboBox cmbAdminMovies;
        private System.Windows.Forms.Button btnAdminUpdate;
    }
}