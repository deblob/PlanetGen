namespace PrcTest.UI
{
    partial class frmConfig
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
            this.lstValues = new System.Windows.Forms.ListView();
            this.clmnhName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmnhValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnApplyValue = new System.Windows.Forms.Button();
            this.txtOutputName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAnimate = new System.Windows.Forms.Button();
            this.btnResetAnimations = new System.Windows.Forms.Button();
            this.lblLastSeed = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnLoadSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstValues
            // 
            this.lstValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmnhName,
            this.clmnhValue});
            this.lstValues.FullRowSelect = true;
            this.lstValues.GridLines = true;
            this.lstValues.HideSelection = false;
            this.lstValues.Location = new System.Drawing.Point(12, 35);
            this.lstValues.MultiSelect = false;
            this.lstValues.Name = "lstValues";
            this.lstValues.Size = new System.Drawing.Size(473, 409);
            this.lstValues.TabIndex = 0;
            this.lstValues.UseCompatibleStateImageBehavior = false;
            this.lstValues.View = System.Windows.Forms.View.Details;
            this.lstValues.SelectedIndexChanged += new System.EventHandler(this.lstValues_SelectedIndexChanged);
            // 
            // clmnhName
            // 
            this.clmnhName.Text = "Name";
            this.clmnhName.Width = 312;
            // 
            // clmnhValue
            // 
            this.clmnhValue.Text = "Value";
            this.clmnhValue.Width = 157;
            // 
            // pnlContainer
            // 
            this.pnlContainer.Location = new System.Drawing.Point(91, 510);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(230, 40);
            this.pnlContainer.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(410, 580);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Generate";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(12, 448);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(473, 60);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "description";
            // 
            // btnApplyValue
            // 
            this.btnApplyValue.Location = new System.Drawing.Point(327, 511);
            this.btnApplyValue.Name = "btnApplyValue";
            this.btnApplyValue.Size = new System.Drawing.Size(75, 23);
            this.btnApplyValue.TabIndex = 5;
            this.btnApplyValue.Text = "Apply";
            this.btnApplyValue.UseVisualStyleBackColor = true;
            this.btnApplyValue.Visible = false;
            this.btnApplyValue.Click += new System.EventHandler(this.btnApplyValue_Click);
            // 
            // txtOutputName
            // 
            this.txtOutputName.Location = new System.Drawing.Point(89, 582);
            this.txtOutputName.Name = "txtOutputName";
            this.txtOutputName.Size = new System.Drawing.Size(138, 20);
            this.txtOutputName.TabIndex = 6;
            this.txtOutputName.Text = "result";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 585);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Output name:";
            // 
            // btnAnimate
            // 
            this.btnAnimate.Location = new System.Drawing.Point(408, 511);
            this.btnAnimate.Name = "btnAnimate";
            this.btnAnimate.Size = new System.Drawing.Size(75, 34);
            this.btnAnimate.TabIndex = 8;
            this.btnAnimate.Text = "Animate (test)";
            this.btnAnimate.UseVisualStyleBackColor = true;
            this.btnAnimate.Visible = false;
            this.btnAnimate.Click += new System.EventHandler(this.btnAnimate_Click);
            // 
            // btnResetAnimations
            // 
            this.btnResetAnimations.Location = new System.Drawing.Point(305, 580);
            this.btnResetAnimations.Name = "btnResetAnimations";
            this.btnResetAnimations.Size = new System.Drawing.Size(100, 23);
            this.btnResetAnimations.TabIndex = 9;
            this.btnResetAnimations.Text = "Reset animations";
            this.btnResetAnimations.UseVisualStyleBackColor = true;
            this.btnResetAnimations.Visible = false;
            this.btnResetAnimations.Click += new System.EventHandler(this.btnResetAnimations_Click);
            // 
            // lblLastSeed
            // 
            this.lblLastSeed.AutoSize = true;
            this.lblLastSeed.Location = new System.Drawing.Point(12, 565);
            this.lblLastSeed.Name = "lblLastSeed";
            this.lblLastSeed.Size = new System.Drawing.Size(79, 13);
            this.lblLastSeed.TabIndex = 10;
            this.lblLastSeed.Text = "Last seed: N/A";
            this.lblLastSeed.Click += new System.EventHandler(this.lblLastSeed_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(359, 9);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(126, 20);
            this.txtSearch.TabIndex = 11;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Search:";
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveSettings.Location = new System.Drawing.Point(12, 9);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(75, 20);
            this.btnSaveSettings.TabIndex = 13;
            this.btnSaveSettings.Text = "Save settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnLoadSettings
            // 
            this.btnLoadSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadSettings.Location = new System.Drawing.Point(89, 9);
            this.btnLoadSettings.Name = "btnLoadSettings";
            this.btnLoadSettings.Size = new System.Drawing.Size(75, 20);
            this.btnLoadSettings.TabIndex = 14;
            this.btnLoadSettings.Text = "Load settings";
            this.btnLoadSettings.UseVisualStyleBackColor = true;
            this.btnLoadSettings.Click += new System.EventHandler(this.btnLoadSettings_Click);
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 615);
            this.Controls.Add(this.btnLoadSettings);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblLastSeed);
            this.Controls.Add(this.btnResetAnimations);
            this.Controls.Add(this.btnAnimate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutputName);
            this.Controls.Add(this.btnApplyValue);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.lstValues);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PlanetGenerator 0.3 by Marc D.";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstValues;
        private System.Windows.Forms.ColumnHeader clmnhName;
        private System.Windows.Forms.ColumnHeader clmnhValue;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnApplyValue;
        private System.Windows.Forms.TextBox txtOutputName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAnimate;
        private System.Windows.Forms.Button btnResetAnimations;
        private System.Windows.Forms.Label lblLastSeed;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnLoadSettings;
    }
}