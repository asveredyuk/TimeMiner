namespace TimeMiner.Slave.BuiltinExtensions
{
    partial class TaskExtensionForm
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
            this.tbShortDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbShortDescription
            // 
            this.tbShortDescription.Location = new System.Drawing.Point(12, 45);
            this.tbShortDescription.MaxLength = 32;
            this.tbShortDescription.Name = "tbShortDescription";
            this.tbShortDescription.Size = new System.Drawing.Size(313, 20);
            this.tbShortDescription.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ShortDescription";
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(250, 80);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 2;
            this.btOk.Text = "Ok";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // TaskExtensionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 115);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbShortDescription);
            this.Name = "TaskExtensionForm";
            this.Text = "TaskExtensionForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbShortDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btOk;
    }
}