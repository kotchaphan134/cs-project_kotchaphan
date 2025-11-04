namespace smokyybite
{
    partial class CartForm
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
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.customerInfoPanel = new System.Windows.Forms.Panel();
            this.confirmOrderButton = new System.Windows.Forms.Button();
            this.tableDisplayTextBox = new System.Windows.Forms.TextBox();
            this.nameDisplayTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cartDetailPanel = new System.Windows.Forms.Panel();
            this.goback = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.deleteAllButton = new System.Windows.Forms.Button();
            this.cartItemsFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.totalLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mainTableLayout.SuspendLayout();
            this.customerInfoPanel.SuspendLayout();
            this.cartDetailPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.goback)).BeginInit();
            this.panel1.SuspendLayout();
            this.footerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.ColumnCount = 2;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.mainTableLayout.Controls.Add(this.customerInfoPanel, 1, 0);
            this.mainTableLayout.Controls.Add(this.cartDetailPanel, 0, 0);
            this.mainTableLayout.Location = new System.Drawing.Point(1, 138);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 1;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Size = new System.Drawing.Size(1581, 714);
            this.mainTableLayout.TabIndex = 0;
            // 
            // customerInfoPanel
            // 
            this.customerInfoPanel.BackColor = System.Drawing.Color.SandyBrown;
            this.customerInfoPanel.Controls.Add(this.confirmOrderButton);
            this.customerInfoPanel.Controls.Add(this.tableDisplayTextBox);
            this.customerInfoPanel.Controls.Add(this.nameDisplayTextBox);
            this.customerInfoPanel.Controls.Add(this.label3);
            this.customerInfoPanel.Controls.Add(this.label2);
            this.customerInfoPanel.Controls.Add(this.label1);
            this.customerInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customerInfoPanel.Location = new System.Drawing.Point(951, 3);
            this.customerInfoPanel.Name = "customerInfoPanel";
            this.customerInfoPanel.Padding = new System.Windows.Forms.Padding(50);
            this.customerInfoPanel.Size = new System.Drawing.Size(627, 708);
            this.customerInfoPanel.TabIndex = 0;
            // 
            // confirmOrderButton
            // 
            this.confirmOrderButton.BackColor = System.Drawing.Color.OrangeRed;
            this.confirmOrderButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.confirmOrderButton.FlatAppearance.BorderSize = 0;
            this.confirmOrderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.confirmOrderButton.Font = new System.Drawing.Font("MN ECLAIR", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.confirmOrderButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.confirmOrderButton.Location = new System.Drawing.Point(88, 597);
            this.confirmOrderButton.Name = "confirmOrderButton";
            this.confirmOrderButton.Size = new System.Drawing.Size(425, 75);
            this.confirmOrderButton.TabIndex = 5;
            this.confirmOrderButton.Text = "ยืนยันคำสั่งซื้อ";
            this.confirmOrderButton.UseVisualStyleBackColor = false;
            this.confirmOrderButton.Click += new System.EventHandler(this.confirmOrderButton_Click);
            // 
            // tableDisplayTextBox
            // 
            this.tableDisplayTextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tableDisplayTextBox.Font = new System.Drawing.Font("MN ECLAIR", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.tableDisplayTextBox.Location = new System.Drawing.Point(62, 424);
            this.tableDisplayTextBox.MinimumSize = new System.Drawing.Size(4, 50);
            this.tableDisplayTextBox.Name = "tableDisplayTextBox";
            this.tableDisplayTextBox.ReadOnly = true;
            this.tableDisplayTextBox.Size = new System.Drawing.Size(451, 65);
            this.tableDisplayTextBox.TabIndex = 4;
            // 
            // nameDisplayTextBox
            // 
            this.nameDisplayTextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.nameDisplayTextBox.Font = new System.Drawing.Font("MN ECLAIR", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.nameDisplayTextBox.Location = new System.Drawing.Point(62, 218);
            this.nameDisplayTextBox.MinimumSize = new System.Drawing.Size(4, 50);
            this.nameDisplayTextBox.Name = "nameDisplayTextBox";
            this.nameDisplayTextBox.ReadOnly = true;
            this.nameDisplayTextBox.Size = new System.Drawing.Size(451, 65);
            this.nameDisplayTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MN ECLAIR", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(53, 363);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 49);
            this.label3.TabIndex = 2;
            this.label3.Text = "โต๊ะที่";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MN ECLAIR", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(53, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 49);
            this.label2.TabIndex = 1;
            this.label2.Text = "ชื่อลูกค้า";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MN ECLAIR", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(113, 50);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 50);
            this.label1.Size = new System.Drawing.Size(355, 116);
            this.label1.TabIndex = 0;
            this.label1.Text = "- ตรวจสอบคำสั่งซื้อ -";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cartDetailPanel
            // 
            this.cartDetailPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(235)))));
            this.cartDetailPanel.Controls.Add(this.goback);
            this.cartDetailPanel.Controls.Add(this.panel1);
            this.cartDetailPanel.Controls.Add(this.cartItemsFlowPanel);
            this.cartDetailPanel.Controls.Add(this.footerPanel);
            this.cartDetailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cartDetailPanel.Location = new System.Drawing.Point(3, 3);
            this.cartDetailPanel.Name = "cartDetailPanel";
            this.cartDetailPanel.Padding = new System.Windows.Forms.Padding(30);
            this.cartDetailPanel.Size = new System.Drawing.Size(942, 708);
            this.cartDetailPanel.TabIndex = 1;
            // 
            // goback
            // 
            this.goback.Cursor = System.Windows.Forms.Cursors.Hand;
            this.goback.Image = global::_1.Properties.Resources.back5;
            this.goback.Location = new System.Drawing.Point(19, 613);
            this.goback.Name = "goback";
            this.goback.Size = new System.Drawing.Size(164, 87);
            this.goback.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.goback.TabIndex = 5;
            this.goback.TabStop = false;
            this.goback.Click += new System.EventHandler(this.goback_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Sienna;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.deleteAllButton);
            this.panel1.Location = new System.Drawing.Point(33, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(876, 70);
            this.panel1.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MN ECLAIR", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label6.Location = new System.Drawing.Point(179, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 37);
            this.label6.TabIndex = 8;
            this.label6.Text = "รายการ";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MN ECLAIR", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label5.Location = new System.Drawing.Point(437, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 37);
            this.label5.TabIndex = 7;
            this.label5.Text = "จำนวน";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MN ECLAIR", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(599, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 37);
            this.label4.TabIndex = 6;
            this.label4.Text = "รวม";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // deleteAllButton
            // 
            this.deleteAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.deleteAllButton.FlatAppearance.BorderSize = 0;
            this.deleteAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteAllButton.Font = new System.Drawing.Font("MN ECLAIR", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.deleteAllButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.deleteAllButton.Location = new System.Drawing.Point(698, 13);
            this.deleteAllButton.Name = "deleteAllButton";
            this.deleteAllButton.Size = new System.Drawing.Size(156, 45);
            this.deleteAllButton.TabIndex = 2;
            this.deleteAllButton.Text = "🗑️ ลบทั้งหมด";
            this.deleteAllButton.UseVisualStyleBackColor = false;
            this.deleteAllButton.Click += new System.EventHandler(this.deleteAllButton_Click);
            // 
            // cartItemsFlowPanel
            // 
            this.cartItemsFlowPanel.AutoScroll = true;
            this.cartItemsFlowPanel.BackColor = System.Drawing.Color.White;
            this.cartItemsFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.cartItemsFlowPanel.Location = new System.Drawing.Point(33, 93);
            this.cartItemsFlowPanel.Name = "cartItemsFlowPanel";
            this.cartItemsFlowPanel.Size = new System.Drawing.Size(876, 507);
            this.cartItemsFlowPanel.TabIndex = 1;
            this.cartItemsFlowPanel.WrapContents = false;
            // 
            // footerPanel
            // 
            this.footerPanel.Controls.Add(this.totalLabel);
            this.footerPanel.Location = new System.Drawing.Point(261, 621);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(651, 79);
            this.footerPanel.TabIndex = 0;
            // 
            // totalLabel
            // 
            this.totalLabel.AutoSize = true;
            this.totalLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.totalLabel.Font = new System.Drawing.Font("MN ECLAIR", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.totalLabel.Location = new System.Drawing.Point(449, 0);
            this.totalLabel.Name = "totalLabel";
            this.totalLabel.Size = new System.Drawing.Size(202, 54);
            this.totalLabel.TabIndex = 0;
            this.totalLabel.Text = "รวม 0.00 บาท";
            this.totalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::_1.Properties.Resources.Admin1;
            this.pictureBox1.Location = new System.Drawing.Point(1, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1581, 143);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // CartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1582, 853);
            this.Controls.Add(this.mainTableLayout);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CartForm";
            this.Load += new System.EventHandler(this.CartForm_Load);
            this.mainTableLayout.ResumeLayout(false);
            this.customerInfoPanel.ResumeLayout(false);
            this.customerInfoPanel.PerformLayout();
            this.cartDetailPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.goback)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.footerPanel.ResumeLayout(false);
            this.footerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.Panel customerInfoPanel;
        private System.Windows.Forms.TextBox tableDisplayTextBox;
        private System.Windows.Forms.TextBox nameDisplayTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button confirmOrderButton;
        private System.Windows.Forms.Panel cartDetailPanel;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Label totalLabel;
        private System.Windows.Forms.FlowLayoutPanel cartItemsFlowPanel;
        private System.Windows.Forms.Button deleteAllButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox goback;
    }
}