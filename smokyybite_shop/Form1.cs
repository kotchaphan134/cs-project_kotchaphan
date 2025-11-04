using _1;
using MySqlX.XDevAPI;
using smokyybite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace smokyybite
{
    public partial class Smokyybite : Form
    {
        private AppSession currentSession = new AppSession();
        private Dictionary<string, List<Topping>> availableToppings;
        private Panel toppingControlPanel;
   
        private Panel globalVeggiePanel;
        private Panel globalSaucePanel;

        public Smokyybite()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // โหลดข้อมูล Toppings มาเก็บไว้ในตัวแปรก่อนเป็นอันดับแรก
            availableToppings = Connect_db.GetToppingsByType();


            // โหลดข้อมูลจากฐานข้อมูลมาแสดงผล
            menuPanel.Width = this.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
          
            // โหลดข้อมูลสินค้าและสร้าง UI
            CreateGlobalToppingControls();
            LoadAndDisplayMenu();
            RefreshAllData(false);

        }
        private void RefreshAllData(bool clearCart)
        {
            if (clearCart)
            {
                currentSession.ClearCart(); // เรียกใช้เมธอดเคลียร์ตะกร้าใน AppSession
                UpdateCartUI();             // อัปเดตตัวเลขบนปุ่มตะกร้าให้เป็น (0)
            }

         
            availableToppings = Connect_db.GetToppingsByType();

            if (toppingControlPanel != null)
            {
                menuPanel.Controls.Remove(toppingControlPanel);
                toppingControlPanel.Dispose();
                nameTextBox.Clear();
                tableComboBox.SelectedIndex = -1;
                customerInputPanel.Visible = true;
                displayPanel.Visible = false;
                editCustomerLink.Visible = false;
                tableComboBox.Items.Clear();
                for (int i = 1; i <= 10; i++)
                {
                    tableComboBox.Items.Add(i.ToString());
                }

                customerInfoLabel.Visible = false;
            }


            CreateGlobalToppingControls();
            LoadAndDisplayMenu();
        }
        // ------------------------------แถบบาร์------------------------------------------
        //ปุ่มแอดมิน
        private void aboutbt_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new aboutme())
            {
                aboutForm.ShowDialog(this);

            }
        }
        private void adminButton_Click(object sender, EventArgs e)
        {
            using (var loginForm = new AdminLoginForm())
            {
                if (loginForm.ShowDialog(this) == DialogResult.OK) // ตรวจสอบว่าล็อกอินสำเร็จมั้ย
                {
                    // ถ้าสำเร็จ ให้เปิดหน้า Admin Panel
                    using (var adminPanel = new AdminPanelForm())
                    {
                        adminPanel.ShowDialog(this);
                    }
                    // ให้โหลดเมนู
                    LoadAndDisplayMenu();
                }
            }
        }
        //ปุ่มตะกร้า
        private void cartButton_Click(object sender, EventArgs e)
        {
            // ตรวจสอบข้อมูลลูกค้า
            if (string.IsNullOrEmpty(currentSession.CustomerName))
            {
                MessageBox.Show("กรุณากรอกข้อมูลลูกค้าก่อน", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ตรวจสอบสินค้าในตะกร้า
            if (!currentSession.Cart.Any())
            {
                MessageBox.Show("ตะกร้าของคุณว่างเปล่า", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // ตรวจสอบ Topping 

            // ---- ตรวจสอบผัก ----
            if (currentSession.ToppingDecisionMade["Vegetable"]) 
            {
                // ตรวจสอบเฉพาะเมื่อเคยกดไปแล้ว
                var pendingVeggieCount = globalVeggiePanel.Controls.Find("checkBoxesPanel", true)
                                                      .FirstOrDefault()?
                                                      .Controls.OfType<FlowLayoutPanel>()
                                                      .SelectMany(pnl => pnl.Controls.OfType<CheckBox>())
                                                      .Count(c => c.Checked) ?? 0;

                if (pendingVeggieCount != currentSession.SelectedToppings["Vegetable"].Count)
                {
                    MessageBox.Show("คุณมีการเปลี่ยนแปลงการเลือก 'ผัก' แต่ยังไม่ได้กดยืนยัน", "กรุณายืนยันการเลือก");
                    return;
                }
            }
            else // ถ้ายังไม่เคยตัดสินใจเลย
            {
                var result = MessageBox.Show("คุณยังไม่ได้เลือก 'ผัก'\n\nไปที่หน้าตะกร้าโดยไม่เพิ่มผักใช่หรือไม่?", "ยืนยันการไม่เลือก", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }

            // ---- ตรวจสอบซอส ----
            if (currentSession.ToppingDecisionMade["Sauce"]) // ตรวจสอบเฉพาะเมื่อเคย "ตัดสินใจ" ไปแล้ว
            {
                var pendingSauceCount = globalSaucePanel.Controls.Find("checkBoxesPanel", true)
                                                     .FirstOrDefault()?
                                                     .Controls.OfType<FlowLayoutPanel>()
                                                     .SelectMany(pnl => pnl.Controls.OfType<CheckBox>())
                                                     .Count(c => c.Checked) ?? 0;

                if (pendingSauceCount != currentSession.SelectedToppings["Sauce"].Count)
                {
                    MessageBox.Show("คุณมีการเปลี่ยนแปลงการเลือก 'ซอส' แต่ยังไม่ได้กดยืนยัน", "กรุณายืนยันการเลือก");
                    return;
                }
            }
            else // ถ้ายังไม่เคยตัดสินใจเลย
            {
                var result = MessageBox.Show("คุณยังไม่ได้เลือก 'ซอส'\n\nไปที่หน้าตะกร้าโดยไม่เพิ่มซอสใช่หรือไม่?", "ยืนยันการไม่เลือก", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }


            // ถ้าผ่านทั้งหมด ให้เปิดฟอร์มตะกร้า
            using (var cartForm = new CartForm(currentSession))
            {
                cartForm.CartUpdated += (s, ev) => UpdateCartUI();
                DialogResult result = cartForm.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    // รีเซ็ตทุกอย่าง
                    RefreshAllData(true);
                }
                else if (result == CartForm.CartClearedResult)
                {
                    ResetToppingsButton_Click(null, null);
                    UpdateCartUI();
                    MessageBox.Show("ตะกร้าถูกล้างแล้ว", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
   

        // ------------------------------หน้ากรอกข้อมูลลูกค้า------------------------------------------
        // ปุ่มบันทึกข้อมูลลูกค้า
        private void saveCustomerButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) || tableComboBox.SelectedItem == null)
            {
                MessageBox.Show("กรุณากรอกชื่อและเลือกโต๊ะ", "ข้อมูลไม่ครบถ้วน");
                return;
            }

            // บันทึกข้อมูล
            currentSession.CustomerName = nameTextBox.Text;
            currentSession.TableNumber = tableComboBox.SelectedItem.ToString();

            // อัปเดต Label ที่จะแสดงผล
            customerInfoLabel.Text = $"👤 {currentSession.CustomerName}   |   🪑 {currentSession.TableNumber}";

            // สลับการแสดงผล
            customerInputPanel.Visible = false;
            displayPanel.Visible = true;
            editCustomerLink.Visible = true;
            customerInfoLabel.Visible = true;
        }
        // ปุ่มแก้ไขข้อมูลลูกค้า
        private void editCustomerLink_Click(object sender, EventArgs e)
        {
            customerInputPanel.Visible = true;
            displayPanel.Visible = false;
            editCustomerLink.Visible = false;
            customerInfoLabel.Visible = false;
        }

  
        //-----------------------topping-------------------------------
        // สร้าง Panel สำหรับ Toppings 
      
        private Panel CreateToppingPanel(string title, List<Topping> toppings, int selectionLimit)
        {
            string toppingType = title.Contains("ผัก") ? "Vegetable" : "Sauce";
            Panel mainPanel = new Panel { Width = 480, Height = 160, BackColor = Color.Transparent, Padding = new Padding(10) };
            Label titleLabel = new Label { Text = title, Location = new Point(5, 5), AutoSize = true, Font = new Font("MN ECLAIR", 12F, FontStyle.Bold) };
            FlowLayoutPanel checkBoxesPanel = new FlowLayoutPanel { Location = new Point(5, 50), Size = new Size(400, 130), Visible = false, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Name = "checkBoxesPanel", Font = new Font("MN ECLAIR Light", 12F), AutoScroll = true };
            Button confirmButton = new Button { Text = "ยืนยัน", Location = new Point(370, 5), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(100, 30), Font = new Font("MN ECLAIR", 10F, FontStyle.Bold), Visible = false };
            Button btnChoose = new Button { Text = "เลือก", Location = new Point(230, 5), BackColor = Color.DarkGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(60, 30), Font = new Font("MN ECLAIR", 10F, FontStyle.Bold) };
            Button btnDecline = new Button { Text = "ไม่ต้องการ", Location = new Point(295, 5), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(80, 30), Font = new Font("MN ECLAIR", 10F, FontStyle.Bold) };
            Label statusLabel = new Label { Text = "ยังไม่ได้เลือก", Location = new Point(5, 30), AutoSize = true, Font = new Font("MN ECLAIR Light", 10F), ForeColor = Color.Gray };

            btnChoose.Click += (s, e) =>
            {
                checkBoxesPanel.Visible = true;
                
            };

            btnDecline.Click += (s, e) =>
            {
                // ซ่อน panel ที่มี checkbox
                checkBoxesPanel.Visible = false;

                // เคลียร์ checkbox ทั้งหมด 
                foreach (CheckBox chk in checkBoxesPanel.Controls.OfType<FlowLayoutPanel>().SelectMany(pnl => pnl.Controls.OfType<CheckBox>()))
                {
                    chk.Checked = false;
                }

               
                // เคลียร์ Topping ที่เคยเลือกไว้ใน Session
                currentSession.SelectedToppings[toppingType].Clear();

                // ตั้งสถานะว่า "ตัดสินใจแล้ว" (ว่าไม่ต้องการ)
                currentSession.ToppingDecisionMade[toppingType] = true;

                // อัปเดต UI ให้ผู้ใช้เห็น
                statusLabel.Text = "ไม่ต้องการ";
                statusLabel.ForeColor = Color.DarkRed;
                confirmButton.Visible = false; // ซ่อนปุ่มยืนยัน

                MessageBox.Show($"ยืนยันการไม่เพิ่ม '{title}'", "ยืนยัน", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            confirmButton.Click += (s, e) =>
            {
              
                // เคลียร์รายการเก่าใน Session ก่อน เพื่อเตรียมรับข้อมูลใหม่
                currentSession.SelectedToppings[toppingType].Clear();

                // วนลูปใน CheckBox ทั้งหมดที่อยู่ใน FlowLayoutPanel ย่อยๆ
                foreach (CheckBox chk in checkBoxesPanel.Controls.OfType<FlowLayoutPanel>().SelectMany(pnl => pnl.Controls.OfType<CheckBox>()))
                {
                    // ถ้า CheckBox ไหนถูกติ๊ก
                    if (chk.Checked)
                    {
                        // ให้เพิ่มข้อมูล Topping (ที่เก็บไว้ใน Tag) ลงใน List ของ Session
                        currentSession.SelectedToppings[toppingType].Add((Topping)chk.Tag);
                    }
                }

                // ตั้งสถานะว่า "ตัดสินใจแล้ว"
                currentSession.ToppingDecisionMade[toppingType] = true;

                // อัปเดต UI ให้ผู้ใช้เห็น
                statusLabel.Text = $"เลือกแล้ว {currentSession.SelectedToppings[toppingType].Count} อย่าง";
                statusLabel.ForeColor = Color.DarkGreen;
                confirmButton.Visible = false; // ซ่อนปุ่มยืนยันหลังจากกดยืนยันแล้ว

                MessageBox.Show($"บันทึกการเลือก '{title}' เรียบร้อยแล้ว", "ยืนยัน", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };


            // ส่วนสร้าง CheckBox 
            if (toppings != null && toppings.Count > 0)
            {
                FlowLayoutPanel columnPanel = null;
                for (int i = 0; i < toppings.Count; i++)
                {
                    // สร้าง Column Panel ทุกๆ 3 รายการ
                    if (i % 3 == 0)
                    {
                        columnPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Margin = new Padding(0, 0, 10, 0) };
                        checkBoxesPanel.Controls.Add(columnPanel);
                    }

                    var topping = toppings[i];
                    bool isOutOfStock = topping.Amount <= 0; // ตรวจสอบสถานะสต็อก

                    // *** สร้าง CheckBox ***
                    CheckBox chk = new CheckBox
                    {
                        Text = topping.Name + (isOutOfStock ? " (หมด)" : ""), // <<< เพิ่มข้อความ (หมด)
                        Tag = topping,
                        AutoSize = true,
                        Margin = new Padding(3, 3, 3, 3),
                        Enabled = !isOutOfStock, // <<< ปิดการใช้งานถ้าหมด
                        ForeColor = isOutOfStock ? Color.DarkGray : Color.Black // <<< เปลี่ยนสีถ้าหมด
                    };

                    // ผูก Event CheckedChanged
                    chk.CheckedChanged += (s, e) =>
                    {
                        int checkedCount = 0;
                        // นับ CheckBox ที่ถูกเลือกทั้งหมด
                        foreach (FlowLayoutPanel pnl in checkBoxesPanel.Controls.OfType<FlowLayoutPanel>())
                        {
                            checkedCount += pnl.Controls.OfType<CheckBox>().Count(c => c.Checked);
                        }

                        // ตรวจสอบลิมิต
                        if (checkedCount > selectionLimit)
                        {
                            MessageBox.Show($"สามารถเลือกได้สูงสุด {selectionLimit} ชนิด", "จำกัดจำนวน");
                            ((CheckBox)s).Checked = false; // ยกเลิกการเลือกอันล่าสุด
                            return;
                        }

                        // อัปเดตสถานะ UI
                        statusLabel.Text = $"เลือก {checkedCount} จาก {selectionLimit}";
                        statusLabel.ForeColor = Color.Red;
                        confirmButton.Visible = true;
                    };
                    if (columnPanel != null) { columnPanel.Controls.Add(chk); }
                }
            }

            mainPanel.Controls.AddRange(new Control[] { titleLabel, statusLabel, btnChoose, btnDecline, confirmButton, checkBoxesPanel });
            return mainPanel;
        }
        // สร้าง Controls สำหรับ Toppings แบบ Global
        private void CreateGlobalToppingControls()
        {
            // สร้าง Container หลัก
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Top,
                Width = menuPanel.ClientSize.Width - 50,
                Margin = new Padding(15), // เอา Margin ออกเพื่อให้ชิดขอบ
                Height = 180, // เพิ่มความสูงเล็กน้อย
                BackColor = Color.Bisque,
                Padding = new Padding(30)
            };

            // สร้าง FlowLayoutPanel สำหรับจัดเรียง
            FlowLayoutPanel flowContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Anchor = AnchorStyles.None
            };

            // สร้าง Panel ผักและซอส
            globalVeggiePanel = CreateToppingPanel("🥬 เพิ่มผัก (ฟรี)-3 ชนิด", availableToppings.ContainsKey("Vegetable") ? availableToppings["Vegetable"] : new List<Topping>(), 3);
            globalSaucePanel = CreateToppingPanel("🍯 เพิ่มซอส (ฟรี)-2 ชนิด", availableToppings.ContainsKey("Sauce") ? availableToppings["Sauce"] : new List<Topping>(), 2);


            flowContainer.Controls.Add(globalVeggiePanel);
            flowContainer.Controls.Add(globalSaucePanel);

            // เพิ่ม FlowLayoutPanel ลงใน Container หลัก
            mainContainer.Controls.Add(flowContainer);

            // จัดให้อยู่กลาง
            mainContainer.Resize += (s, e) =>
            {
                flowContainer.Location = new Point((mainContainer.Width - flowContainer.Width) / 2, (mainContainer.Height - flowContainer.Height) / 2);
            };

            toppingControlPanel = mainContainer; // อัปเดตตัวแปรระดับคลาส
            menuPanel.Controls.Add(toppingControlPanel);
        }

    
        private void ResetToppingsButton_Click(object sender, EventArgs e)
        {
            // --- รีเซ็ต Panel ผัก ---
            if (globalVeggiePanel != null)
            {
                var checkBoxesPanel = globalVeggiePanel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                var statusLabel = globalVeggiePanel.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("เลือก") || l.Text.Contains("ไม่ต้องการ"));
                var confirmButton = globalVeggiePanel.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "ยืนยัน");

                if (checkBoxesPanel != null)
                {
                    checkBoxesPanel.Visible = false;
                    foreach (CheckBox chk in checkBoxesPanel.Controls.OfType<CheckBox>()) { chk.Checked = false; }
                }
                if (statusLabel != null) { statusLabel.Text = "ยังไม่ได้เลือก"; statusLabel.ForeColor = Color.Gray; }
                if (confirmButton != null) confirmButton.Visible = false;
            }

            // --- รีเซ็ต Panel ซอส ---
            if (globalSaucePanel != null)
            {
                var checkBoxesPanel = globalSaucePanel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                var statusLabel = globalSaucePanel.Controls.OfType<Label>().FirstOrDefault(l => l.Text.Contains("เลือก") || l.Text.Contains("ไม่ต้องการ"));
                var confirmButton = globalSaucePanel.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "ยืนยัน");

                if (checkBoxesPanel != null)
                {
                    checkBoxesPanel.Visible = false;
                    foreach (CheckBox chk in checkBoxesPanel.Controls.OfType<CheckBox>()) { chk.Checked = false; }
                }
                if (statusLabel != null) { statusLabel.Text = "ยังไม่ได้เลือก"; statusLabel.ForeColor = Color.Gray; }
                if (confirmButton != null) confirmButton.Visible = false;
            }

            // เคลียร์ข้อมูลใน Session และรีเซ็ตสถานะการตัดสินใจ
            currentSession.Clear(); 
        }

        //------------------------------แสดงเมนูสินค้า------------------------------------------
        public void LoadAndDisplayMenu(string searchTerm = "")
        {
            var cardsToRemove = menuPanel.Controls.OfType<Panel>().Where(p => p != toppingControlPanel).ToList();
            foreach (var card in cardsToRemove)
            {
                menuPanel.Controls.Remove(card);
            }

            // ดึงข้อมูลสินค้าทั้งหมดจากฐานข้อมูล
            var allMenuItems = Connect_db.GetMenuItems();

            // กรองข้อมูลตามคำค้นหา 
            List<MenuItem> filteredItems;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // ถ้าไม่มีคำค้นหา ให้ใช้ข้อมูลทั้งหมด
                filteredItems = allMenuItems;
            }
            else
            {
                // ถ้ามีคำค้นหา ให้กรองเฉพาะสินค้าที่มี "ชื่อ" (Name) ตรงกับคำค้นหา
                // .ToLower() ทำให้ตัวเล็กหมด 
                filteredItems = allMenuItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
            }
            // สร้างการ์ดสำหรับแต่ละสินค้า
            foreach (var item in filteredItems)
            {
                Panel card = new Panel
                {
                    BackColor = Color.White,
                    Width = 250,
                    Height = 350,
                    Margin = new Padding(15)
                };

                PictureBox picBox = new PictureBox { Image = item.Photo, SizeMode = PictureBoxSizeMode.Zoom, Size = new Size(230, 150), Location = new Point(10, 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.WhiteSmoke };
                if (item.Photo == null) { picBox.Paint += (s, ev) => { ev.Graphics.DrawString("No Image", this.Font, Brushes.Gray, new PointF(100, 65)); }; }

                Label nameLabel = new Label { Text = item.Name, Font = new Font("MN ECLAIR", 14F, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#ea580c"), AutoSize = true, MaximumSize = new Size(260, 0), Location = new Point(10, 170) };
                card.Controls.Add(nameLabel);

                Label descLabel = new Label { Text = item.Description, Font = new Font("MN ECLAIR", 10F), AutoSize = true, MaximumSize = new Size(260, 0), Location = new Point(10, nameLabel.Bottom + 5) };



                // สร้าง Label ราคา
                Label priceLabel = new Label
                {
                    Text = $"฿{item.Price:N2}",
                    Font = new Font("MN ECLAIR", 14F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml("#ea580c"),
                    AutoSize = true

                };


                // สร้างจำนวนสินค้า
                NumericUpDown quantityInput = new NumericUpDown
                {
                    Minimum = 1,
                   
                    Value = 1,// ค่าเริ่มต้น
                    Width = 60, // ปรับความกว้างให้เหมาะสม
                    Location = new Point(200, 300),
                    Font = new Font("MN ECLAIR", 12F, FontStyle.Bold),
                    TextAlign = HorizontalAlignment.Center,
                    Enabled = item.Stock > 0 // ปิดการใช้งานถ้าสินค้าหมด
                };
                // กำหนดค่า Maximum ตามสต็อก
                if (item.Stock < 10)
                {
                    quantityInput.Maximum = item.Stock > 0 ? item.Stock : 1;
                }// ถ้าสต็อกน้อยกว่า 10 ให้ตั้งค่า Maximum เท่ากับสต็อก


                else
                { 
                    quantityInput.Maximum = 10;
                }
                // สร้างปุ่ม "เพิ่มลงตะกร้า"
                Button addButton = new Button
                {
                    Text = item.Stock > 0 ? "เพิ่มลงตะกร้า" : "สินค้าหมด",
                    Enabled = item.Stock > 0,
                    BackColor = item.Stock > 0 ? Color.Orange : Color.Gray,
                    ForeColor = Color.White,
                    Font = new Font("MN ECLAIR", 9F, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(120, 40) // ปรับขนาดปุ่ม
                };

                // ผูก Event และ Tag 
                addButton.Tag = card;
                addButton.Click += AddToCart_Click;
                card.Tag = new Tuple<MenuItem, NumericUpDown>(item, quantityInput);


                // จัดวางตำแหน่ง Controls 

                // ราคา ยังคงอยู่ที่มุมซ้ายล่าง
                priceLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                priceLabel.Location = new Point(10, card.Height - priceLabel.Height - 15);



                // ปุ่ม "เพิ่มลงตะกร้า" จะอยู่ที่มุมขวาล่างสุด
                addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                addButton.Location = new Point(card.Width - addButton.Width - 10, card.Height - addButton.Height - 10);

                // NumericUpDown จะอยู่เหนือปุ่ม "เพิ่มลงตะกร้า"
                quantityInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                quantityInput.Location = new Point(
                    addButton.Left, // ให้ X ตรงกับปุ่ม
                    addButton.Top - quantityInput.Height - 5
                );
                // ปรับความกว้างของ NumericUpDown
                quantityInput.Width = addButton.Width;

                // แสดงข้อความเตือนถ้าสต็อกน้อยกว่า 10
                if (item.Stock < 10 && item.Stock > 0)
                {
                    Label stockLabel = new Label
                    {
                        Text = $"เหลือ {item.Stock} ชิ้น",
                        Font = new Font("MN ECLAIR Light", 9F),
                        ForeColor = Color.Red,
                        AutoSize = true,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                    };
                    // จัดตำแหน่งให้อยู่เหนือ NumericUpDown
                    stockLabel.Location = new Point(quantityInput.Left+70, quantityInput.Top - stockLabel.Height);
                    card.Controls.Add(stockLabel);
                }
                // เพิ่ม Controls ทั้งหมดลงในการ์ด 
                card.Controls.AddRange(new Control[] { picBox, descLabel, priceLabel, quantityInput, addButton });

                menuPanel.Controls.Add(card);



            }

        }
        // จะทำงานถ้ากดปุ่ม "เพิ่มลงตะกร้า"

        private void AddToCart_Click(object sender, EventArgs e)
        {

            // ---  ดึงข้อมูลที่จำเป็น  ---

            //1 `sender` คือ "ปุ่ม" ที่ถูกคลิก
            Button clickedButton = (Button)sender;

            //2 `Tag` ของปุ่มเก็บ "การ์ด" ทั้งใบ (Panel)
            Panel card = (Panel)clickedButton.Tag;

            //3 `Tag` ของการ์ดเก็บ "ข้อมูล" ที่เราต้องการ (Tuple)
            var cardData = (Tuple<MenuItem, NumericUpDown>)card.Tag;

            MenuItem itemToAdd = cardData.Item1;
            int quantityToAdd = (int)cardData.Item2.Value;

            // ตรวจสอบจำนวนในตะกร้า 

            int quantityAlreadyInCart = currentSession.GetTotalQuantityOfItemById(itemToAdd.Id);
            int totalQuantityAfterAdd = quantityAlreadyInCart + quantityToAdd;

            //1 ตรวจสอบสต็อกคงเหลือจริง
            if (totalQuantityAfterAdd > itemToAdd.Stock)
            {
                MessageBox.Show(
                    $"ขออภัย สินค้า '{itemToAdd.Name}' มีคงเหลือเพียง {itemToAdd.Stock} ชิ้น\n" +
                    $"คุณมีอยู่ในตะกร้าแล้ว {quantityAlreadyInCart} ชิ้น และไม่สามารถเพิ่มอีก {quantityToAdd} ชิ้นได้",
                    "สินค้าไม่เพียงพอ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                cardData.Item2.Value = 1;

                return; // ออกจากฟังก์ชัน ไม่เพิ่มสินค้า
            }
            //2 ตรวจสอบจำนวนสูงสุดที่อนุญาต (10 ชิ้น)
            if (quantityAlreadyInCart + quantityToAdd > 10)
            {
                MessageBox.Show($"ขออภัย คุณมี '{itemToAdd.Name}' อยู่ในตะกร้าแล้ว {quantityAlreadyInCart} ชิ้น\nสามารถสั่งซื้อได้ทั้งหมดไม่เกิน 10 ชิ้น", "จำกัดจำนวน");
                cardData.Item2.Value = 1;

                return;
            }

            // **เรียกใช้ AddToCart
            currentSession.AddToCart(itemToAdd, quantityToAdd);

          

            // รีเซ็ต UI และอัปเดตตะกร้า 
            cardData.Item2.Value = 1;
            UpdateCartUI();

     
        }

        private void UpdateCartUI()
        {
            cartButton.Text = $"🛒 ตะกร้า ({currentSession.Cart.Sum(i => i.Quantity)})";
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (currentSession.Cart.Any())
            {
                var result = MessageBox.Show(
                    "การรีเฟรชจะล้างข้อมูลสินค้าทั้งหมดในตะกร้าของคุณ\n\n" +
                    "คุณต้องการดำเนินการต่อใช่หรือไม่?",
                    "ยืนยันการรีเฟรช",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // ถ้าผู้ใช้ตอบ "No" ให้ออกจากฟังก์ชันทันที
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            // เรียกใช้เมธอด Refresh หลัก โดยส่งค่า true เพื่อบอกให้เคลียร์ตะกร้า
            RefreshAllData(true);

            MessageBox.Show("ข้อมูลได้รับการอัปเดตและตะกร้าถูกล้างแล้ว", "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            // ดึงคำค้นหาปัจจุบันจาก TextBox
            string searchTerm = searchTextBox.Text;

            // เรียกใช้ LoadAndDisplayMenu ใหม่ โดยส่งคำค้นหาเข้าไปด้วย
            LoadAndDisplayMenu(searchTerm);
        }

        private void shut_down_Click_1(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                 "คุณต้องการปิดโปรแกรมใช่หรือไม่?",
                 "ยืนยันการรีเฟรช",
                 MessageBoxButtons.YesNo,
                 MessageBoxIcon.Warning
             );

            // ถ้าผู้ใช้ตอบ "No" ให้ออกจากฟังก์ชันทันที
            if (result == DialogResult.No)
            {
                return;
            }
            else
            {
                Application.Exit();
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void customerInfoLabel_Click(object sender, EventArgs e)
        {

        }

    
    }
}