
namespace PersonalTracking.DesktopUI
{
    partial class FrmDepartmentList
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvDepartmentList = new System.Windows.Forms.DataGridView();
            this.PnlDepartmentList = new System.Windows.Forms.Panel();
            this.BtnNew = new System.Windows.Forms.Button();
            this.BtnUpdate = new System.Windows.Forms.Button();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.BtnBack = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartmentList)).BeginInit();
            this.PnlDepartmentList.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvDepartmentList
            // 
            this.dgvDepartmentList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvDepartmentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDepartmentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDepartmentList.Location = new System.Drawing.Point(0, 0);
            this.dgvDepartmentList.Name = "dgvDepartmentList";
            this.dgvDepartmentList.RowTemplate.Height = 25;
            this.dgvDepartmentList.Size = new System.Drawing.Size(1006, 784);
            this.dgvDepartmentList.TabIndex = 0;
            // 
            // PnlDepartmentList
            // 
            this.PnlDepartmentList.Controls.Add(this.BtnBack);
            this.PnlDepartmentList.Controls.Add(this.BtnDelete);
            this.PnlDepartmentList.Controls.Add(this.BtnUpdate);
            this.PnlDepartmentList.Controls.Add(this.BtnNew);
            this.PnlDepartmentList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlDepartmentList.Location = new System.Drawing.Point(0, 709);
            this.PnlDepartmentList.Name = "PnlDepartmentList";
            this.PnlDepartmentList.Size = new System.Drawing.Size(1006, 75);
            this.PnlDepartmentList.TabIndex = 1;
            // 
            // BtnNew
            // 
            this.BtnNew.Location = new System.Drawing.Point(88, 17);
            this.BtnNew.Name = "BtnNew";
            this.BtnNew.Size = new System.Drawing.Size(122, 46);
            this.BtnNew.TabIndex = 0;
            this.BtnNew.Text = "New";
            this.BtnNew.UseVisualStyleBackColor = true;
            // 
            // BtnUpdate
            // 
            this.BtnUpdate.Location = new System.Drawing.Point(234, 17);
            this.BtnUpdate.Name = "BtnUpdate";
            this.BtnUpdate.Size = new System.Drawing.Size(122, 46);
            this.BtnUpdate.TabIndex = 0;
            this.BtnUpdate.Text = "Update";
            this.BtnUpdate.UseVisualStyleBackColor = true;
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(373, 17);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(122, 46);
            this.BtnDelete.TabIndex = 0;
            this.BtnDelete.Text = "Delete";
            this.BtnDelete.UseVisualStyleBackColor = true;
            // 
            // BtnBack
            // 
            this.BtnBack.Location = new System.Drawing.Point(519, 17);
            this.BtnBack.Name = "BtnBack";
            this.BtnBack.Size = new System.Drawing.Size(122, 46);
            this.BtnBack.TabIndex = 0;
            this.BtnBack.Text = "Close";
            this.BtnBack.UseVisualStyleBackColor = true;
            // 
            // FrmDepartmentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 784);
            this.Controls.Add(this.PnlDepartmentList);
            this.Controls.Add(this.dgvDepartmentList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmDepartmentList";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Departments List";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDepartmentList)).EndInit();
            this.PnlDepartmentList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDepartmentList;
        private System.Windows.Forms.Panel PnlDepartmentList;
        private System.Windows.Forms.Button BtnBack;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnUpdate;
        private System.Windows.Forms.Button BtnNew;
    }
}

