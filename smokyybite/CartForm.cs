using Google.Protobuf.WellKnownTypes;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace smokyybite
{
    public partial class CartForm : Form
    {
        private AppSession _session;
        public event EventHandler CartUpdated; // Event สำหรับส่งสัญญาณกลับไป
        public static readonly DialogResult CartClearedResult = DialogResult.Retry;
        // โค้ดใน Constructor จะถูกสร้างโดย Designer เมื่อเราดับเบิลคลิก Controls
        public CartForm(AppSession session)
        {
            
            InitializeComponent();
            // เก็บ session ที่ส่งมา
            _session = session;
            // ผูก Event ของฟอร์ม
            this.Load += CartForm_Load;

        }


        // โหลดข้อมูลเมื่อฟอร์มแสดงผล
        private void CartForm_Load(object sender, EventArgs e)
        {
            // แสดงข้อมูลลูกค้า
            nameDisplayTextBox.Text = _session.CustomerName;
            tableDisplayTextBox.Text = _session.TableNumber;

            // แสดงรายการสินค้า
            DisplayCartItems();



        }
        private void DisplayCartItems()
        {
            // ล้างข้อมูลเดิม
            cartItemsFlowPanel.Controls.Clear();
            deleteAllButton.Visible = _session.Cart.Any();
            bool isCartEmpty = !_session.Cart.Any();

            // --- ควบคุมการมองเห็นของปุ่มต่างๆ ---
            deleteAllButton.Visible = !isCartEmpty;
            confirmOrderButton.Enabled = !isCartEmpty;
            
            if (!isCartEmpty)
            {
                // --- ส่วนแสดงรายการสินค้าหลัก ---
                foreach (var item in _session.Cart)
                {
                    // สร้าง Panel สำหรับ 1 แถว
                    Panel rowPanel = new Panel
                    {
                        Width = cartItemsFlowPanel.ClientSize.Width - 25,
                        Height = 80,
                        Margin = new Padding(0, 0, 0, 5),
                        BackColor = Color.White
                    };

                    // --- สร้าง Controls ภายในแถว ---
                    Label itemName = new Label { Text = item.Name, Font = new Font("MN ECLAIR", 12F), Location = new Point(20, 15), AutoSize = true };
                    Label itemPrice = new Label { Text = $"{item.Price:N2}", Font = new Font("MN ECLAIR", 10F), ForeColor = Color.Gray, Location = new Point(20, 45), AutoSize = true };

                    // สร้าง NumericUpDown สำหรับปรับจำนวนสินค้า**
                    MenuItem fullItemInfo = Connect_db.GetMenuItemById(item.Id);
                    int currentStock = (fullItemInfo != null) ? fullItemInfo.Stock : 0;
                    NumericUpDown quantity = new NumericUpDown
                    {
                        Value = Math.Min(item.Quantity, currentStock > 0 ? currentStock : 1), // ป้องกันไม่ให้ค่าเริ่มต้นเกินสต็อก
                        Minimum = 1,
                        Font = new Font("MN ECLAIR", 12F, FontStyle.Bold),
                        Width = 70,
                        Tag = item,
                        Location = new Point(320, 25)
                    };

                    // ตั้งค่า Maximum 
                    if (currentStock >= 10)
                    {
                        quantity.Maximum = 10;
                    }
                    else
                    {
                        quantity.Maximum = currentStock > 0 ? currentStock : 1; // ถ้าสต็อกเป็น 0 ให้ Maximum เป็น 1 
                    }

                    // ผูก Event 
                    quantity.ValueChanged += Quantity_ValueChanged;

                    //  สร้าง Label เพื่อแสดงจำนวนคงเหลือ ถ้าเหลือน้อย
                    if (currentStock < 10 && currentStock > 0)
                    {
                        Label stockLabel = new Label
                        {
                            Text = $"(เหลือ {currentStock})",
                            Font = new Font("MN ECLAIR Light", 9F),
                            ForeColor = Color.Red,
                            AutoSize = true,
                            Location = new Point(quantity.Left, quantity.Bottom + 2) // แสดงใต้ NumericUpDown
                        };
                        rowPanel.Controls.Add(stockLabel);
                    }
                    // ถ้าสินค้าหมด (Stock = 0)
                    if (currentStock <= 0)
                    {
                        quantity.Enabled = false; // ปิด NumericUpDown
                        Label outOfStockLabel = new Label
                        {
                            Text = "(สินค้าหมด)",
                            Font = new Font("MN ECLAIR Light", 9F),
                            ForeColor = Color.Red,
                            AutoSize = true,
                            Location = new Point(quantity.Left, quantity.Bottom + 2)
                        };
                        rowPanel.Controls.Add(outOfStockLabel);
                    }

                    quantity.ValueChanged += Quantity_ValueChanged;
                    // สร้าง Label สำหรับแสดงราคารวมย่อย
                    Label subTotal = new Label { Name = "subTotalLabel", Text = $"{(item.Price * item.Quantity):N2}", Font = new Font("MN ECLAIR", 12F, FontStyle.Bold), Location = new Point(450, 30), AutoSize = true, Tag = item };

                    Button deleteButton = new Button { Text = "ลบ", BackColor = Color.Firebrick, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(60, 40), Location = new Point(550, 20), Tag = item };
                    deleteButton.Click += DeleteButton_Click;

                    rowPanel.Controls.AddRange(new Control[] { itemName, itemPrice, quantity, subTotal, deleteButton });
                    cartItemsFlowPanel.Controls.Add(rowPanel);
                }

                // --- ส่วนแสดง Toppings ---
                if (_session.SelectedToppings.Values.Any(list => list.Any()))
                {
                    Panel toppingPanel = new Panel
                    {
                        Width = cartItemsFlowPanel.ClientSize.Width - 25,
                        AutoSize = true,
                        MinimumSize = new Size(0, 80),
                        Margin = new Padding(0, 10, 0, 5),
                        BackColor = Color.White,
                        Padding = new Padding(10)
                    };

                    Label toppingHeader = new Label { Text = "เพิ่มผัก ซอส", Font = new Font("MN ECLAIR", 12F, FontStyle.Bold), Dock = DockStyle.Top };
                    Label toppingList = new Label { Font = new Font("MN ECLAIR", 11F), Dock = DockStyle.Fill, AutoSize = true };

                    string toppingText = "";
                    foreach (var toppingListValue in _session.SelectedToppings.Values)
                    {
                        foreach (var topping in toppingListValue)
                        {
                            toppingText += $"+ {topping.Name}\n";
                        }
                    }
                    toppingList.Text = toppingText;

                    toppingPanel.Controls.Add(toppingList);
                    toppingPanel.Controls.Add(toppingHeader);
                    cartItemsFlowPanel.Controls.Add(toppingPanel);
                }
                else // ถ้าตะกร้าว่าง
                {
                    //  แสดงข้อความ
                    Label emptyLabel = new Label { Text = "ตะกร้าของคุณว่างเปล่า", Font = new Font("Tahoma", 16F), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
                    cartItemsFlowPanel.Controls.Add(emptyLabel);
                }

                UpdateTotal();
            }

        }

        private void Quantity_ValueChanged(object sender, EventArgs e)
        {
            
            NumericUpDown num = (NumericUpDown)sender;// ดึง CartItem จาก Tag
            CartItem item = (CartItem)num.Tag;// อัปเดตจำนวนในตะกร้า
            if (num.Value == 0) // ถ้าผู้ใช้ปรับจำนวนเป็น 0
            {
                // ทำเหมือนกดปุ่มลบ
                _session.Cart.Remove(item);
                DisplayCartItems();
            }
            else
            {
                item.Quantity = (int)num.Value;
                Panel rowPanel = (Panel)num.Parent;
                // อัปเดตราคารวมย่อย
                Label subTotalLabel = (Label)rowPanel.Controls.Find("subTotalLabel", false).First();
                
                subTotalLabel.Text = $"{(item.Price * item.Quantity):N2}";
                UpdateTotal();
            }
        }



        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // ดึง CartItem จาก Tag
            Button btn = (Button)sender;
            CartItem item = (CartItem)btn.Tag;
            _session.Cart.Remove(item);

            // ถ้าลบจนหมด ให้เคลียร์ Topping ด้วย
            if (!_session.Cart.Any())
            {
                _session.SelectedToppings["Vegetable"].Clear();
                _session.SelectedToppings["Sauce"].Clear();
                this.DialogResult = CartClearedResult;
                this.Close();
            }
            else
            {
                DisplayCartItems();

            }
        }

        private void deleteAllButton_Click(object sender, EventArgs e)
        {
            if (!_session.Cart.Any()) return;

            var confirmResult = MessageBox.Show(
                "คุณต้องการลบสินค้าทั้งหมดออกจากตะกร้าใช่หรือไม่?",
                "ยืนยันการลบทั้งหมด",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                _session.Cart.Clear(); // ล้างสินค้าในตะกร้า

                // เคลียร์ Topping ด้วย
                _session.SelectedToppings["Vegetable"].Clear();
                _session.SelectedToppings["Sauce"].Clear();
                
                this.DialogResult = CartClearedResult;
                this.Close();
            }
        }

        private void UpdateTotal()
        {
            totalLabel.Text = $"รวม {_session.GetCartTotal():N2} บาท";
            // ส่งสัญญาณกลับไปบอก Form1 ว่าตะกร้าอัปเดตแล้ว
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void confirmOrderButton_Click(object sender, EventArgs e)
        {
            // สร้าง object Customizations จากข้อมูล Topping ที่เลือกไว้ใน AppSession
            var customOptionsResult = new Customizations
            {
                SelectedVeggies = _session.SelectedToppings["Vegetable"],
                SelectedSauces = _session.SelectedToppings["Sauce"]
            };

            // เปิด PaymentForm โดยส่งพารามิเตอร์ไปให้ครบ
            using (var paymentForm = new PaymentForm(_session, customOptionsResult))
            {
                if (paymentForm.ShowDialog(this) == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK; // ส่งสัญญาณกลับไปให้หน้าหลักว่าสำเร็จ
                    this.Close();
                }
            }
        }

        private void goback_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

}


    

      