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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.pbImport = new System.Windows.Forms.ProgressBar();
            this.lblProgressPercent = new System.Windows.Forms.Label();
            this.btnTemplate = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvClientes
            // 
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Location = new System.Drawing.Point(12, 85);
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.RowHeadersWidth = 51;
            this.dgvClientes.Size = new System.Drawing.Size(665, 465);
            this.dgvClientes.TabIndex = 0;
            // 
            // pbImport
            // 
            this.pbImport.Location = new System.Drawing.Point(125, 48);
            this.pbImport.Name = "pbImport";
            this.pbImport.Size = new System.Drawing.Size(400, 13);
            this.pbImport.TabIndex = 1;
            // 
            // lblProgressPercent
            // 
            this.lblProgressPercent.Location = new System.Drawing.Point(536, 42);
            this.lblProgressPercent.Name = "lblProgressPercent";
            this.lblProgressPercent.Size = new System.Drawing.Size(50, 23);
            this.lblProgressPercent.TabIndex = 3;
            this.lblProgressPercent.Text = "0%";
            this.lblProgressPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnTemplate
            // 
            this.btnTemplate.Image = global::WKClientsImporter.Properties.Resources.file_pen_solid_small;
            this.btnTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTemplate.Location = new System.Drawing.Point(281, 12);
            this.btnTemplate.Name = "btnTemplate";
            this.btnTemplate.Size = new System.Drawing.Size(150, 30);
            this.btnTemplate.TabIndex = 4;
            this.btnTemplate.Text = "Template";
            this.btnTemplate.UseVisualStyleBackColor = true;
            this.btnTemplate.Click += new System.EventHandler(this.btnTemplate_Click);
            // 
            // btnImport
            // 
            this.btnImport.Image = global::WKClientsImporter.Properties.Resources.file_circle_plus_solid_small;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(125, 12);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(150, 30);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(684, 561);
            this.Controls.Add(this.btnTemplate);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblProgressPercent);
            this.Controls.Add(this.pbImport);
            this.Controls.Add(this.dgvClientes);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WK-Cliente-Importer";
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.ProgressBar pbImport;
        private System.Windows.Forms.Label lblProgressPercent;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnTemplate;
    }
}