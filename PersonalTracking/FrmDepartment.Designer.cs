using BLL.Interfaces;
using HLP.Interfaces;
using PersonalTracking.Models;

namespace PersonalTracking
{
    partial class FrmDepartment : IValidateHelper
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
            this.btnSave = new MaterialSkin.Controls.MaterialFlatButton();
            this.btnClose = new MaterialSkin.Controls.MaterialFlatButton();
            this.lblDepartment = new MaterialSkin.Controls.MaterialLabel();
            this.txtDepartment = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Depth = 0;
            this.btnSave.Icon = null;
            this.btnSave.Location = new System.Drawing.Point(182, 195);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.Primary = false;
            this.btnSave.Size = new System.Drawing.Size(55, 36);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.Depth = 0;
            this.btnClose.Icon = null;
            this.btnClose.Location = new System.Drawing.Point(291, 195);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnClose.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnClose.Name = "btnClose";
            this.btnClose.Primary = false;
            this.btnClose.Size = new System.Drawing.Size(63, 36);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblDepartment
            // 
            this.lblDepartment.AutoSize = true;
            this.lblDepartment.Depth = 0;
            this.lblDepartment.Font = new System.Drawing.Font("Roboto", 11F);
            this.lblDepartment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblDepartment.Location = new System.Drawing.Point(118, 124);
            this.lblDepartment.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblDepartment.Name = "lblDepartment";
            this.lblDepartment.Size = new System.Drawing.Size(87, 19);
            this.lblDepartment.TabIndex = 0;
            this.lblDepartment.Text = "Department";
            // 
            // txtDepartment
            // 
            this.txtDepartment.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtDepartment.Location = new System.Drawing.Point(231, 120);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Size = new System.Drawing.Size(213, 26);
            this.txtDepartment.TabIndex = 1;
            this.txtDepartment.TextChanged += new System.EventHandler(this.txtDepartment_TextChanged);
            // 
            // FrmDepartment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 246);
            this.Controls.Add(this.txtDepartment);
            this.Controls.Add(this.lblDepartment);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmDepartment";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create or Update Department";
            this.Load += new System.EventHandler(this.FrmDepartment_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public bool FieldValidate(bool condition)
        {
            return condition;
        }

        private bool GetFieldEmptyOrFilled()
        {
            var departmentIsEmpty = FieldValidate(string.IsNullOrEmpty(txtDepartment.Text) || 
                                                  string.IsNullOrWhiteSpace(txtDepartment.Text));
            return departmentIsEmpty;
        }

        private bool GetFieldLength()
        {
            var departmentAmountCharactersIsValid = FieldValidate(txtDepartment.Text.Length < 3);
            return departmentAmountCharactersIsValid;
        }

        private bool GetDepartmentFieldLengthAmount()
        {
            var departmentField = FieldValidate(txtDepartment.Text.Length >= 3);
            return departmentField;
        }

        private void UpdateDepartment(DepartmentModel department)
        {
            _departmentService.UpdateEntityService(department);
            _information.EntityUpdatedMessage(department.DepartmentModelName);
            ClearFields();
        }

        private void SaveDepartment(DepartmentModel department)
        {
            _departmentService.CreateEntityService(department);
            _information.EntitySavedWithSuccessMessage(department.DepartmentModelName);
            ClearFields();
        }

        #endregion
        private MaterialSkin.Controls.MaterialFlatButton btnSave;
        private MaterialSkin.Controls.MaterialFlatButton btnClose;
        private MaterialSkin.Controls.MaterialLabel lblDepartment;
        private System.Windows.Forms.TextBox txtDepartment;
        private IDepartmentService _departmentService;
        public DepartmentModel department;
        public bool isUpdate = false;
        private readonly IEntityMessages _information;
    }
}