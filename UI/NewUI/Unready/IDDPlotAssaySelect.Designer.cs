﻿namespace NewUI
{
    partial class IDDPlotAssaySelect
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.ItemIdLabel = new System.Windows.Forms.Label();
            this.DateLabel = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.MassLabel = new System.Windows.Forms.Label();
            this.DoublesRateLabel = new System.Windows.Forms.Label();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.HelpBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(15, 30);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(641, 394);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // ItemIdLabel
            // 
            this.ItemIdLabel.AutoSize = true;
            this.ItemIdLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemIdLabel.Location = new System.Drawing.Point(12, 9);
            this.ItemIdLabel.Name = "ItemIdLabel";
            this.ItemIdLabel.Size = new System.Drawing.Size(78, 18);
            this.ItemIdLabel.TabIndex = 1;
            this.ItemIdLabel.Text = "Item id";
            // 
            // DateLabel
            // 
            this.DateLabel.AutoSize = true;
            this.DateLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DateLabel.Location = new System.Drawing.Point(130, 9);
            this.DateLabel.Name = "DateLabel";
            this.DateLabel.Size = new System.Drawing.Size(48, 18);
            this.DateLabel.TabIndex = 2;
            this.DateLabel.Text = "Date";
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLabel.Location = new System.Drawing.Point(223, 9);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(48, 18);
            this.TimeLabel.TabIndex = 3;
            this.TimeLabel.Text = "Time";
            // 
            // MassLabel
            // 
            this.MassLabel.AutoSize = true;
            this.MassLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MassLabel.Location = new System.Drawing.Point(311, 9);
            this.MassLabel.Name = "MassLabel";
            this.MassLabel.Size = new System.Drawing.Size(178, 18);
            this.MassLabel.TabIndex = 4;
            this.MassLabel.Text = "Decl. Pu240e Mass";
            // 
            // DoublesRateLabel
            // 
            this.DoublesRateLabel.AutoSize = true;
            this.DoublesRateLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoublesRateLabel.Location = new System.Drawing.Point(548, 9);
            this.DoublesRateLabel.Name = "DoublesRateLabel";
            this.DoublesRateLabel.Size = new System.Drawing.Size(108, 18);
            this.DoublesRateLabel.TabIndex = 5;
            this.DoublesRateLabel.Text = "Dbls. Rate";
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(684, 30);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 6;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(684, 59);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 7;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // HelpBtn
            // 
            this.HelpBtn.Location = new System.Drawing.Point(684, 88);
            this.HelpBtn.Name = "HelpBtn";
            this.HelpBtn.Size = new System.Drawing.Size(75, 23);
            this.HelpBtn.TabIndex = 8;
            this.HelpBtn.Text = "Help";
            this.HelpBtn.UseVisualStyleBackColor = true;
            this.HelpBtn.Click += new System.EventHandler(this.HelpBtn_Click);
            // 
            // IDDPlotAssaySelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 436);
            this.Controls.Add(this.HelpBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.DoublesRateLabel);
            this.Controls.Add(this.MassLabel);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.DateLabel);
            this.Controls.Add(this.ItemIdLabel);
            this.Controls.Add(this.listBox1);
            this.Name = "IDDPlotAssaySelect";
            this.Text = "Select Verification Results to Plot";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label ItemIdLabel;
        private System.Windows.Forms.Label DateLabel;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.Label MassLabel;
        private System.Windows.Forms.Label DoublesRateLabel;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button HelpBtn;
    }
}