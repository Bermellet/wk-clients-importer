namespace WKClientsImporter.Views
{
    partial class MainForm
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
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.pbImport = new System.Windows.Forms.ProgressBar();
            this.btnImportCsv = new System.Windows.Forms.Button();
            this.btnImportJson = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCustomers
            // 
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomers.Location = new System.Drawing.Point(156, 41);
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.Size = new System.Drawing.Size(430, 253);
            this.dgvCustomers.TabIndex = 0;
            // 
            // pbImport
            // 
            this.pbImport.Location = new System.Drawing.Point(156, 300);
            this.pbImport.Name = "pbImport";
            this.pbImport.Size = new System.Drawing.Size(430, 27);
            this.pbImport.TabIndex = 1;
            // 
            // btnImportCsv
            // 
            this.btnImportCsv.Location = new System.Drawing.Point(511, 333);
            this.btnImportCsv.Name = "btnImportCsv";
            this.btnImportCsv.Size = new System.Drawing.Size(75, 23);
            this.btnImportCsv.TabIndex = 2;
            this.btnImportCsv.Text = "Import CSV";
            this.btnImportCsv.UseVisualStyleBackColor = true;
            this.btnImportCsv.Click += new System.EventHandler(this.btnImportCsv_Click);
            // 
            // btnImportJson
            // 
            this.btnImportJson.Location = new System.Drawing.Point(511, 362);
            this.btnImportJson.Name = "btnImportJson";
            this.btnImportJson.Size = new System.Drawing.Size(75, 23);
            this.btnImportJson.TabIndex = 3;
            this.btnImportJson.Text = "Import JSON";
            this.btnImportJson.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(699, 515);
            this.Controls.Add(this.btnImportJson);
            this.Controls.Add(this.btnImportCsv);
            this.Controls.Add(this.pbImport);
            this.Controls.Add(this.dgvCustomers);
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.ProgressBar pbImport;
        private System.Windows.Forms.Button btnImportCsv;
        private System.Windows.Forms.Button btnImportJson;
    }
}