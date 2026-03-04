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
            this.btnCsvTemplate = new System.Windows.Forms.Button();
            this.btnJsonTemplate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCustomers
            // 
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomers.Location = new System.Drawing.Point(12, 89);
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.RowHeadersWidth = 51;
            this.dgvCustomers.Size = new System.Drawing.Size(665, 414);
            this.dgvCustomers.TabIndex = 0;
            // 
            // pbImport
            // 
            this.pbImport.Location = new System.Drawing.Point(126, 70);
            this.pbImport.Name = "pbImport";
            this.pbImport.Size = new System.Drawing.Size(430, 13);
            this.pbImport.TabIndex = 1;
            // 
            // btnImportCsv
            // 
            this.btnImportCsv.Location = new System.Drawing.Point(126, 12);
            this.btnImportCsv.Name = "btnImportCsv";
            this.btnImportCsv.Size = new System.Drawing.Size(115, 23);
            this.btnImportCsv.TabIndex = 2;
            this.btnImportCsv.Text = "Import CSV";
            this.btnImportCsv.UseVisualStyleBackColor = true;
            this.btnImportCsv.Click += new System.EventHandler(this.btnImportCsv_Click);
            // 
            // btnImportJson
            // 
            this.btnImportJson.Location = new System.Drawing.Point(126, 41);
            this.btnImportJson.Name = "btnImportJson";
            this.btnImportJson.Size = new System.Drawing.Size(115, 23);
            this.btnImportJson.TabIndex = 3;
            this.btnImportJson.Text = "Import JSON";
            this.btnImportJson.UseVisualStyleBackColor = true;
            this.btnImportJson.Click += new System.EventHandler(this.btnImportJson_Click);
            // 
            // btnCsvTemplate
            // 
            this.btnCsvTemplate.Location = new System.Drawing.Point(247, 12);
            this.btnCsvTemplate.Name = "btnCsvTemplate";
            this.btnCsvTemplate.Size = new System.Drawing.Size(115, 23);
            this.btnCsvTemplate.TabIndex = 4;
            this.btnCsvTemplate.Text = "CSV template";
            this.btnCsvTemplate.UseVisualStyleBackColor = true;
            this.btnCsvTemplate.Click += new System.EventHandler(this.btnCsvTemplate_Click);
            // 
            // btnJsonTemplate
            // 
            this.btnJsonTemplate.Location = new System.Drawing.Point(247, 41);
            this.btnJsonTemplate.Name = "btnJsonTemplate";
            this.btnJsonTemplate.Size = new System.Drawing.Size(115, 23);
            this.btnJsonTemplate.TabIndex = 5;
            this.btnJsonTemplate.Text = "JSON template";
            this.btnJsonTemplate.UseVisualStyleBackColor = true;
            this.btnJsonTemplate.Click += new System.EventHandler(this.btnJsonTemplate_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(699, 515);
            this.Controls.Add(this.btnJsonTemplate);
            this.Controls.Add(this.btnCsvTemplate);
            this.Controls.Add(this.btnImportJson);
            this.Controls.Add(this.btnImportCsv);
            this.Controls.Add(this.pbImport);
            this.Controls.Add(this.dgvCustomers);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainForm";
            this.Text = "WK-Customer-Importer";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.ProgressBar pbImport;
        private System.Windows.Forms.Button btnImportCsv;
        private System.Windows.Forms.Button btnImportJson;
        private System.Windows.Forms.Button btnCsvTemplate;
        private System.Windows.Forms.Button btnJsonTemplate;
    }
}