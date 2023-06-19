
namespace PersonalTracking
{
    partial class FrmDashboard
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
            this.grbDashboard = new System.Windows.Forms.GroupBox();
            this.btnExit = new FontAwesome.Sharp.IconButton();
            this.btnPermission = new FontAwesome.Sharp.IconButton();
            this.btnSalary = new FontAwesome.Sharp.IconButton();
            this.btnTask = new FontAwesome.Sharp.IconButton();
            this.btnPosition = new FontAwesome.Sharp.IconButton();
            this.btnDepartments = new FontAwesome.Sharp.IconButton();
            this.btnEmployee = new FontAwesome.Sharp.IconButton();
            this.pnlDashBoard = new System.Windows.Forms.Panel();
            this.grbDashboard.SuspendLayout();
            this.pnlDashBoard.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbDashboard
            // 
            this.grbDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(30)))), ((int)(((byte)(68)))));
            this.grbDashboard.Controls.Add(this.btnExit);
            this.grbDashboard.Controls.Add(this.btnPermission);
            this.grbDashboard.Controls.Add(this.btnSalary);
            this.grbDashboard.Controls.Add(this.btnTask);
            this.grbDashboard.Controls.Add(this.btnPosition);
            this.grbDashboard.Controls.Add(this.btnDepartments);
            this.grbDashboard.Controls.Add(this.btnEmployee);
            this.grbDashboard.Dock = System.Windows.Forms.DockStyle.Left;
            this.grbDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grbDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.grbDashboard.ForeColor = System.Drawing.Color.Snow;
            this.grbDashboard.Location = new System.Drawing.Point(0, 0);
            this.grbDashboard.Name = "grbDashboard";
            this.grbDashboard.Padding = new System.Windows.Forms.Padding(15);
            this.grbDashboard.Size = new System.Drawing.Size(235, 823);
            this.grbDashboard.TabIndex = 0;
            this.grbDashboard.TabStop = false;
            this.grbDashboard.Text = "Modules";
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnExit.IconChar = FontAwesome.Sharp.IconChar.CircleMinus;
            this.btnExit.IconColor = System.Drawing.Color.Crimson;
            this.btnExit.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnExit.IconSize = 32;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(15, 395);
            this.btnExit.Name = "btnExit";
            this.btnExit.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnExit.Size = new System.Drawing.Size(205, 60);
            this.btnExit.TabIndex = 13;
            this.btnExit.Text = "Exit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPermission
            // 
            this.btnPermission.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPermission.FlatAppearance.BorderSize = 0;
            this.btnPermission.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPermission.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnPermission.IconChar = FontAwesome.Sharp.IconChar.UserGear;
            this.btnPermission.IconColor = System.Drawing.Color.Crimson;
            this.btnPermission.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPermission.IconSize = 32;
            this.btnPermission.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPermission.Location = new System.Drawing.Point(15, 335);
            this.btnPermission.Name = "btnPermission";
            this.btnPermission.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnPermission.Size = new System.Drawing.Size(205, 60);
            this.btnPermission.TabIndex = 12;
            this.btnPermission.Text = "Permission";
            this.btnPermission.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPermission.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPermission.UseVisualStyleBackColor = true;
            this.btnPermission.Click += new System.EventHandler(this.btnPermission_Click);
            // 
            // btnSalary
            // 
            this.btnSalary.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSalary.FlatAppearance.BorderSize = 0;
            this.btnSalary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalary.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnSalary.IconChar = FontAwesome.Sharp.IconChar.MoneyBillTransfer;
            this.btnSalary.IconColor = System.Drawing.Color.DarkSlateGray;
            this.btnSalary.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSalary.IconSize = 32;
            this.btnSalary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalary.Location = new System.Drawing.Point(15, 275);
            this.btnSalary.Name = "btnSalary";
            this.btnSalary.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnSalary.Size = new System.Drawing.Size(205, 60);
            this.btnSalary.TabIndex = 11;
            this.btnSalary.Text = "Salary";
            this.btnSalary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalary.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSalary.UseVisualStyleBackColor = true;
            this.btnSalary.Click += new System.EventHandler(this.btnSalary_Click);
            // 
            // btnTask
            // 
            this.btnTask.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTask.FlatAppearance.BorderSize = 0;
            this.btnTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTask.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnTask.IconChar = FontAwesome.Sharp.IconChar.ListCheck;
            this.btnTask.IconColor = System.Drawing.Color.DarkGoldenrod;
            this.btnTask.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTask.IconSize = 32;
            this.btnTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTask.Location = new System.Drawing.Point(15, 215);
            this.btnTask.Name = "btnTask";
            this.btnTask.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnTask.Size = new System.Drawing.Size(205, 60);
            this.btnTask.TabIndex = 10;
            this.btnTask.Text = "Task";
            this.btnTask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTask.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTask.UseVisualStyleBackColor = true;
            this.btnTask.Click += new System.EventHandler(this.btnTask_Click);
            // 
            // btnPosition
            // 
            this.btnPosition.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPosition.FlatAppearance.BorderSize = 0;
            this.btnPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosition.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnPosition.IconChar = FontAwesome.Sharp.IconChar.AddressCard;
            this.btnPosition.IconColor = System.Drawing.Color.LightCoral;
            this.btnPosition.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPosition.IconSize = 32;
            this.btnPosition.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPosition.Location = new System.Drawing.Point(15, 155);
            this.btnPosition.Name = "btnPosition";
            this.btnPosition.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnPosition.Size = new System.Drawing.Size(205, 60);
            this.btnPosition.TabIndex = 9;
            this.btnPosition.Text = "Postion";
            this.btnPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPosition.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPosition.UseVisualStyleBackColor = true;
            this.btnPosition.Click += new System.EventHandler(this.btnPosition_Click);
            // 
            // btnDepartments
            // 
            this.btnDepartments.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDepartments.FlatAppearance.BorderSize = 0;
            this.btnDepartments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDepartments.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnDepartments.IconChar = FontAwesome.Sharp.IconChar.NetworkWired;
            this.btnDepartments.IconColor = System.Drawing.Color.Brown;
            this.btnDepartments.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDepartments.IconSize = 32;
            this.btnDepartments.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDepartments.Location = new System.Drawing.Point(15, 95);
            this.btnDepartments.Name = "btnDepartments";
            this.btnDepartments.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnDepartments.Size = new System.Drawing.Size(205, 60);
            this.btnDepartments.TabIndex = 8;
            this.btnDepartments.Text = "Department";
            this.btnDepartments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDepartments.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDepartments.UseVisualStyleBackColor = true;
            this.btnDepartments.Click += new System.EventHandler(this.btnDepartments_Click);
            // 
            // btnEmployee
            // 
            this.btnEmployee.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEmployee.FlatAppearance.BorderSize = 0;
            this.btnEmployee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmployee.ForeColor = System.Drawing.Color.Gainsboro;
            this.btnEmployee.IconChar = FontAwesome.Sharp.IconChar.Users;
            this.btnEmployee.IconColor = System.Drawing.Color.Aqua;
            this.btnEmployee.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnEmployee.IconSize = 32;
            this.btnEmployee.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmployee.Location = new System.Drawing.Point(15, 35);
            this.btnEmployee.Name = "btnEmployee";
            this.btnEmployee.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnEmployee.Size = new System.Drawing.Size(205, 60);
            this.btnEmployee.TabIndex = 7;
            this.btnEmployee.Text = "Employees";
            this.btnEmployee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmployee.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEmployee.UseVisualStyleBackColor = true;
            this.btnEmployee.Click += new System.EventHandler(this.btnEmployee_Click);
            // 
            // pnlDashBoard
            // 
            this.pnlDashBoard.Controls.Add(this.grbDashboard);
            this.pnlDashBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDashBoard.Location = new System.Drawing.Point(0, 0);
            this.pnlDashBoard.Name = "pnlDashBoard";
            this.pnlDashBoard.Size = new System.Drawing.Size(1248, 823);
            this.pnlDashBoard.TabIndex = 1;
            // 
            // FrmDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1248, 823);
            this.Controls.Add(this.pnlDashBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmDashboard";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dashboard";
            this.grbDashboard.ResumeLayout(false);
            this.pnlDashBoard.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbDashboard;
        private FontAwesome.Sharp.IconButton btnPermission;
        private FontAwesome.Sharp.IconButton btnSalary;
        private FontAwesome.Sharp.IconButton btnTask;
        private FontAwesome.Sharp.IconButton btnPosition;
        private FontAwesome.Sharp.IconButton btnDepartments;
        private FontAwesome.Sharp.IconButton btnEmployee;
        private System.Windows.Forms.Panel pnlDashBoard;
        private FontAwesome.Sharp.IconButton btnExit;
    }
}