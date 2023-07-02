using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    partial class FrmDepartmentList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvDepartment = new System.Windows.Forms.DataGridView();
            this.pnlDepartmentCrud = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartment)).BeginInit();
            this.pnlDepartmentCrud.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDepartment
            // 
            this.dgvDepartment.AllowUserToOrderColumns = true;
            this.dgvDepartment.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDepartment.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvDepartment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDepartment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDepartment.Location = new System.Drawing.Point(0, 0);
            this.dgvDepartment.Margin = new System.Windows.Forms.Padding(2);
            this.dgvDepartment.MultiSelect = false;
            this.dgvDepartment.Name = "dgvDepartment";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDepartment.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDepartment.RowTemplate.Height = 28;
            this.dgvDepartment.Size = new System.Drawing.Size(1022, 823);
            this.dgvDepartment.TabIndex = 0;
            this.dgvDepartment.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDeparments_RowEnter);
            // 
            // pnlDepartmentCrud
            // 
            this.pnlDepartmentCrud.Controls.Add(this.btnClose);
            this.pnlDepartmentCrud.Controls.Add(this.btnDelete);
            this.pnlDepartmentCrud.Controls.Add(this.btnUpdate);
            this.pnlDepartmentCrud.Controls.Add(this.btnNew);
            this.pnlDepartmentCrud.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDepartmentCrud.Location = new System.Drawing.Point(0, 762);
            this.pnlDepartmentCrud.Name = "pnlDepartmentCrud";
            this.pnlDepartmentCrud.Size = new System.Drawing.Size(1022, 61);
            this.pnlDepartmentCrud.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClose.Location = new System.Drawing.Point(658, 10);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(61, 32);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDelete.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDelete.Location = new System.Drawing.Point(494, 10);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(68, 32);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.AutoSize = true;
            this.btnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnUpdate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnUpdate.Location = new System.Drawing.Point(336, 10);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(74, 32);
            this.btnUpdate.TabIndex = 10;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnNew
            // 
            this.btnNew.AutoSize = true;
            this.btnNew.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnNew.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNew.Location = new System.Drawing.Point(207, 10);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(52, 32);
            this.btnNew.TabIndex = 10;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // FrmDepartmentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1022, 823);
            this.Controls.Add(this.pnlDepartmentCrud);
            this.Controls.Add(this.dgvDepartment);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "FrmDepartmentList";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Department List";
            this.Load += new System.EventHandler(this.FrmDepartmentList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartment)).EndInit();
            this.pnlDepartmentCrud.ResumeLayout(false);
            this.pnlDepartmentCrud.PerformLayout();
            this.ResumeLayout(false);

        }

        private void ConfigureColumns()
        {
            dgvDepartment.Columns[0].Width = 10;
            dgvDepartment.Columns[0].Visible = false;
            dgvDepartment.Columns[1].HeaderText = "Department Name";
            dgvDepartment.Columns[2].Visible = false;
            dgvDepartment.Columns[3].Visible = false;
        }

        private void dgvDeparments_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            departmentModel.DepartmentModelId = Convert.ToInt32(dgvDepartment.Rows[e.RowIndex].Cells[0].Value);
            departmentModel.DepartmentModelName = dgvDepartment.Rows[e.RowIndex].Cells[1].Value.ToString();
        }


        #endregion

        private System.Windows.Forms.DataGridView dgvDepartment;
        private System.Windows.Forms.Panel pnlDepartmentCrud;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnNew;
    }
}