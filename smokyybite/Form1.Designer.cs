namespace smokyybite
{
    partial class Smokyybite
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Smokyybite));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.shut_down = new System.Windows.Forms.Button();
            this.aboutbt = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.adminButton = new System.Windows.Forms.Button();
            this.cartButton = new System.Windows.Forms.Button();
            this.customerBar = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.displayPanel = new System.Windows.Forms.Panel();
            this.customerInfoLabel = new System.Windows.Forms.Label();
            this.editCustomerLink = new System.Windows.Forms.Button();
            this.customerInputPanel = new System.Windows.Forms.Panel();
            this.saveCustomerButton = new System.Windows.Forms.Button();
            this.tableComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.customerBar.SuspendLayout();
            this.panel1.SuspendLayout();
            this.displayPanel.SuspendLayout();
            this.customerInputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.shut_down);
            this.headerPanel.Controls.Add(this.aboutbt);
            this.headerPanel.Controls.Add(this.refreshButton);
            this.headerPanel.Controls.Add(this.pictureBox1);
            this.headerPanel.Controls.Add(this.adminButton);
            this.headerPanel.Controls.Add(this.cartButton);
            resources.ApplyResources(this.headerPanel, "headerPanel");
            this.headerPanel.Name = "headerPanel";
            // 
            // shut_down
            // 
            this.shut_down.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.shut_down, "shut_down");
            this.shut_down.Image = global::_1.Properties.Resources.shut;
            this.shut_down.Name = "shut_down";
            this.shut_down.UseVisualStyleBackColor = true;
            this.shut_down.Click += new System.EventHandler(this.shut_down_Click_1);
            // 
            // aboutbt
            // 
            this.aboutbt.BackColor = System.Drawing.Color.LightGray;
            this.aboutbt.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.aboutbt, "aboutbt");
            this.aboutbt.Name = "aboutbt";
            this.aboutbt.UseVisualStyleBackColor = false;
            this.aboutbt.Click += new System.EventHandler(this.aboutbt_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.BackColor = System.Drawing.Color.Transparent;
            this.refreshButton.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.refreshButton.ForeColor = System.Drawing.Color.Red;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::_1.Properties.Resources.logo2;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // adminButton
            // 
            this.adminButton.BackColor = System.Drawing.Color.Gainsboro;
            this.adminButton.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.adminButton, "adminButton");
            this.adminButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.adminButton.Name = "adminButton";
            this.adminButton.UseVisualStyleBackColor = false;
            this.adminButton.Click += new System.EventHandler(this.adminButton_Click);
            // 
            // cartButton
            // 
            this.cartButton.BackColor = System.Drawing.Color.OrangeRed;
            this.cartButton.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.cartButton, "cartButton");
            this.cartButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cartButton.Name = "cartButton";
            this.cartButton.UseVisualStyleBackColor = false;
            this.cartButton.Click += new System.EventHandler(this.cartButton_Click);
            // 
            // customerBar
            // 
            this.customerBar.BackColor = System.Drawing.Color.SaddleBrown;
            this.customerBar.Controls.Add(this.panel1);
            this.customerBar.Controls.Add(this.displayPanel);
            this.customerBar.Controls.Add(this.customerInputPanel);
            resources.ApplyResources(this.customerBar, "customerBar");
            this.customerBar.Name = "customerBar";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.searchTextBox);
            this.panel1.Controls.Add(this.label3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // searchTextBox
            // 
            this.searchTextBox.BackColor = System.Drawing.Color.White;
            this.searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.searchTextBox, "searchTextBox");
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Firebrick;
            this.label3.Name = "label3";
            // 
            // displayPanel
            // 
            resources.ApplyResources(this.displayPanel, "displayPanel");
            this.displayPanel.BackColor = System.Drawing.Color.Transparent;
            this.displayPanel.Controls.Add(this.customerInfoLabel);
            this.displayPanel.Controls.Add(this.editCustomerLink);
            this.displayPanel.Name = "displayPanel";
            // 
            // customerInfoLabel
            // 
            resources.ApplyResources(this.customerInfoLabel, "customerInfoLabel");
            this.customerInfoLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.customerInfoLabel.Name = "customerInfoLabel";
            this.customerInfoLabel.Click += new System.EventHandler(this.customerInfoLabel_Click);
            // 
            // editCustomerLink
            // 
            resources.ApplyResources(this.editCustomerLink, "editCustomerLink");
            this.editCustomerLink.BackColor = System.Drawing.Color.DarkOrange;
            this.editCustomerLink.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.editCustomerLink.FlatAppearance.BorderSize = 0;
            this.editCustomerLink.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.editCustomerLink.Name = "editCustomerLink";
            this.editCustomerLink.UseVisualStyleBackColor = false;
            this.editCustomerLink.Click += new System.EventHandler(this.editCustomerLink_Click);
            // 
            // customerInputPanel
            // 
            this.customerInputPanel.Controls.Add(this.saveCustomerButton);
            this.customerInputPanel.Controls.Add(this.tableComboBox);
            this.customerInputPanel.Controls.Add(this.label2);
            this.customerInputPanel.Controls.Add(this.nameTextBox);
            this.customerInputPanel.Controls.Add(this.label1);
            this.customerInputPanel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.customerInputPanel, "customerInputPanel");
            this.customerInputPanel.Name = "customerInputPanel";
            // 
            // saveCustomerButton
            // 
            this.saveCustomerButton.BackColor = System.Drawing.Color.DarkOrange;
            this.saveCustomerButton.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.saveCustomerButton, "saveCustomerButton");
            this.saveCustomerButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.saveCustomerButton.Name = "saveCustomerButton";
            this.saveCustomerButton.UseVisualStyleBackColor = false;
            this.saveCustomerButton.Click += new System.EventHandler(this.saveCustomerButton_Click);
            // 
            // tableComboBox
            // 
            this.tableComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.tableComboBox, "tableComboBox");
            this.tableComboBox.FormattingEnabled = true;
            this.tableComboBox.Items.AddRange(new object[] {
            resources.GetString("tableComboBox.Items"),
            resources.GetString("tableComboBox.Items1"),
            resources.GetString("tableComboBox.Items2"),
            resources.GetString("tableComboBox.Items3"),
            resources.GetString("tableComboBox.Items4"),
            resources.GetString("tableComboBox.Items5"),
            resources.GetString("tableComboBox.Items6"),
            resources.GetString("tableComboBox.Items7"),
            resources.GetString("tableComboBox.Items8")});
            this.tableComboBox.Name = "tableComboBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.Name = "nameTextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // menuPanel
            // 
            resources.ApplyResources(this.menuPanel, "menuPanel");
            this.menuPanel.Name = "menuPanel";
            // 
            // Smokyybite
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.menuPanel);
            this.Controls.Add(this.customerBar);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Smokyybite";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.headerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.customerBar.ResumeLayout(false);
            this.customerBar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.displayPanel.ResumeLayout(false);
            this.displayPanel.PerformLayout();
            this.customerInputPanel.ResumeLayout(false);
            this.customerInputPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Button cartButton;
        private System.Windows.Forms.Panel customerBar;
        private System.Windows.Forms.Button adminButton;
        private System.Windows.Forms.Panel customerInputPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tableComboBox;
        private System.Windows.Forms.Button saveCustomerButton;
        private System.Windows.Forms.Panel displayPanel;
        private System.Windows.Forms.Label customerInfoLabel;
        private System.Windows.Forms.Button editCustomerLink;
        private System.Windows.Forms.FlowLayoutPanel menuPanel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button aboutbt;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button shut_down;
    }
}

