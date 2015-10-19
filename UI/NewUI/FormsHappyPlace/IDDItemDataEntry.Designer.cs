﻿namespace NewUI
{
    partial class IDDItemDataEntry
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
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.HelpBtn = new System.Windows.Forms.Button();
            this.PrintBtn = new System.Windows.Forms.Button();
            this.DeleteBtn = new System.Windows.Forms.Button();
            this.ItemIdDataGrid = new System.Windows.Forms.DataGridView();
            this.ItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MBA = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.MatType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.IsoId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.StratId = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.InvChangeCode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.IOCode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Mass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HvyMetalUMass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HeavyMetalLen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ItemIdDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(1021, 12);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(96, 23);
            this.OKBtn.TabIndex = 1;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(1021, 49);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(96, 23);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // HelpBtn
            // 
            this.HelpBtn.Location = new System.Drawing.Point(1021, 86);
            this.HelpBtn.Name = "HelpBtn";
            this.HelpBtn.Size = new System.Drawing.Size(96, 23);
            this.HelpBtn.TabIndex = 3;
            this.HelpBtn.Text = "Help";
            this.HelpBtn.UseVisualStyleBackColor = true;
            this.HelpBtn.Click += new System.EventHandler(this.HelpBtn_Click);
            // 
            // PrintBtn
            // 
            this.PrintBtn.Location = new System.Drawing.Point(1021, 123);
            this.PrintBtn.Name = "PrintBtn";
            this.PrintBtn.Size = new System.Drawing.Size(96, 23);
            this.PrintBtn.TabIndex = 4;
            this.PrintBtn.Text = "Print";
            this.PrintBtn.UseVisualStyleBackColor = true;
            this.PrintBtn.Click += new System.EventHandler(this.PrintBtn_Click);
            // 
            // DeleteBtn
            // 
            this.DeleteBtn.Location = new System.Drawing.Point(1021, 160);
            this.DeleteBtn.Name = "DeleteBtn";
            this.DeleteBtn.Size = new System.Drawing.Size(96, 23);
            this.DeleteBtn.TabIndex = 5;
            this.DeleteBtn.Text = "Delete Items...";
            this.DeleteBtn.UseVisualStyleBackColor = true;
            this.DeleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // ItemIdDataGrid
            // 
            this.ItemIdDataGrid.AllowUserToDeleteRows = false;
            this.ItemIdDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ItemIdDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ItemId,
            this.MBA,
            this.MatType,
            this.IsoId,
            this.StratId,
            this.InvChangeCode,
            this.IOCode,
            this.Mass,
            this.HvyMetalUMass,
            this.HeavyMetalLen});
            this.ItemIdDataGrid.Location = new System.Drawing.Point(12, 12);
            this.ItemIdDataGrid.Name = "ItemIdDataGrid";
            this.ItemIdDataGrid.RowHeadersVisible = false;
            this.ItemIdDataGrid.Size = new System.Drawing.Size(1003, 460);
            this.ItemIdDataGrid.TabIndex = 6;
            this.ItemIdDataGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ItemIdDataGrid_CellContentClick);
            this.ItemIdDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ItemIdDataGrid_CellValueChanged);
            this.ItemIdDataGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.ItemIdDataGrid_DataError);
            this.ItemIdDataGrid.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.ItemIdDataGrid_UserAddedRow);
            // 
            // ItemId
            // 
            this.ItemId.HeaderText = "Item id";
            this.ItemId.Name = "ItemId";
            // 
            // MBA
            // 
            this.MBA.HeaderText = "Material Balance Area";
            this.MBA.Name = "MBA";
            // 
            // MatType
            // 
            this.MatType.HeaderText = "Material Type";
            this.MatType.Name = "MatType";
            // 
            // IsoId
            // 
            this.IsoId.HeaderText = "Isotopics id";
            this.IsoId.Name = "IsoId";
            // 
            // StratId
            // 
            this.StratId.HeaderText = "Stratum id";
            this.StratId.Name = "StratId";
            // 
            // InvChangeCode
            // 
            this.InvChangeCode.HeaderText = "Inv Change Code";
            this.InvChangeCode.Name = "InvChangeCode";
            // 
            // IOCode
            // 
            this.IOCode.HeaderText = "I/O Code";
            this.IOCode.Name = "IOCode";
            // 
            // Mass
            // 
            this.Mass.HeaderText = "Declared Mass (g)";
            this.Mass.Name = "Mass";
            // 
            // HvyMetalUMass
            // 
            this.HvyMetalUMass.HeaderText = "Heavy Metal Declared U Mass (g)";
            this.HvyMetalUMass.Name = "HvyMetalUMass";
            // 
            // HeavyMetalLen
            // 
            this.HeavyMetalLen.HeaderText = "Heavy Metal Length (cm)";
            this.HeavyMetalLen.Name = "HeavyMetalLen";
            // 
            // IDDItemDataEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 486);
            this.Controls.Add(this.ItemIdDataGrid);
            this.Controls.Add(this.DeleteBtn);
            this.Controls.Add(this.PrintBtn);
            this.Controls.Add(this.HelpBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Name = "IDDItemDataEntry";
            this.Text = "Enter Item Data";
            ((System.ComponentModel.ISupportInitialize)(this.ItemIdDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button HelpBtn;
        private System.Windows.Forms.Button PrintBtn;
        private System.Windows.Forms.Button DeleteBtn;
        private System.Windows.Forms.DataGridView ItemIdDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemId;
        private System.Windows.Forms.DataGridViewComboBoxColumn MBA;
        private System.Windows.Forms.DataGridViewComboBoxColumn MatType;
        private System.Windows.Forms.DataGridViewComboBoxColumn IsoId;
        private System.Windows.Forms.DataGridViewComboBoxColumn StratId;
        private System.Windows.Forms.DataGridViewComboBoxColumn InvChangeCode;
        private System.Windows.Forms.DataGridViewComboBoxColumn IOCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mass;
        private System.Windows.Forms.DataGridViewTextBoxColumn HvyMetalUMass;
        private System.Windows.Forms.DataGridViewTextBoxColumn HeavyMetalLen;
    }
}