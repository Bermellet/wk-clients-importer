using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WKClientsImporter.Views
{
    partial class TemplateFormatDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        private Label lblFormat;
        private ComboBox cbFormats;
        private Button btnOk;
        private Button btnCancel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblFormat = new System.Windows.Forms.Label();
            this.cbFormats = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Font = new System.Drawing.Font("Arial Rounded MT Bold", 7.8F);
            this.lblFormat.Location = new System.Drawing.Point(12, 15);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(58, 15);
            this.lblFormat.TabIndex = 0;
            this.lblFormat.Text = "Format:";
            // 
            // cbFormats
            // 
            this.cbFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFormats.Font = new System.Drawing.Font("Arial Rounded MT Bold", 7.8F);
            this.cbFormats.FormattingEnabled = true;
            this.cbFormats.Location = new System.Drawing.Point(78, 12);
            this.cbFormats.Name = "cbFormats";
            this.cbFormats.Size = new System.Drawing.Size(270, 23);
            this.cbFormats.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Font = new System.Drawing.Font("Arial Rounded MT Bold", 7.8F);
            this.btnOk.Location = new System.Drawing.Point(184, 60);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 25);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Arial Rounded MT Bold", 7.8F);
            this.btnCancel.Location = new System.Drawing.Point(265, 60);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // TemplateFormatDialog
            // 
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(360, 110);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.cbFormats);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 7.8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateFormatDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select template format";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}