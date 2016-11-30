namespace UniversityOfWestminster.MsCrm.DataExporter
{
    partial class MainUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUserControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGo = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialogCSV = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialogXML = new System.Windows.Forms.SaveFileDialog();
            this.pageDatatable = new System.Windows.Forms.TabPage();
            this.grid = new System.Windows.Forms.DataGridView();
            this.pageRequest = new System.Windows.Forms.TabPage();
            this.txtRequest = new System.Windows.Forms.RichTextBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.pageOptions = new System.Windows.Forms.TabPage();
            this.toolStripMenu.SuspendLayout();
            this.pageDatatable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.pageRequest.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.toolStripSeparator2,
            this.tsbGo,
            this.tsbSave});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripMenu.Size = new System.Drawing.Size(522, 27);
            this.toolStripMenu.TabIndex = 5;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Image = ((System.Drawing.Image)(resources.GetObject("tsbClose.Image")));
            this.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(24, 24);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbGo
            // 
            this.tsbGo.Image = ((System.Drawing.Image)(resources.GetObject("tsbGo.Image")));
            this.tsbGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGo.Name = "tsbGo";
            this.tsbGo.Size = new System.Drawing.Size(71, 24);
            this.tsbGo.Text = "Execute";
            this.tsbGo.ToolTipText = "Perfomrs a Who I Am request";
            this.tsbGo.Click += new System.EventHandler(this.tsbWhoAmI_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(55, 24);
            this.tsbSave.Text = "Save";
            this.tsbSave.Click += new System.EventHandler(this.asCSVToolStripMenuItem_Click);
            // 
            // saveFileDialogCSV
            // 
            this.saveFileDialogCSV.DefaultExt = "csv";
            this.saveFileDialogCSV.Filter = "CSV files|*.csv|All files|*.*";
            // 
            // saveFileDialogXML
            // 
            this.saveFileDialogXML.DefaultExt = "xml";
            this.saveFileDialogXML.Filter = "XML files|*.xml|All files|*.*";
            this.saveFileDialogXML.Title = "Save result set";
            // 
            // pageDatatable
            // 
            this.pageDatatable.Controls.Add(this.grid);
            this.pageDatatable.Location = new System.Drawing.Point(4, 22);
            this.pageDatatable.Name = "pageDatatable";
            this.pageDatatable.Padding = new System.Windows.Forms.Padding(3);
            this.pageDatatable.Size = new System.Drawing.Size(514, 329);
            this.pageDatatable.TabIndex = 2;
            this.pageDatatable.Text = "Data";
            this.pageDatatable.UseVisualStyleBackColor = true;
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToOrderColumns = true;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(3, 3);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.Size = new System.Drawing.Size(508, 323);
            this.grid.TabIndex = 0;
            // 
            // pageRequest
            // 
            this.pageRequest.Controls.Add(this.txtRequest);
            this.pageRequest.Location = new System.Drawing.Point(4, 22);
            this.pageRequest.Name = "pageRequest";
            this.pageRequest.Padding = new System.Windows.Forms.Padding(3);
            this.pageRequest.Size = new System.Drawing.Size(514, 329);
            this.pageRequest.TabIndex = 0;
            this.pageRequest.Text = "Request";
            this.pageRequest.UseVisualStyleBackColor = true;
            // 
            // txtRequest
            // 
            this.txtRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRequest.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.txtRequest.Location = new System.Drawing.Point(3, 3);
            this.txtRequest.Name = "txtRequest";
            this.txtRequest.Size = new System.Drawing.Size(508, 323);
            this.txtRequest.TabIndex = 0;
            this.txtRequest.Text = resources.GetString("txtRequest.Text");
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.pageRequest);
            this.tcMain.Controls.Add(this.pageDatatable);
            this.tcMain.Controls.Add(this.pageOptions);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 27);
            this.tcMain.Multiline = true;
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(522, 355);
            this.tcMain.TabIndex = 6;
            // 
            // pageOptions
            // 
            this.pageOptions.Location = new System.Drawing.Point(4, 22);
            this.pageOptions.Name = "pageOptions";
            this.pageOptions.Padding = new System.Windows.Forms.Padding(3);
            this.pageOptions.Size = new System.Drawing.Size(514, 329);
            this.pageOptions.TabIndex = 3;
            this.pageOptions.Text = "Options";
            this.pageOptions.UseVisualStyleBackColor = true;
            // 
            // MainUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MainUserControl";
            this.Size = new System.Drawing.Size(522, 382);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.pageDatatable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.pageRequest.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbGo;
        private System.Windows.Forms.SaveFileDialog saveFileDialogCSV;
        private System.Windows.Forms.SaveFileDialog saveFileDialogXML;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.TabPage pageDatatable;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.TabPage pageRequest;
        private System.Windows.Forms.RichTextBox txtRequest;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage pageOptions;
    }
}
