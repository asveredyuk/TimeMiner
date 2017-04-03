namespace MasterDatabaseExplorer
{
    partial class Form1
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
            this.lbStatus = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.numExcelExportUserId = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btExcelExport = new System.Windows.Forms.Button();
            this.btImportOldLog = new System.Windows.Forms.Button();
            this.btLogStat = new System.Windows.Forms.Button();
            this.btClearDb = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btImportAppsList = new System.Windows.Forms.Button();
            this.btClearApps = new System.Windows.Forms.Button();
            this.btTest = new System.Windows.Forms.Button();
            this.btCountPerDay = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExcelExportUserId)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbStatus.Location = new System.Drawing.Point(13, 13);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(232, 24);
            this.lbStatus.TabIndex = 1;
            this.lbStatus.Text = "LogDatabase is not loaded";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(13, 41);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(693, 240);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.numExcelExportUserId);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btExcelExport);
            this.tabPage2.Controls.Add(this.btImportOldLog);
            this.tabPage2.Controls.Add(this.btCountPerDay);
            this.tabPage2.Controls.Add(this.btLogStat);
            this.tabPage2.Controls.Add(this.btClearDb);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(685, 214);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Logs";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // numExcelExportUserId
            // 
            this.numExcelExportUserId.Location = new System.Drawing.Point(182, 14);
            this.numExcelExportUserId.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numExcelExportUserId.Name = "numExcelExportUserId";
            this.numExcelExportUserId.Size = new System.Drawing.Size(120, 20);
            this.numExcelExportUserId.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "UserId";
            // 
            // btExcelExport
            // 
            this.btExcelExport.Location = new System.Drawing.Point(244, 44);
            this.btExcelExport.Name = "btExcelExport";
            this.btExcelExport.Size = new System.Drawing.Size(101, 30);
            this.btExcelExport.TabIndex = 3;
            this.btExcelExport.Text = "Export";
            this.btExcelExport.UseVisualStyleBackColor = true;
            this.btExcelExport.Click += new System.EventHandler(this.btExcelExport_Click);
            // 
            // btImportOldLog
            // 
            this.btImportOldLog.Location = new System.Drawing.Point(125, 44);
            this.btImportOldLog.Name = "btImportOldLog";
            this.btImportOldLog.Size = new System.Drawing.Size(113, 30);
            this.btImportOldLog.TabIndex = 0;
            this.btImportOldLog.Text = "Import old log";
            this.btImportOldLog.UseVisualStyleBackColor = true;
            this.btImportOldLog.Click += new System.EventHandler(this.btImportOldLog_Click);
            // 
            // btLogStat
            // 
            this.btLogStat.Location = new System.Drawing.Point(7, 44);
            this.btLogStat.Name = "btLogStat";
            this.btLogStat.Size = new System.Drawing.Size(113, 30);
            this.btLogStat.TabIndex = 0;
            this.btLogStat.Text = "Statistics";
            this.btLogStat.UseVisualStyleBackColor = true;
            this.btLogStat.Click += new System.EventHandler(this.btLogStat_Click);
            // 
            // btClearDb
            // 
            this.btClearDb.Location = new System.Drawing.Point(7, 7);
            this.btClearDb.Name = "btClearDb";
            this.btClearDb.Size = new System.Drawing.Size(113, 30);
            this.btClearDb.TabIndex = 0;
            this.btClearDb.Text = "Clear Logs";
            this.btClearDb.UseVisualStyleBackColor = true;
            this.btClearDb.Click += new System.EventHandler(this.btClearDb_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btImportAppsList);
            this.tabPage1.Controls.Add(this.btClearApps);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(685, 214);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btImportAppsList
            // 
            this.btImportAppsList.Location = new System.Drawing.Point(125, 6);
            this.btImportAppsList.Name = "btImportAppsList";
            this.btImportAppsList.Size = new System.Drawing.Size(113, 30);
            this.btImportAppsList.TabIndex = 1;
            this.btImportAppsList.Text = "Import apps list";
            this.btImportAppsList.UseVisualStyleBackColor = true;
            this.btImportAppsList.Click += new System.EventHandler(this.btImportAppsList_Click);
            // 
            // btClearApps
            // 
            this.btClearApps.Location = new System.Drawing.Point(6, 6);
            this.btClearApps.Name = "btClearApps";
            this.btClearApps.Size = new System.Drawing.Size(113, 30);
            this.btClearApps.TabIndex = 2;
            this.btClearApps.Text = "Clear settings database";
            this.btClearApps.UseVisualStyleBackColor = true;
            this.btClearApps.Click += new System.EventHandler(this.btClearApps_Click);
            // 
            // btTest
            // 
            this.btTest.Location = new System.Drawing.Point(627, 12);
            this.btTest.Name = "btTest";
            this.btTest.Size = new System.Drawing.Size(75, 23);
            this.btTest.TabIndex = 3;
            this.btTest.Text = "test";
            this.btTest.UseVisualStyleBackColor = true;
            this.btTest.Click += new System.EventHandler(this.btTest_Click);
            // 
            // btCountPerDay
            // 
            this.btCountPerDay.Location = new System.Drawing.Point(7, 178);
            this.btCountPerDay.Name = "btCountPerDay";
            this.btCountPerDay.Size = new System.Drawing.Size(113, 30);
            this.btCountPerDay.TabIndex = 0;
            this.btCountPerDay.Text = "Count per day ";
            this.btCountPerDay.UseVisualStyleBackColor = true;
            this.btCountPerDay.Click += new System.EventHandler(this.btCountPerDay_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 293);
            this.Controls.Add(this.btTest);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lbStatus);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExcelExportUserId)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btClearDb;
        private System.Windows.Forms.Button btImportOldLog;
        private System.Windows.Forms.Button btClearApps;
        private System.Windows.Forms.Button btImportAppsList;
        private System.Windows.Forms.Button btExcelExport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numExcelExportUserId;
        private System.Windows.Forms.Button btLogStat;
        private System.Windows.Forms.Button btTest;
        private System.Windows.Forms.Button btCountPerDay;
    }
}

