using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static smokyybite.Connect_db;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace smokyybite
{
    public partial class AdminPanelForm : Form
    {
        //--- ตัวแปรเก็บข้อมูล สถานะ ---
        private List<MenuItem> _allMenuItems; // สำหรับเก็บรายการสินค้าทั้งหมด
        private MenuItem _selectedItem; // สำหรับเก็บสินค้าที่ถูกเลือกในตาราง
        private Topping _selectedTopping; // สำหรับเก็บท็อปปิ้งที่ถูกเลือกในตาราง
        private List<SalesReportItem> currentReportData; // สำหรับเก็บข้อมูลรายงานปัจจุบัน
        private List<OrderHistoryItem> _fullReportData; // เก็บข้อมูลทั้งหมดในช่วงวันที่ที่เลือก



        // หน้าต่างการจัดการแอดมิน
        public AdminPanelForm()
        {
            InitializeComponent();
            // ผูก Event ของฟอร์ม
            this.Load += AdminPanelForm_Load;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            this.dgvHistory.SelectionChanged += new System.EventHandler(this.dgvHistory_SelectionChanged);
            this.dgvToppings.SelectionChanged += new System.EventHandler(this.dgvToppings_SelectionChanged);
            this.btnAdd_Topping.Click += new System.EventHandler(this.btnAdd_Topping_Click);
            this.btnEdit_Topping.Click += new System.EventHandler(this.btnEdit_Topping_Click);
            this.btnDelete_Topping.Click += new System.EventHandler(this.btnDelete_Topping_Click);
            reportBtnSearch.Click += reportBtnSearch_Click;
            reportBtnReset.Click += reportBtnReset_Click;
            // ผูก Event Handlers
            reportBtnSearch.Click += reportBtnSearch_Click;
            reportBtnReset.Click += reportBtnReset_Click;
            btnApplyChartFilter.Click += btnApplyChartFilter_Click;
            btnResetChart.Click += (s, e) => { UpdateDashboard(_fullReportData); }; // รีเซ็ตกราฟโดยใช้ข้อมูลเดิม
            //this.btnBrowse.Click += btnBrowse_Click;
        }

        // โหลดข้อมูลตอนฟอร์มถูกเปิด
        private void AdminPanelForm_Load(object sender, EventArgs e)
        {
            {

                // --- แท็บ 1: จัดการสินค้า ---
                SetupDataGridView();
                LoadProducts();
                ClearInputFields();


                SetupToppingDataGridView();
                LoadToppings(); // โหลดข้อมูลมาก่อน
                EnterNewMode_Topping(); // 

                // --- แท็บ 2 : การเตรียมอาหาร ---
                SetupKitchenTab();
                LoadKitchenOrders();

                // --- แท็บ 3: ประวัติการสั่งซื้อ ---
                SetupHistoryTab();
                LoadAllHistory();
                btnSearch.PerformClick();
                // โหลดข้อมูลทั้งหมดในตอนแรก

                //---- แท็บ 4: รายงานสถิติ ----
                SetupReportDataGridView();
                reportDtpStartDate.Value = DateTime.Now.AddYears(-1);
                reportDtpEndDate.Value = DateTime.Now;


                
                // ผูก Event Handlers
                dgvPendingOrderss.SelectionChanged += dgvPendingOrdersss_SelectionChanged;
                dgvReadyToServee.SelectionChanged += dgvReadyToServee_SelectionChanged;
                dgvCancelledOrderss.SelectionChanged += dgvCancelledOrderss_SelectionChanged;                
                dgvServedOrderss.SelectionChanged += dgvServedOrderss_SelectionChanged;
                dgvProducts.SelectionChanged += dgvProducts_SelectionChanged;
                btnAdd.Click += btnAdd_Click;
                btnEdit.Click += btnEdit_Click;
                //btnDelete.Click += btnDelete_Click;
                btnConfirm.Click += btnConfirm_Click;
                txtSearch.TextChanged += txtSearch_TextChanged;
                btnReset.Click += (s, ev) => { txtSearch.Clear(); };


            }

        }
        // --------------------------------แท็บ 1: จัดการสินค้า ---------------------------------
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {

            // รับ TabPage ปัจจุบันที่กำลังจะวาด
            TabPage currentPage = tabControl1.TabPages[e.Index];

            // รับพื้นที่ (Rectangle) ที่จะวาดสำหรับแท็บนี้
            Rectangle tabBounds = e.Bounds;

            // เตรียม Brush สำหรับวาดข้อความ
            //    ถ้าแท็บนี้ถูกเลือกอยู่ ให้ใช้ Brush สีน้ำเงินเข้ม, ถ้าไม่ ให้ใช้สีดำ
            Brush textBrush = (e.State == DrawItemState.Selected) ? Brushes.White : Brushes.Black;

            // เตรียม Font สำหรับวาดข้อความ
            Font tabFont = new Font("Segoe UI Emoji", 28F, FontStyle.Bold);

            // วาดพื้นหลังของแท็บ (เวอร์ชันปรับแต่งสี)
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {                // --- สีของแท็บที่ "ถูกเลือก" ---

                e.Graphics.FillRectangle(Brushes.Peru, tabBounds);

            }
            else
            {
                // --- สีของแท็บที่ "ไม่ได้ถูกเลือก" ---
                e.Graphics.FillRectangle(Brushes.Tan, tabBounds);

            }

            Image tabImage = null;
            if (currentPage.ImageIndex >= 0 && tabControl1.ImageList != null)
            {
                tabImage = tabControl1.ImageList.Images[currentPage.ImageIndex];
            }

          
            if (tabImage != null)
            {
                // คำนวณตำแหน่ง Y เพื่อให้รูปอยู่กึ่งกลางแนวตั้ง
                int imageY = tabBounds.Top + (tabBounds.Height - tabImage.Height) / 2;
                // วาดรูปทางซ้ายของแท็บ (เยื้องเข้ามา 5 pixels)
                e.Graphics.DrawImage(tabImage, tabBounds.Left + 5, imageY);
            }

            //วาดข้อความแนวนอน
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near; // จัดชิดซ้าย
            sf.LineAlignment = StringAlignment.Center; // จัดกึ่งกลางแนวตั้ง

            // สร้างพื้นที่สำหรับวาดข้อความ โดยให้เยื้องจากขอบซ้ายเข้ามา (เผื่อที่สำหรับรูปภาพ)
            int textOffset = (tabImage?.Width ?? 0) + 10; // ระยะห่างจากขอบซ้าย = ความกว้างของรูป + 10 pixels
            RectangleF textBounds = new RectangleF(tabBounds.Left + textOffset, tabBounds.Top, tabBounds.Width - textOffset, tabBounds.Height);

            // วาดข้อความลงบนพื้นที่ที่กำหนด
            e.Graphics.DrawString(currentPage.Text, tabFont, textBrush, textBounds, sf);

            // 9. (Optional) วาดเส้นขอบรอบๆ แท็บเพื่อความสวยงาม
            e.Graphics.DrawRectangle(Pens.Gray, tabBounds);

            // 10. (Optional) วาดกรอบ Focus เมื่อผู้ใช้เลือกแท็บด้วยคีย์บอร์ด
            e.DrawFocusRectangle();
        }


        // เมธอดสำหรับตั้งค่าคอลัมน์ของตารางสินค้า

        private void SetupDataGridView()
        {
            dgvProducts.RowTemplate.Height = 60;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR", 14F, System.Drawing.FontStyle.Bold);
            dgvProducts.DefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR Light", 11F);
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewImageColumn { DataPropertyName = "Photo", HeaderText = "Image", Width = 80, ImageLayout = DataGridViewImageCellLayout.Zoom });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Stock", HeaderText = "Stock", Width = 70 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "Price", Width = 80, DefaultCellStyle = { Format = "N2" } });
        }


        // เมธอดสำหรับโหลดข้อมูลสินค้า
        private void LoadProducts()
        {
            _allMenuItems = Connect_db.GetMenuItems();
            dgvProducts.DataSource = _allMenuItems;
            ClearInputFields();
        }

        // การพิมพ์ในช่องค้นหา
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            string keyword = txtSearch.Text.Trim().ToLower(); // ดึงคีย์เวิร์ดและแปลงเป็นตัวพิมพ์เล็ก
            if (string.IsNullOrEmpty(keyword))
            {
                dgvProducts.DataSource = _allMenuItems; // ถ้าไม่มีคีย์เวิร์ด ให้แสดงทั้งหมด
            }
            else
            {
                // กรองข้อมูลจาก List ที่เก็บไว้
                var filteredList = _allMenuItems.Where(item =>
                    item.Name.ToLower().Contains(keyword) ||
                    item.Description.ToLower().Contains(keyword)
                ).ToList();
                dgvProducts.DataSource = filteredList; // แสดงผลลัพธ์ที่กรองแล้ว
            }
        }

        //  เมื่อมีการเลือกแถวในตารางสินค้า
        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {

            // เมื่อมีการเลือกแถวในตาราง ให้แสดงข้อมูลในฟอร์มฝั่งขวา
            if (dgvProducts.SelectedRows.Count > 0)
            {

                _selectedItem = (MenuItem)dgvProducts.SelectedRows[0].DataBoundItem;
                DisplayItemDetails(_selectedItem);
                btnConfirm.BackColor = Color.DarkGoldenrod;
                txtstatus.Text = $"สถานะ: กำลังแก้ไข '{_selectedItem.Name}'";

            }
        }

        // แสดงรายละเอียดสินค้าในฟอร์มฝั่งขวา
        private void DisplayItemDetails(MenuItem item)
        {

            if (item == null) return;
            txtName.Text = item.Name;
            txtDesc.Text = item.Description;
            numPrice.Value = item.Price;
            numAmount.Value = item.Stock;
            picPhoto.Image = item.Photo;
            btnConfirm.Enabled = true; // เปิดปุ่มยืนยัน
        }
        // ล้างข้อมูลในฟอร์มฝั่งขวา
        private void ClearInputFields()
        {
            // ล้างข้อมูลในฟอร์มฝั่งขวา
            _selectedItem = null;
            dgvProducts.ClearSelection();
            txtName.Clear();
            txtDesc.Clear();
            numPrice.Value = 0;
            numAmount.Value = 0;
            picPhoto.Image = null; // หรือใส่รูป Placeholder
            btnConfirm.Enabled = true;
            txtstatus.Text = "สถานะ: - ";
        }
        //-------------------------ปุ่ม-------------------------
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            
            _selectedItem = new MenuItem(); // สร้างสินค้าใหม่ (แต่ยังไม่มี Id)
            txtName.Focus();// เลื่อนเคอร์เซอร์ไปที่ช่องชื่อ
            btnConfirm.Text = "ยืนยัน";
            btnConfirm.Enabled = true;
            btnConfirm.BackColor = Color.LightGreen;
            txtstatus.Text = "สถานะ: กำลังเพิ่มรายการใหม่";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่ามีการเลือกสินค้าหรือไม่
            if (_selectedItem == null || _selectedItem.Id == 0)
            {
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการลบจากตาราง", "แจ้งเตือน");
                return;
            }
            if (MessageBox.Show($"คุณแน่ใจหรือไม่ที่จะลบ '{_selectedItem.Name}'?", "ยืนยันการลบ", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (Connect_db.DeleteMenuItem(_selectedItem.Id))
                {
                    LoadProducts();
                }
            }
        }
        //-------------------------กรอกเพิ่ม แก้-------------------------
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // เปิดหน้าต่างเลือกไฟล์รูปภาพ
            using (var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.png;*.gif" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picPhoto.Image = Image.FromFile(ofd.FileName);
                }
            }
        }
        // ปุ่มยืนยันการเพิ่มหรือแก้ไข
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (_selectedItem == null) return;
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("กรุณากรอกชื่อสินค้า");
                return;
            }
            if (numPrice.Value <= 0)
            {
                MessageBox.Show("กรุณากรอกราคาสินค้าให้ถูกต้อง");
                return;
            }

            if (numAmount.Value < 0)
            {
                MessageBox.Show("จำนวนคงคลังต้องไม่ติดลบ");
                return;
            }
            if (picPhoto.Image == null)
            {
                MessageBox.Show("กรุณาเลือกรูปภาพสินค้า");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDesc.Text))
            {
                MessageBox.Show("กรุณากรอกรายละเอียดสินค้า");
                return;
            }
            // ถ้าผ่านการตรวจสอบทั้งหมด ให้ดำเนินการต่อ

            _selectedItem.Name = txtName.Text;
            _selectedItem.Description = txtDesc.Text;
            _selectedItem.Price = numPrice.Value;
            _selectedItem.Stock = (int)numAmount.Value;
            _selectedItem.Photo = picPhoto.Image;

            bool success;
            bool isAddingNewItem = (_selectedItem.Id == 0); //

            if (_selectedItem.Id == 0) // ถ้าเป็นสินค้าใหม่
            {
                //ตรวจสอบถ้าชื่อสินค้าซ้ำ
                if (Connect_db.GetMenuItems().Any(i => i.Name.Equals(_selectedItem.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("ชื่อสินค้านี้มีอยู่ในระบบแล้ว กรุณาใช้ชื่ออื่น", "ข้อผิดพลาด");
                    LoadProducts(); // โหลดข้อมูลใหม่ทั้งหมด

                    return;
                }
                success = Connect_db.AddMenuItem(_selectedItem);
                MessageBox.Show("เพิ่มสินค้าเรียบร้อย");
            }
            else // ถ้าเป็นสินค้าเก่า
            {
                success = Connect_db.UpdateMenuItem(_selectedItem);
                MessageBox.Show("แก้ไขสินค้าเรียบร้อย");
            }

            if (success)
            {
                LoadProducts(); // โหลดข้อมูลใหม่ทั้งหมด
                if (isAddingNewItem)
                {
                    // ถ้าเป็นการเพิ่มสินค้าใหม่ ให้จำลองการกดปุ่ม "เพิ่ม" อีกครั้ง
                    // เพื่อเข้าสู่สถานะพร้อมกรอกข้อมูลชิ้นต่อไปทันที
                    btnAdd_Click(sender, e);
                }
                else
                {
                    // ถ้าเป็นการแก้ไข ให้เคลียร์ฟอร์มตามปกติ
                    ClearInputFields();
                }
            }
        }


        // ------------แท็บ 2: จัดการท็อปปิ้ง ------------

        // เมธอดสำหรับตั้งค่าคอลัมน์ของตารางท็อปปิ้ง
        private void SetupToppingDataGridView()
        {
            // ตั้งค่าพื้นฐาน
            dgvToppings.RowTemplate.Height = 30;
            dgvToppings.AllowUserToAddRows = false;
            dgvToppings.ReadOnly = true;
            dgvToppings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvToppings.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR", 14F, System.Drawing.FontStyle.Bold);
            dgvToppings.DefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR Light", 11F);
            // สร้างคอลัมน์เอง
            dgvToppings.AutoGenerateColumns = false;
            dgvToppings.Columns.Clear();

            // เพิ่มคอลัมน์ที่ต้องการ
            dgvToppings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name_topping", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvToppings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "Type", Width = 120 });
            dgvToppings.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Amount", HeaderText = "Stock", Width = 120 });
        }

        // เมธอดสำหรับโหลดข้อมูลท็อปปิ้งจากฐานข้อมูล
        private void LoadToppings()
        {
            // ถอด Event  ออกชั่วคราว
            dgvToppings.SelectionChanged -= dgvToppings_SelectionChanged;

            // โหลดข้อมูลตามปกติ
            dgvToppings.DataSource = null;
            dgvToppings.DataSource = Connect_db.GetToppings();

            // ใส่ Event กลับเข้าไปใหม่
            dgvToppings.SelectionChanged += dgvToppings_SelectionChanged;
            lblEditMode.Text = "สถานะ: - ";


        }


        /// รีเซ็ตฟอร์มด้านขวาให้อยู่ในสถานะ "เพิ่มรายการใหม่"
        private void EnterNewMode_Topping()
        {
            // ยกเลิกการเลือกแถวในตาราง
            dgvToppings.ClearSelection();

            // สร้าง object Topping ใหม่ที่ว่างเปล่าสำหรับรอรับข้อมูล
            _selectedTopping = new Topping { Id = 0 };

            // ล้างค่าใน input fields ทั้งหมด
            txtName_Topping.Clear();
            cmbType_Topping.SelectedIndex = -1;
            //txtPrice_Topping.Text = "0.00"; // ถ้ามี
            numAmount_Topping.Value = 0;

            // อัปเดต UI ของฟอร์มให้อยู่ใน "โหมดเพิ่มใหม่"
            lblEditMode.Text = "สถานะ: กำลังเพิ่มรายการใหม่";
            btnConfirm_Topping.Text = "ยืนยันการเพิ่ม";
            btnConfirm_Topping.BackColor = Color.LightGreen; // สีสำหรับโหมดเพิ่ม
            btnConfirm_Topping.Enabled = true;

            // ปิด/เปิดปุ่มให้ถูกต้องตามสถานะ
            btnEdit_Topping.Enabled = false;   // ปิดปุ่มแก้ไข
            btnDelete_Topping.Enabled = false; // ปิดปุ่มลบ

            // ย้าย Cursor ไปรอที่ช่องกรอกชื่อ
            txtName_Topping.Focus();
        }

        
        // เลือกแถวในตารางท็อปปิ้ง
        private void dgvToppings_SelectionChanged(object sender, EventArgs e)
        {   // ตรวจสอบว่ามีแถวถูกเลือกจริง และไม่ใช่การ ClearSelection
            if (dgvToppings.SelectedRows.Count > 0)
            {
                // ดึงข้อมูล Topping จากแถวที่เลือก
                _selectedTopping = (Topping)dgvToppings.SelectedRows[0].DataBoundItem;

                // นำข้อมูลไปแสดงในฟอร์มแก้ไข
                txtName_Topping.Text = _selectedTopping.Name;
                cmbType_Topping.SelectedItem = _selectedTopping.Type;
                numAmount_Topping.Value = _selectedTopping.Amount;

                // อัปเดต UI ให้อยู่ใน "โหมดแก้ไข"
                lblEditMode.Text = $"สถานะ: กำลังแก้ไข '{_selectedTopping.Name}'";
                btnConfirm_Topping.Text = "ยืนยันการแก้ไข";
                btnConfirm_Topping.BackColor = Color.Goldenrod; // สีสำหรับโหมดแก้ไข
                btnConfirm_Topping.Enabled = true;

                // เปิดใช้งานปุ่มที่ใช้ได้เฉพาะในโหมดแก้ไข
                btnEdit_Topping.Enabled = true;
                btnDelete_Topping.Enabled = true;

            }

        }

        //------------ปุ่ม----------------
        private void btnAdd_Topping_Click(object sender, EventArgs e)
        {
            lblEditMode.Text = "สถานะ: กำลังเพิ่มรายการใหม่";
            EnterNewMode();
            lblEditMode.Visible = true;
            btnConfirm_Topping.Enabled = true;
            btnConfirm_Topping.BackColor = Color.LightGreen;
            btnConfirm_Topping.Text = "ยืนยัน";

        }
        private void btnDelete_Topping_Click(object sender, EventArgs e)
        {
            if (_selectedTopping == null || _selectedTopping.Id == 0) return;

            if (MessageBox.Show($"ยืนยันการลบ '{_selectedTopping.Name}'?", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (Connect_db.DeleteTopping(_selectedTopping.Id))
                {

                    LoadToppings();
                    EnterNewMode(); // กลับสู่โหมดเพิ่มใหม่
                }
            }
        }


        //------------โหมดแก้ไข----------------
        private NumericUpDown GetNumAmount_Topping()
        {
            return numAmount_Topping;
        }


        private void EnterEditMode(NumericUpDown numAmount_Topping)
        {
            if (_selectedTopping == null) return;

            // อัปเดต UI
            lblEditMode.Text = $"สถานะ: กำลังแก้ไข '{_selectedTopping.Name}'";
            btnConfirm_Topping.Text = "ยืนยัน";
            btnDelete_Topping.Enabled = true; // เปิดปุ่มลบ

            // นำข้อมูลไปใส่ในฟอร์ม
            txtName_Topping.Text = _selectedTopping.Name;
            cmbType_Topping.SelectedItem = _selectedTopping.Type;
            txtPrice_Topping.Text = _selectedTopping.Price.ToString("N2");
            numAmount_Topping.Value = Convert.ToDecimal(_selectedTopping.Amount);

        }

        // เข้าสู่โหมด "เพิ่มใหม่"
        private void EnterNewMode()
        {
            lblEditMode.Visible = true;

            // ยกเลิกการเลือกแถวในตาราง
            dgvToppings.ClearSelection();

            // สร้าง object Topping ใหม่ที่ว่างเปล่าสำหรับรอรับข้อมูล (Id=0 คือสถานะ "ใหม่")
            _selectedTopping = new Topping { Id = 0 };

            // ล้างค่าใน input fields ทั้งหมด
            txtName_Topping.Clear();
            cmbType_Topping.SelectedIndex = -1;
            numAmount_Topping.Value = 0;

            // อัปเดต UI ของฟอร์มให้อยู่ใน "โหมดเพิ่มใหม่"
            lblEditMode.Text = "สถานะ: กำลังเพิ่มรายการใหม่";
            btnConfirm_Topping.Text = "ยืนยันการเพิ่ม";
            btnConfirm_Topping.BackColor = Color.LightGreen;
            btnConfirm_Topping.Enabled = true;

            // ปิด/เปิดปุ่มให้ถูกต้องตามสถานะ
            btnEdit_Topping.Enabled = false;   // ปิดปุ่มแก้ไข เพราะยังไม่มีอะไรให้แก้ไข
            btnDelete_Topping.Enabled = false; // ปิดปุ่มลบ เพราะยังไม่มีอะไรให้ลบ

            // ย้าย Cursor ไปรอที่ช่องกรอกชื่อ เพื่อความสะดวก
            txtName_Topping.Focus();
        }

        private void btnSave_Topping_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName_Topping.Text) || cmbType_Topping.SelectedItem == null)
            {
                MessageBox.Show("กรุณากรอกชื่อและเลือกประเภท", "ข้อมูลไม่ครบถ้วน");
                return;
            }
            _selectedTopping.Name = txtName_Topping.Text;
            _selectedTopping.Type = cmbType_Topping.SelectedItem.ToString();
            _selectedTopping.Amount = (int)numAmount_Topping.Value;
            _selectedTopping.Price = 0; // ล็อกราคา

            bool success;
            // ตรวจสอบจาก ID ว่าควรจะ "เพิ่ม" หรือ "แก้ไข"
            if (_selectedTopping.Id == 0)
            {
                // ตรวจว่าชื่อซ้ำไหม
                if (Connect_db.GetToppings().Any(t => t.Name.Equals(_selectedTopping.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("ชื่อท็อปปิ้งนี้มีอยู่ในระบบแล้ว กรุณาใช้ชื่ออื่น", "ข้อผิดพลาด");
                    LoadToppings(); // โหลดข้อมูลใหม่ทั้งหมด
                    return;
                }
                success = Connect_db.AddTopping(_selectedTopping);
                MessageBox.Show("เพิ่ม topping เรียบร้อย", "สำเร็จ");

            }
            else
            {
                success = Connect_db.UpdateTopping(_selectedTopping);
                MessageBox.Show("แก้ไข topping เรียบร้อย", "สำเร็จ");
            }

            if (success)
            {
                LoadToppings();      // โหลดข้อมูลใหม่
                EnterNewMode();     // กลับสู่โหมดเพิ่มใหม่เสมอหลังบันทึกสำเร็จ
            }
        }
        //------------แท็บ 2: การเตรียมอาหาร ------------
        
        private void SetupKitchenTab()
        {
            // ตั้งค่า DataGridView ของฝั่ง "รอดำเนินการ"
            SetupKitchenDataGridView(dgvPendingOrderss);
            SetupKitchenDataGridView(dgvReadyToServee);

            // ตั้งค่า DataGridView ของฝั่ง "เสิร์ฟแล้ว"
            SetupKitchenDataGridView(dgvServedOrderss);
            SetupKitchenDataGridView(dgvCancelledOrderss);
        }

        private void SetupKitchenDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR", 14F, System.Drawing.FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR Light", 11F);
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderDateTime", HeaderText = "เวลา", Width = 80, DefaultCellStyle = { Format = "HH:mm:ss" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderDateTime", HeaderText = "วัน", Width = 80, DefaultCellStyle = { Format = "dd/MM" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TableCustomer", HeaderText = "โต๊ะ", Width = 60 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "ชื่อลูกค้า", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalSum", HeaderText = "ยอดรวม", Width = 80, DefaultCellStyle = { Format = "N2" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "สถานะ", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        }

        // เมธอดหลักสำหรับโหลดข้อมูลทั้งสองฝั่ง
        private void LoadKitchenOrders()
        {
            // โหลดฝั่งรอดำเนินการ
            var pendingData = ProcessRawHistoryData(Connect_db.GetRawHistoryByStatus("รอดำเนินการ"));
            dgvPendingOrderss.DataSource = pendingData;
            // โหลดฝั่งพร้อมเสิร์ฟ
            var readyData = ProcessRawHistoryData(Connect_db.GetRawHistoryByStatus("พร้อมเสริฟ"));
            dgvReadyToServee.DataSource = readyData;

            // โหลดฝั่งเสิร์ฟแล้ว
            var servedData = ProcessRawHistoryData(Connect_db.GetRawHistoryByStatus("เสิร์ฟแล้ว"));
            dgvServedOrderss.DataSource = servedData;
            // โหลดฝั่งยกเลิก
            var cancalData = ProcessRawHistoryData(Connect_db.GetRawHistoryByStatus("ยกเลิก"));
            dgvCancelledOrderss.DataSource = cancalData;

            // เคลียร์ฟอร์มรายละเอียด
            ClearKitchenDetails(true); // true for pending side
            ClearKitchenDetails(false); // false for served side
        }

        // เมธอดสำหรับจัดกลุ่มข้อมูลดิบ (ใช้ร่วมกัน)
        private List<OrderHistoryItem> ProcessRawHistoryData(List<RawHistoryItem> rawData)
        {
            return rawData
                .GroupBy(rawItem => new { rawItem.OrderDateTime, rawItem.CustomerName })
                .Select(group => {
                    var first = group.First();
                    return new OrderHistoryItem
                    {
                        Id = first.Id,
                        OrderDateTime = first.OrderDateTime,
                        CustomerName = first.CustomerName,
                        TableCustomer = first.TableCustomer,
                        Phone = first.Phone,
                        PaymentSlip = first.PaymentSlip,
                        ToppingsSelected = first.ToppingsSelected,
                        ItemsList = group.Select(item => $"{item.ItemName} x{item.ItemAmount}").ToList(),
                        TotalSum = group.Sum(item => item.ItemSum),
                        Status = first.Status
                    };
                })
                .OrderBy(order => order.OrderDateTime)
                .ToList();
        }

        // --- Event Handlers ฝั่ง "รอดำเนินการ" ---
        private void dgvPendingOrdersss_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPendingOrderss.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvPendingOrderss.SelectedRows[0].DataBoundItem;
                kotCustomerTextBoxx.Text = selectedOrder.CustomerName;
                kotPhoneTextBoxx.Text = selectedOrder.Phone;
                kottotaltextBoxx.Text = selectedOrder.TotalSum.ToString("N2");
                kotItemsTextBoxx.Text = selectedOrder.FormattedItems;
                kotTableTextBoxxx.Text = selectedOrder.TableCustomer;
                kotToppingTextBoxx.Text = selectedOrder.ToppingsSelected;
                kotPicSlipp.Image = selectedOrder.PaymentSlip;
            }
        }
        private void btnMarkAsReady_Click(object sender, EventArgs e)
        {
            if (dgvPendingOrderss.SelectedRows.Count > 0)
            {

                var selectedOrder = (OrderHistoryItem)dgvPendingOrderss.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("ต้องการเปลี่ยนสถานะเป็น 'พร้อมเสิร์ฟ' หรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    bool success = Connect_db.UpdateOrderStatus(selectedOrder.OrderDateTime, selectedOrder.CustomerName, "พร้อมเสริฟ");
                    if (success)
                    {
                        MessageBox.Show("อัปเดตสถานะเป็น 'พร้อมเสิร์ฟ' เรียบร้อย");
                    }
                   
                    LoadKitchenOrders(); // โหลดข้อมูลใหม่ทั้งหมด
                }
            }

        }
        private void btnMarkAsCancal_Click(object sender, EventArgs e)
        {
            if (dgvPendingOrderss.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvPendingOrderss.SelectedRows[0].DataBoundItem;


                if (MessageBox.Show("ต้องการยกเลิกคำสั่งนี้หรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    bool success = Connect_db.UpdateOrderStatus(selectedOrder.OrderDateTime, selectedOrder.CustomerName, "ยกเลิก");
                    if (success)
                    {
                        MessageBox.Show("อัปเดตสถานะเป็น 'ยกเลิก' เรียบร้อย");
                    }
                    LoadKitchenOrders(); // โหลดข้อมูลใหม่ทั้งหมด

                }

            }
        }
        // --- Event Handlers ฝั่ง "พร้อมเสริฟ" ---
        private void dgvReadyToServee_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReadyToServee.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvReadyToServee.SelectedRows[0].DataBoundItem;
                rtsTableTextBoxxx.Text = selectedOrder.TableCustomer;
                rtsCustomerTextBoxx.Text = selectedOrder.CustomerName;
                rtsPhoneTextBoxx.Text = selectedOrder.Phone;
                rtstotaltextBoxx.Text = selectedOrder.TotalSum.ToString("N2");
                rtsItemsTextBoxx.Text = selectedOrder.FormattedItems;
                rtsToppingTextBoxx.Text = selectedOrder.ToppingsSelected;
                rtsPicSlipp.Image = selectedOrder.PaymentSlip;
            }
        }
        private void BackToKot_Click(object sender, EventArgs e)
        {
            if (dgvReadyToServee.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvReadyToServee.SelectedRows[0].DataBoundItem;


                if (MessageBox.Show("ต้องการแก้ไขสถานะเป็น 'รอดำเนินการ' ใช่หรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    bool success = Connect_db.UpdateOrderStatus(selectedOrder.OrderDateTime, selectedOrder.CustomerName, "รอดำเนินการ");
                    if (success)
                    {
                        MessageBox.Show("อัปเดตสถานะเป็น 'รอดำเนินการ' เรียบร้อย");
                    }
                }

                LoadKitchenOrders(); // โหลดข้อมูลใหม่ทั้งหมด
            }
        }

        private void btnMarkAsServe_Click(object sender, EventArgs e)
        {
            if (dgvReadyToServee.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvReadyToServee.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("ต้องการเปลี่ยนสถานะเป็น 'เสิร์ฟแล้ว' หรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    bool success = Connect_db.UpdateOrderStatus(selectedOrder.OrderDateTime, selectedOrder.CustomerName, "เสิร์ฟแล้ว");
                    if (success)
                    {
                        MessageBox.Show("อัปเดตสถานะเป็น 'เสิร์ฟแล้ว' เรียบร้อย");
                    }
                }
                LoadKitchenOrders(); // โหลดข้อมูลใหม่ทั้งหมด
            }

        }

        // --- Event Handlers ฝั่ง "เสิร์ฟแล้ว" ---
        private void dgvServedOrderss_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvServedOrderss.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvServedOrderss.SelectedRows[0].DataBoundItem;
                servedTableTextBoxx.Text = selectedOrder.TableCustomer;
                servedCustomerTextBoxx.Text = selectedOrder.CustomerName;
                servedPhoneTextBoxx.Text = selectedOrder.Phone;
                servedtotaltextt.Text = selectedOrder.TotalSum.ToString("N2");
                servedItemsTextBoxx.Text = selectedOrder.FormattedItems;
                servedToppingTextBoxx.Text = selectedOrder.ToppingsSelected;
                servedPicSlipp.Image = selectedOrder.PaymentSlip;
               
            }
        }
        private void BacktoRTS_Click(object sender, EventArgs e)
        {
            if (dgvServedOrderss.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvServedOrderss.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("ต้องการเปลี่ยนสถานะเป็น 'พร้อมเสริฟ' หรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    bool success = Connect_db.UpdateOrderStatus(selectedOrder.OrderDateTime, selectedOrder.CustomerName, "พร้อมเสริฟ");
                    if (success)
                    {
                        MessageBox.Show("อัปเดตสถานะเป็น 'พร้อมเสริฟ' เรียบร้อย");
                    }
                }
                LoadKitchenOrders(); // โหลดข้อมูลใหม่ทั้งหมด

            }
        }
        // --- Event Handlers ฝั่ง "ยกเลิก" ---
        private void dgvCancelledOrderss_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCancelledOrderss.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvCancelledOrderss.SelectedRows[0].DataBoundItem;
                CancalTableTextBoxx.Text = selectedOrder.TableCustomer;
                CancalCustomerTextBoxx.Text = selectedOrder.CustomerName;
                CancalPhoneTextBoxx.Text = selectedOrder.Phone;
                CancaltotaltextBoxx.Text = selectedOrder.TotalSum.ToString("N2");
                CancalItemsTextBoxx.Text = selectedOrder.FormattedItems;
                CancalToppingTextBoxx.Text = selectedOrder.ToppingsSelected;
                CancalPicSlipp.Image = selectedOrder.PaymentSlip;
            }
        }
        private void btn_backtokot_Click(object sender, EventArgs e)
        {
            if (dgvCancelledOrderss.SelectedRows.Count > 0)
            {
                var selectedOrder = (OrderHistoryItem)dgvCancelledOrderss.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("ต้องการเปลี่ยนสถานะเป็น 'รอดำเนินการ' หรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    bool success = Connect_db.UpdateOrderStatus(selectedOrder.OrderDateTime, selectedOrder.CustomerName, "รอดำเนินการ");
                    if (success)
                    {
                        MessageBox.Show("อัปเดตสถานะเป็น 'รอดำเนินการ' เรียบร้อย");
                    }
                }
                LoadKitchenOrders(); // โหลดข้อมูลใหม่ทั้งหมด

            }

        }


        // เมธอดสำหรับล้างฟอร์มรายละเอียด
        private void ClearKitchenDetails(bool isPendingSide)
        {
            if (isPendingSide)
            {
                kotTableTextBox.Clear();
            }
            else
            {
            }
        }





        //------------แท็บ 3: ประวัติการสั่งซื้อ ------------
        private void SetupHistoryTab()
        {
            dtpStartDate.Format = DateTimePickerFormat.Custom;
            dtpStartDate.CustomFormat = "d MMMM yyyy"; //  8 October 2025

            dtpEndDate.Format = DateTimePickerFormat.Custom;
            dtpEndDate.CustomFormat = "d MMMM yyyy";

            // ตั้งค่าเริ่มต้น
            dtpStartDate.Value = DateTime.Now.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;  

            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.ReadOnly = true;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR", 14F, System.Drawing.FontStyle.Bold);
            dgvHistory.DefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR Light", 11F);

            //ปรับขนาดแถวอัตโนมัต
            dgvHistory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; 
            dgvHistory.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // ทำให้ข้อความขึ้นบรรทัดใหม่

            // สร้างคอลัมน์ 
            dgvHistory.AutoGenerateColumns = false;
            dgvHistory.Columns.Clear();

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OrderDateTime",
                HeaderText = "Date",
                Width = 150,
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm:ss" }
            });
            

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CustomerName",
                HeaderText = "Customer",
                Width = 120
            });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TableCustomer",
                HeaderText = "Table",
                Width = 80
            });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemsSummary",
                HeaderText = "Items",
                Width = 150
            });
            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ToppingsSelected",
                HeaderText = "Toppings",
                Width = 150
            });


            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalSum",
                HeaderText = "Total",
                Width = 100,
                DefaultCellStyle = { Format = "N2" }
            });

            dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "สถานะ",
                Width = 120
            });
            ClearHistoryDetails();
        }

        // โหลดข้อมูลประวัติการสั่งซื้อทั้งหมด
        private void LoadAllHistory()
        {
           
            // เช่น 1 ปี ย้อนหลัง
            dtpStartDate.Value = DateTime.Now.AddYears(-1);
            dtpEndDate.Value = DateTime.Now;
            // เรียกปุ่มค้นหาอัตโนมัติ
            btnSearch.PerformClick();
        }
        // ปุ่มค้นหาข้อมูลตามช่วงวันที่
        private void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtpStartDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date;

            // ตรวจสอบความถูกต้องของช่วงวันที่
            List<OrderHistoryItem> groupedData = Connect_db.GetGroupedOrderHistory(startDate, endDate);

            // นำข้อมูลไปแสดงใน DataGridView 
            dgvHistory.DataSource = null;
            dgvHistory.DataSource = groupedData;

            // คำนวณยอดรวมทั้งหมดและแสดงผล
            decimal grandTotal = groupedData.Sum(item => item.TotalSum);
            lblTotalSum.Text = $"ยอดรวม: ฿{grandTotal:N2}";

            count_order.Text = $"จำนวน : {groupedData.Count} ออเดอร์";

            if (!groupedData.Any())
            {
                MessageBox.Show("ไม่พบข้อมูลประวัติการสั่งซื้อในช่วงวันที่ที่เลือก", "No Data Found");
                ClearHistoryDetails();
            }
        
        }
        // ปุ่มรีเซ็ตการค้นหา
        private void btnreset2_Click(object sender, EventArgs e)
        {
            // ตั้งค่า DatePicker กลับไปเป็นค่าเริ่มต้น
            dtpStartDate.Value = DateTime.Now.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;

            // เรียกใช้ฟังก์ชันโหลดข้อมูลทั้งหมด
            LoadAllHistory();
        }


        // เลือกแถวในตารางประวัติการสั่งซื้อ
        private void dgvHistory_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count > 0)
            {
                // ดึงข้อมูลจากแถวที่เลือก
                
                var selectedItem = (OrderHistoryItem)dgvHistory.SelectedRows[0].DataBoundItem;
                // แสดงรายละเอียดทางด้านขวา
                txtCustomerName.Text = selectedItem.CustomerName;
                txtPhone.Text = selectedItem.Phone;
                txtorderitem.Text = selectedItem.ItemsSummary.Replace("\n", Environment.NewLine); 
                txttopping.Text = selectedItem.ToppingsSelected;
                txtTransferDate.Text = selectedItem.OrderDateTime.ToString("d/MM/yyyy");
                txtTransferTime.Text = selectedItem.OrderDateTime.ToString("HH:mm:ss");
                txtPaymentTotal.Text = selectedItem.TotalSum.ToString("N2");

                // แก้ไขส่วนดึงรูปภาพ ให้ใช้ Property ที่มีอยู่จริง
                picSlip.Image = selectedItem.PaymentSlip;
                // ดึงรูปสลิปจากฐานข้อมูล
                picSlip.Image = Connect_db.GetSlipImage(selectedItem.Id);
                if (picSlip.Image == null)
                {
                    // ถ้าไม่มีรูป ให้วาดข้อความ "No Picture"
                    picSlip.Paint += (s, ev) => { ev.Graphics.DrawString("No Picture", this.Font, Brushes.Gray, new PointF(110, 180)); };
                    picSlip.Invalidate(); // สั่งให้วาดใหม่
                }
            }
            else
            {
                // ถ้าไม่มีอะไรถูกเลือก ให้เคลียร์ฟอร์มด้านขวา
                txtCustomerName.Clear();
                txtPhone.Clear();
                txtTransferDate.Clear();
                txtTransferTime.Clear();
                txtPaymentTotal.Clear();
                picSlip.Image = null;
            }

        }

        // เคลียร์รายละเอียดในฝั่งานขวา
        private void ClearHistoryDetails()
        {
            picSlip.Image = null;
            txtCustomerName.Clear();
            txtPaymentTotal.Clear();
            txtPhone.Clear();
            txtTransferDate.Clear();
            txtTransferTime.Clear();
        }

        //------------แท็ป 4: รายงาน-------------------------------------
        //ฝั่งซ้าย Dashboard
        // เมธอดหลักสำหรับอัปเดตแดชบอร์ดทั้งหมด
        private void UpdateDashboard(List<OrderHistoryItem> data)
        { // อ่านค่าตัวกรองปัจจุบันจาก UI
            string filterType = cmbChartFilter.SelectedItem?.ToString() ?? "รายวัน";
            DateTime hourlyDate = dtpHourlyChartDate.Value.Date;

            // กรองข้อมูลตามตัวกรองย่อย
            List<OrderHistoryItem> filteredDataForDisplay;

            if (data == null || !data.Any())
            {
                filteredDataForDisplay = new List<OrderHistoryItem>(); // ใช้ List ว่างถ้าไม่มีข้อมูล
            }
            else
            {
                switch (filterType)
                {
                    case "รายชั่วโมง":
                        filteredDataForDisplay = data.Where(o => o.OrderDateTime.Date == hourlyDate).ToList();
                        lblOrderListDateRange.Text = $"วันที่: {hourlyDate:d MMMM yyyy}";
                        break;
             
                    default: // รายวัน และอื่นๆ
                        filteredDataForDisplay = data;
                        lblOrderListDateRange.Text = $"ช่วง: {reportDtpStartDate.Value:dd/MM/yy} - {reportDtpEndDate.Value:dd/MM/yy}";
                        break;
                }
            }

            // --- 3. อัปเดต Controls ทั้งหมดโดยใช้ข้อมูลที่ผ่านการกรองแล้ว ---
            UpdateSummaryCards(filteredDataForDisplay);
            UpdateSalesChart(filteredDataForDisplay, filterType); // ส่งข้อมูลที่กรองแล้วไปให้กราฟ
            UpdateRecentOrdersTable(filteredDataForDisplay); // ส่งข้อมูลที่กรองแล้วไปให้ตาราง
        }

            
        
        private void UpdateSummaryCards(List<OrderHistoryItem> data)
        {
            if (data == null || !data.Any())
            {
                lblTotalOrders.Text = "-";
                lblTotalRevenue.Text = "-";
                lblAverageOrderValue.Text = "-";
                return;
            }
            lblTotalOrders.Text = $" {data.Count}";
            lblTotalRevenue.Text = $"฿{data.Sum(item => item.TotalSum):N2}";
            lblAverageOrderValue.Text = $"{(data.Sum(item => item.TotalSum) / data.Count):N2}";
        }



        private void btnApplyChartFilter_Click(object sender, EventArgs e)
        {
            UpdateDashboard(_fullReportData);
        }
        private void UpdateSalesChart(List<OrderHistoryItem> filteredData, string filterType)
        
       {
            // --- 1. เคลียร์และเตรียมพื้นที่กราฟ ---
            salesChart.Series.Clear();
            salesChart.ChartAreas.Clear();
            salesChart.Titles.Clear();

            // สร้างพื้นที่วาดใหม่
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("MainArea");
            chartArea.AxisX.Interval = 1; // ให้แสดง Label แกน X ทุกๆ 1 หน่วย
            chartArea.AxisY.LabelStyle.Format = "N0"; // แสดงแกน Y เป็นตัวเลขไม่มีทศนิยม
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray; // สีเส้นกริดจางๆ
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            salesChart.ChartAreas.Add(chartArea);

            // สร้าง Series (เส้นกราฟ) 
            var series = new System.Windows.Forms.DataVisualization.Charting.Series("Sales")
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
                BorderWidth = 3,
                MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle,
                MarkerSize = 8,
                // (Optional) แสดงตัวเลขบนจุดของกราฟ
                IsValueShownAsLabel = true
            };
            salesChart.Series.Add(series);

            // --- 2. ตรวจสอบว่ามีข้อมูลหรือไม่ ---
            if (filteredData == null || !filteredData.Any())
            {
                // ถ้าไม่มีข้อมูล ให้แสดงข้อความบนกราฟแล้วจบการทำงาน
                salesChart.Titles.Add("ไม่มีข้อมูลให้แสดงผลในช่วงที่เลือก");
                return;
            }

            // --- 3. ตรรกะการจัดกลุ่มและพล็อตข้อมูล ---

            if (filterType == "รายชั่วโมง")
            {
                // --- กรณี "รายชั่วโมง" ---
                DateTime selectedDate = dtpHourlyChartDate.Value.Date;

                //1 เตรียมแกน X คงที่ (8:00 - 18:00)
                var hourlySales = new Dictionary<int, decimal>();
                for (int hour = 8; hour <= 18; hour++)
                {
                    hourlySales[hour] = 0;
                }

                //2 ดึงและจัดกลุ่มข้อมูลยอดขายจริงจาก filteredData
                var actualSales = filteredData
                    .GroupBy(o => o.OrderDateTime.Hour)
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalSum));

                //3 นำยอดขายจริงไป "เติม" ใน Dictionary ที่เตรียมไว้
                foreach (var sale in actualSales)
                {
                    if (hourlySales.ContainsKey(sale.Key))
                    {
                        hourlySales[sale.Key] = sale.Value;
                    }
                }

                //4 พล็อตกราฟจาก Dictionary
                foreach (var hourSale in hourlySales.OrderBy(kvp => kvp.Key))
                {
                    series.Points.AddXY($"{hourSale.Key:00}:00", hourSale.Value);
                }

                salesChart.Titles.Add($"สรุปยอดขายรายชั่วโมง วันที่ {selectedDate:d MMMM yyyy}");
            }
            else
            {
                // --- กรณี "รายวัน", "รายเดือน", "รายปี" ---
                dynamic groupedData = null;
                string format = "dd/MM"; // Format เริ่มต้น

                switch (filterType)
                {
                    case "รายเดือน":
                        groupedData = filteredData.GroupBy(o => new { o.OrderDateTime.Year, o.OrderDateTime.Month }).Select(g => new { Date = new DateTime(g.Key.Year, g.Key.Month, 1), Total = g.Sum(o => o.TotalSum) }).OrderBy(x => x.Date);
                        format = "MMM yy"; // รูปแบบ: Jan 25
                        break;

                    case "รายปี":
                        groupedData = filteredData.GroupBy(o => o.OrderDateTime.Year).Select(g => new { Date = new DateTime(g.Key, 1, 1), Total = g.Sum(o => o.TotalSum) }).OrderBy(x => x.Date);
                        format = "yyyy"; // รูปแบบ: 2024, 2025
                        break;

                    default: // "รายวัน"
                        groupedData = filteredData.GroupBy(o => o.OrderDateTime.Date).Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalSum) }).OrderBy(x => x.Date);
                        format = "dd/MM"; // รูปแบบ: 23/10
                        break;
                }

                if (groupedData != null)
                {
                    foreach (var point in groupedData)
                    {
                        // พล็อตจุดลงบนกราฟ
                        series.Points.AddXY(point.Date.ToString(format), point.Total);
                    }
                }
                salesChart.Titles.Add($"สรุปยอดขาย{filterType}");
            }
        
        }
                 


        private IEnumerable<dynamic> ParseItemsFromString(string itemsSummary)
        {
            if (string.IsNullOrEmpty(itemsSummary)) yield break;

            var lines = itemsSummary.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { " x" }, StringSplitOptions.None);
                if (parts.Length == 2 && int.TryParse(parts[1], out int quantity))
                {
                    yield return new { Name = parts[0].Trim(), Quantity = quantity };
                }
            }
        }
        private void UpdateRecentOrdersTable(List<OrderHistoryItem> filteredData)
        {
            // --- 1. ตั้งค่าคอลัมน์ (ทำแค่ครั้งเดียว) ---
            if (dgvRecentOrders.Columns.Count == 0) // ตรวจสอบว่ายังไม่ได้ตั้งค่าคอลัมน์
            {
                dgvRecentOrders.AutoGenerateColumns = false;
                dgvRecentOrders.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR", 14F, System.Drawing.FontStyle.Bold);
                dgvRecentOrders.DefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR Light", 11F);
                dgvRecentOrders.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "OrderDateTime",
                    HeaderText = "เวลา",
                    Width = 120,
                    DefaultCellStyle = { Format = "dd/MM HH:mm" }
                });
                
                dgvRecentOrders.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "CustomerName",
                    HeaderText = "ลูกค้า",
                    Width = 120,
                });
                dgvRecentOrders.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "TotalSum",
                    HeaderText = "ยอดรวม",
                    Width = 100,
                    DefaultCellStyle = { Format = "N2" }
                });
            }

            // --- 2. ตรวจสอบข้อมูลที่ได้รับมา ---
            if (filteredData == null || !filteredData.Any())
            {
                // ถ้าไม่มีข้อมูล (เช่น ค้นหาไม่เจอ หรือกดรีเซ็ต) ให้ล้างตาราง
                dgvRecentOrders.DataSource = null;
                return;
            }

            // --- 3. จัดเรียงข้อมูลและแสดงผล ---
            var displayData = filteredData
                .OrderByDescending(order => order.OrderDateTime)
                .ToList();

            // ผูกข้อมูลที่จัดเรียงแล้วเข้ากับ DataGridView
            dgvRecentOrders.DataSource = null;
            dgvRecentOrders.DataSource = displayData;
        }
        private void btnResetChart_Click(object sender, EventArgs e)
        {
            // รีเซ็ตตัวกรองเป็นค่าเริ่มต้น
            cmbChartFilter.SelectedIndex = 0; // รายวัน
            dtpHourlyChartDate.Value = reportDtpStartDate.Value; // ตั้งค่า DatePicker เป็นวันแรกของช่วงที่ค้นหา
            // อัปเดตแดชบอร์ดด้วยข้อมูลเต็ม
            UpdateDashboard(_fullReportData);

        }

        private void cmbChartFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ตรวจสอบว่าผู้ใช้เลือก "รายชั่วโมง" หรือไม่
            if (cmbChartFilter.SelectedItem.ToString() == "รายชั่วโมง")
            {
                // ถ้าใช่, ให้แสดง DatePicker และตั้งค่าเริ่มต้นเป็นวันแรกของช่วงที่ค้นหา
                dtpHourlyChartDate.Visible = true;
                dtpHourlyChartDate.Value = reportDtpStartDate.Value; // ตั้งค่าเริ่มต้น
            }
            else
            {
                // ถ้าเลือกอย่างอื่น ให้ซ่อน DatePicker
                dtpHourlyChartDate.Visible = false;
            }
        }

        //ฝั่งขวา
        private void SetupReportDataGridView()

        {
            // --- 1. ตั้งค่า Date Pickers ให้เป็น ค.ศ. เสมอ ---
            
            reportDtpStartDate.Format = DateTimePickerFormat.Custom;
            reportDtpStartDate.CustomFormat = "d MMMM yyyy"; // e.g., 8 October 2025

            reportDtpEndDate.Format = DateTimePickerFormat.Custom;
            reportDtpEndDate.CustomFormat = "d MMMM yyyy"; // e.g., 8 October 2025
                                                           // ตั้งค่าเริ่มต้น: 1 ปีย้อนหลัง
            reportDtpStartDate.Value = DateTime.Now.AddYears(-1);
            reportDtpEndDate.Value = DateTime.Now;

            // ตั้งค่าเริ่มต้น ComboBox
            cmbChartFilter.SelectedIndex = 0; // รายวัน

            // โหลดข้อมูลครั้งแรก
            reportBtnSearch_Click(null, null);

            // ตั้งค่าเริ่มต้น (ให้ครอบคลุม 1 เดือนย้อนหลัง)
            reportDtpStartDate.Value = DateTime.Now.AddYears(-1);
            reportDtpEndDate.Value = DateTime.Now;
            

            reportDgv.AutoGenerateColumns = false;
            reportDgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR", 14F, System.Drawing.FontStyle.Bold);
            reportDgv.DefaultCellStyle.Font = new System.Drawing.Font("MN ECLAIR Light", 11F);
            reportDgv.Columns.Clear();
            reportDgv.ReadOnly = true;

            // สร้างคอลัมน์ให้ตรงกับ SalesReportItem
            reportDgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Rank", HeaderText = "อันดับที่", Width = 60 });
            reportDgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "รายการ", Width = 150 });
            reportDgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PricePerUnit", HeaderText = "ราคาต่อหน่วย", Width = 100, DefaultCellStyle = { Format = "N2" } });
            reportDgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalQuantitySold", HeaderText = "จำนวนที่ขายได้", Width = 120 });
            reportDgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalSales", HeaderText = "ยอดขาย", Width = 100, DefaultCellStyle = { Format = "N2" } });
            }

        private void reportBtnSearch_Click(object sender, EventArgs e)
        {
            DateTime startDate = reportDtpStartDate.Value;
            DateTime endDate = reportDtpEndDate.Value;

            // ดึงข้อมูล "ประวัติการสั่งซื้อแบบสรุปรวม" สำหรับ Dashboard ฝั่งซ้าย
            _fullReportData = Connect_db.GetGroupedOrderHistory(startDate, endDate);

            // ดึงข้อมูล "รายงานสินค้าขายดี" สำหรับตารางฝั่งขวา
            currentReportData = Connect_db.GetSalesReport(startDate, endDate);

            // แสดงผลตารางสินค้าขายดี (ฝั่งขวา)
            reportDgv.DataSource = null;
            reportDgv.DataSource = currentReportData;

            // ตรวจสอบและอัปเดต Dashboard (ฝั่งซ้าย)
            if (_fullReportData != null && _fullReportData.Any())
            {
                // แสดงยอดขายรวมฝั่งขวา
                decimal grandTotal = currentReportData.Sum(item => item.TotalSales);
                reportTotalRevenueLabel.Text = $"ยอดขายรวม : {grandTotal:N2} บาท";
                reportTotalRevenueLabel.Visible = true;

                // อัปเดต Dashboard ฝั่งซ้ายด้วยข้อมูลใหม่
                UpdateDashboard(_fullReportData);
            }
            else
            {
                MessageBox.Show("ไม่พบข้อมูลรายงานในช่วงวันที่ที่เลือก", "No Data Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                reportTotalRevenueLabel.Visible = false;
                // เคลียร์ Dashboard ฝั่งซ้าย
                UpdateDashboard(null);

            }
        }

       
        private void reportBtnReset_Click(object sender, EventArgs e)
        {
            reportDtpStartDate.Value = DateTime.Now.AddYears(-1);
            reportDtpEndDate.Value = DateTime.Now;
            reportDgv.DataSource = null;
            currentReportData = null;
            
            reportTotalRevenueLabel.Visible = false;
            reportBtnSearch_Click(null, null);

            cmbChartFilter.SelectedIndex = 0; // รายวัน
            dtpHourlyChartDate.Value = reportDtpStartDate.Value; // ตั้งค่า DatePicker เป็นวันแรกของช่วงที่ค้นหา
            UpdateDashboard(_fullReportData);

        }
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่ามีข้อมูลพร้อมส่งออกหรือไม่ (ใช้ข้อมูลชุดเดียวกับที่แสดงบน UI)
            if (_fullReportData == null || !_fullReportData.Any() || currentReportData == null || !currentReportData.Any())
            {
                MessageBox.Show("กรุณาค้นหาข้อมูลเพื่อสร้างรายงานก่อน", "ไม่มีข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // เรียกใช้เมธอดสร้าง Excel โดยส่งข้อมูลทั้ง 2 ชุดเข้าไป
                CreateExcelReport(_fullReportData, currentReportData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการสร้างไฟล์ Excel:\nอาจจะเกิดจากคุณยังไม่ได้ติดตั้ง Microsoft Excel หรือมีปัญหาในการเข้าถึง\n\nError: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        // --- เมธอดหลักสำหรับสร้างไฟล์ Excel ---
        private void CreateExcelReport(List<OrderHistoryItem> orderHistory, List<SalesReportItem> salesSummary)
        {
            // --- 1. เตรียมโปรแกรม Excel ---
            Excel.Application excelApp = new Excel.Application();
            if (excelApp == null)
            {
                MessageBox.Show("ไม่สามารถเปิดโปรแกรม Microsoft Excel ได้ กรุณาตรวจสอบว่าติดตั้งโปรแกรมเรียบร้อยแล้ว", "Excel Error");
                return;
            }
            excelApp.Visible = true;
            Excel.Workbook workbook = excelApp.Workbooks.Add();

            // --- 2. สร้าง Worksheet ที่ 1: ประวัติการสั่งซื้อ ---
            Excel._Worksheet sheet1 = (Excel._Worksheet)workbook.Sheets[1];
            sheet1.Name = "ประวัติการสั่งซื้อ";

            // --- จัดรูปแบบ Sheet 1 ---
            // ใส่หัวข้อหลัก
            sheet1.Cells[1, 1] = "ประวัติการสั่งซื้อ";
            sheet1.Range["A1:G1"].Merge();
            sheet1.Range["A1"].Font.Bold = true;
            sheet1.Range["A1"].Font.Size = 18;
            sheet1.Range["A1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            sheet1.Cells[2, 1] = $"ช่วงวันที่: {reportDtpStartDate.Value:d MMMM yyyy} - {reportDtpEndDate.Value:d MMMM yyyy}";
            sheet1.Range["A2:G2"].Merge();
            sheet1.Range["A2"].Font.Bold = true;
            sheet1.Range["A2"].Font.Italic = false;

            sheet1.Cells[3, 1] = $"จำนวนออเดอร์ทั้งหมด: {orderHistory.Count} ออเดอร์";
            sheet1.Range["A3:G3"].Merge();
            sheet1.Range["A3"].Font.Italic = true;

            // ใส่หัวตาราง (เริ่มที่แถวที่ 5)
            string[] historyHeaders = { "Date/Time", "Customer", "Table", "Items", "Toppings", "Total" };
            for (int i = 0; i < historyHeaders.Length; i++)
            {
                sheet1.Cells[5, i + 1] = historyHeaders[i];
            }

            // จัดรูปแบบหัวตาราง
            Excel.Range historyHeaderRange = sheet1.Range["A5", sheet1.Cells[5, historyHeaders.Length]];
            historyHeaderRange.Font.Bold = true;
            historyHeaderRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
            historyHeaderRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
         
            // ใส่ข้อมูล (เริ่มที่แถวที่ 6)
            for (int i = 0; i < orderHistory.Count; i++)
            {
                var item = orderHistory[i];
                int currentRow = i + 6;
                //จัดตัวหนังสือให้อยู่แบบ Top Left
                sheet1.Rows[currentRow].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                // ใส่ข้อมูลแต่ละคอลัมน์
                sheet1.Cells[currentRow, 1] = item.OrderDateTime.ToString("dd/MM/yyyy HH:mm");
                sheet1.Cells[currentRow, 2] = item.CustomerName;
                sheet1.Cells[currentRow, 3] = item.TableCustomer;
                sheet1.Cells[currentRow, 4] = item.ItemsSummary;
                sheet1.Cells[currentRow, 5] = item.ToppingsSelected;
                sheet1.Cells[currentRow, 6] = item.TotalSum;
            }
            int lastSheetRow = orderHistory.Count + 6;
            sheet1.Cells[lastSheetRow, 5].Font.Bold = true;
            sheet1.Cells[lastSheetRow, 5].Value = "ยอดรวมทั้งหมด:";
            sheet1.Cells[lastSheetRow, 6].Font.Bold = true;
            sheet1.Cells[lastSheetRow, 6].Formula = $"=SUM(F4:F{lastSheetRow - 1})"; // ใช้สูตร Excel
            sheet1.Range[$"E{lastSheetRow}"].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            sheet1.Range[$"E{lastSheetRow}"].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
            
            // จัดรูปแบบคอลัมน์ที่เป็นตัวเลข
            int lastHistoryRow = orderHistory.Count + 6;
            sheet1.Range[$"F5:F{lastHistoryRow}"].NumberFormat = "#,##0.00";

            // ปรับขนาดคอลัมน์อัตโนมัติ
            sheet1.Columns.AutoFit();
            sheet1.Rows.WrapText = true; // ทำให้ข้อความยาวๆ ขึ้นบรรทัดใหม่


            // --- 3. สร้าง Worksheet ที่ 2: สรุปยอดขาย ---
            Excel._Worksheet sheet2 = (Excel._Worksheet)workbook.Worksheets.Add(After: sheet1);
            sheet2.Name = "สรุปยอดขายสินค้า";

            // --- จัดรูปแบบ Sheet 2 ---
            sheet2.Cells[1, 1] = "สรุปยอดขายสินค้า";
            sheet2.Range["A1:E1"].Merge();
            sheet2.Range["A1"].Font.Bold = true;
            sheet2.Range["A1"].Font.Size = 18;
            sheet2.Range["A1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            sheet2.Cells[2, 1] = $"ช่วงวันที่: {reportDtpStartDate.Value:d MMMM yyyy} - {reportDtpEndDate.Value:d MMMM yyyy}";
            sheet2.Range["A2:G2"].Merge();
            sheet2.Range["A2"].Font.Bold = true;
            sheet2.Range["A2"].Font.Italic = false;

            // ใส่หัวตาราง (เริ่มที่แถวที่ 4)
            string[] salesHeaders = { "อันดับ", "รายการสินค้า", "ราคาต่อหน่วย", "จำนวนที่ขายได้", "ยอดขายรวม" };
            for (int i = 0; i < salesHeaders.Length; i++)
            {
                sheet2.Cells[4, i + 1] = salesHeaders[i];
            }

            // จัดรูปแบบหัวตาราง
            Excel.Range salesHeaderRange = sheet2.Range["A4", sheet2.Cells[4, salesHeaders.Length]];
            salesHeaderRange.Font.Bold = true;
            salesHeaderRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);

            // ใส่ข้อมูล (เริ่มที่แถวที่ 4)
            for (int i = 0; i < salesSummary.Count; i++)
            {
                var item = salesSummary[i];
                int currentRow = i + 5;
                sheet2.Cells[currentRow, 1] = item.Rank;
                sheet2.Cells[currentRow, 2] = item.ItemName;
                sheet2.Cells[currentRow, 3] = item.PricePerUnit;
                sheet2.Cells[currentRow, 4] = item.TotalQuantitySold;
                sheet2.Cells[currentRow, 5] = item.TotalSales;
            }

            // ใส่แถวสรุปยอดรวม
            int lastSalesRow = salesSummary.Count + 5;
            sheet2.Cells[lastSalesRow, 4].Font.Bold = true;
            sheet2.Cells[lastSalesRow, 4].Value = "ยอดรวมทั้งหมด:";
            sheet2.Cells[lastSalesRow, 5].Font.Bold = true;
            sheet2.Cells[lastSalesRow, 5].Formula = $"=SUM(E4:E{lastSalesRow - 1})"; // ใช้สูตร Excel
            sheet2.Range[$"E{lastSalesRow}"].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            sheet2.Range[$"E{lastSalesRow}"].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;

            // จัดรูปแบบคอลัมน์ตัวเลข
            sheet2.Range[$"C4:C{lastSalesRow}"].NumberFormat = "#,##0.00";
            sheet2.Range[$"E4:E{lastSalesRow}"].NumberFormat = "#,##0.00";

            // ปรับขนาดคอลัมน์อัตโนมัติ
            sheet2.Columns.AutoFit();


            sheet1.Activate();
        }
        private void reportBtnPrint_Click(object sender, EventArgs e)
        {

        }


  



        private void leftButtonPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void searchPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void btnReset_Click(object sender, EventArgs e)
        {

        }
        private void SplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }
        private void SplitContainer_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedTopping == null) // ตรวจสอบตัวแปรนี้โดยตรง
            {
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการแก้ไขจากตาราง", "ยังไม่ได้เลือกรายการ");
                return;
            }
            _selectedTopping.Name = txtName_Topping.Text;
            _selectedTopping.Type = cmbType_Topping.SelectedItem.ToString();
            _selectedTopping.Amount = Convert.ToInt32(numAmount_Topping.Value);
            if (Connect_db.UpdateTopping(_selectedTopping))
            {
                LoadToppings();
                EnterNewMode_Topping();
            }
        }
        private void btnEdit_Topping_Click(object sender, EventArgs e)
        {



            if (dgvProducts.SelectedRows == null)
            {
                MessageBox.Show("กรุณาเลือกสินค้าที่ต้องการแก้ไข", "ยังไม่ได้เลือกรายการ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // ออกจากเมธอดไปเลยถ้ายังไม่เลือก
            }
            else
            {

                lblEditMode.Visible = true;

                btnConfirm_Topping.BackColor = Color.Goldenrod;
                btnConfirm_Topping.Enabled = true;
                EnterEditMode(GetNumAmount_Topping());
            }
            _selectedTopping.Name = txtName_Topping.Text;
            _selectedTopping.Type = cmbType_Topping.SelectedItem.ToString();
            _selectedTopping.Amount = (int)numAmount_Topping.Value;

            // ส่งไปอัปเดตในฐานข้อมูล
            if (Connect_db.UpdateTopping(_selectedTopping))
            {
                int selectedRowIndex = dgvToppings.SelectedRows[0].Index; // จำตำแหน่งแถวที่เลือกไว้
                LoadToppings(); // โหลดข้อมูลใหม่
                EnterNewMode_Topping(); // รีเซ็ตฟอร์มด้านขวา

               
                if (dgvToppings.Rows.Count > selectedRowIndex)
                {
                    dgvToppings.Rows[selectedRowIndex].Selected = true;
                }
            }

        }

        private void backadmin_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }





        // เมธอดสำหรับเปิด/ปิด ปุ่มแก้ไขและลบ
        private void EnableEditMode(bool isEditing)
        {
            btnEdit_Topping.Enabled = isEditing;
            btnDelete_Topping.Enabled = isEditing;
        }


    }
    
}





