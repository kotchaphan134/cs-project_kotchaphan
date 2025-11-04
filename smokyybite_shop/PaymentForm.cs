using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace smokyybite
{
    public partial class PaymentForm : Form
    {
        
        private AppSession _session;
        private Customizations _customOptions;
        private Image uploadedSlip;
        private Order finalOrder;
        public PaymentForm(AppSession session, Customizations customOptions)
        {
            InitializeComponent();
            // เก็บข้อมูล session และ customOptions ที่ส่งมา
            _session = session;
            _customOptions = customOptions;
        }
        private void PaymentForm_Load(object sender, EventArgs e)
        {
            DisplayOrderItems();
          
        }
        //--- ฟังก์ชันแสดงรายการสินค้าในตะกร้าและ Topping ---//
        private void DisplayOrderItems()
        {
            // เคลียร์รายการเก่าทิ้ง
            orderItemsPanel.Controls.Clear();

            // คำนวณราคารวมทั้งหมด
            decimal cartTotal = _session.GetCartTotal();
            decimal toppingTotal = _customOptions.GetTotalPrice();
            decimal finalTotal = cartTotal + toppingTotal;

            // ตั้งค่าตัวแปรสำหรับวาด UI
            int yPos = 10; // ตำแหน่งเริ่มต้นในแนวตั้ง
            int leftMargin = 10;
            int rightMargin = orderItemsPanel.ClientSize.Width - 10;
            Font itemFont = new Font("MN ECLAIR", 14F);
            Font toppingFont = new Font("MN ECLAIR Light", 12F);
            Font toppingHeaderFont = new Font("MN ECLAIR", 14F, FontStyle.Bold);

            // วนลูปแสดงรายการสินค้าหลัก
            foreach (var item in _session.Cart)
            {
                // Label ชื่อ (ชิดซ้าย)
                Label itemNameLabel = new Label
                {
                    Text = $"{item.Name} x{item.Quantity}",
                    Font = itemFont,
                    Location = new Point(leftMargin, yPos),
                    AutoSize = true
                };

                // Label ราคา (ชิดขวา)
                Label itemPriceLabel = new Label
                {
                    Text = $"{(item.Price * item.Quantity):N2}",
                    Font = itemFont,
                    AutoSize = true
                };
                itemPriceLabel.Location = new Point(rightMargin - itemPriceLabel.Width, yPos);
                itemPriceLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                orderItemsPanel.Controls.Add(itemNameLabel);
                orderItemsPanel.Controls.Add(itemPriceLabel);

                // เลื่อนตำแหน่ง yPos ลงสำหรับรายการถัดไป
                yPos += Math.Max(itemNameLabel.Height, itemPriceLabel.Height) + 5;
            }

            // วนลูปแสดงรายการ Topping 
            if (_customOptions != null && (_customOptions.SelectedVeggies.Any() || _customOptions.SelectedSauces.Any(s => s.Name != "ไม่ใส่ซอส")))
            {
                yPos += 10; // เพิ่มระยะห่างก่อนเริ่มกลุ่ม Topping

                // หัวข้อ "เพิ่มผักและซอส"
                Label headerLabel = new Label { Text = "เพิ่มผักและซอส", Font = toppingHeaderFont, Location = new Point(leftMargin, yPos), AutoSize = true };
                orderItemsPanel.Controls.Add(headerLabel);
                yPos += headerLabel.Height + 5;

                // แสดงผัก
                foreach (var veg in _customOptions.SelectedVeggies)
                {
                    Label vegNameLabel = new Label { Text = $"  + {veg.Name}", Font = toppingFont, Location = new Point(leftMargin, yPos), AutoSize = true };
                    Label vegPriceLabel = new Label { Text = $"({veg.Price:N2})", Font = toppingFont, AutoSize = true };
                    vegPriceLabel.Location = new Point(rightMargin - vegPriceLabel.Width, yPos);
                    vegPriceLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                    orderItemsPanel.Controls.Add(vegNameLabel);
                    orderItemsPanel.Controls.Add(vegPriceLabel);
                    yPos += Math.Max(vegNameLabel.Height, vegPriceLabel.Height) + 2;
                }
                // แสดงซอส
                foreach (var sauce in _customOptions.SelectedSauces)
                {
                    if (sauce.Price > 0 || sauce.Name != "ไม่ใส่ซอส")
                    {
                        Label sauceNameLabel = new Label { Text = $"  + {sauce.Name}", Font = toppingFont, Location = new Point(leftMargin, yPos), AutoSize = true };
                        Label saucePriceLabel = new Label { Text = $"({sauce.Price:N2})", Font = toppingFont, AutoSize = true };
                        saucePriceLabel.Location = new Point(rightMargin - saucePriceLabel.Width, yPos);
                        saucePriceLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                        orderItemsPanel.Controls.Add(sauceNameLabel);
                        orderItemsPanel.Controls.Add(saucePriceLabel);
                        yPos += Math.Max(sauceNameLabel.Height, saucePriceLabel.Height) + 2;
                    }
                }
            }

            // อัปเดตยอดรวมใน Label ที่อยู่ด้านล่างสุด
            totalLabel.Text = $"รวม   {finalTotal:N2} บาท";
        }
        //--- ฟังก์ชันตรวจสอบการกรอกเบอร์โทรศัพท์ ---//
        // อนุญาตให้กรอกได้เฉพาะตัวเลขและปุ่ม Backspace
        // ปุ่มเลือกไฟล์สลิป
        private void browseButton_Click(object sender, EventArgs e)
        {
            // เปิดหน้าต่างเลือกไฟล์
            using (var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.png", Title = "เลือกสลิป" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    uploadedSlip = Image.FromFile(ofd.FileName);
                    slipPictureBox.Image = uploadedSlip;
                }
            }
        }
        // ปุ่มยืนยันการสั่งซื้อ
        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (uploadedSlip == null) { MessageBox.Show("กรุณาอัปโหลดสลิป"); return; }
            if (string.IsNullOrWhiteSpace(phoneTextBox.Text)) { MessageBox.Show("กรุณากรอกเบอร์โทรศัพท์"); return; }
            // ส่งข้อมูลคำสั่งซื้อไปยังฐานข้อมูล
            finalOrder = new Order
            {
                Date = DateTime.Now,
                CustomerName = _session.CustomerName,
                TableNumber = _session.TableNumber,
                Phone = phoneTextBox.Text,
                Items = _session.Cart,
                Total = _session.GetCartTotal() + _customOptions.GetTotalPrice(),
                PaymentSlip = uploadedSlip,
                CustomOptions = _customOptions
            };
            bool success = Connect_db.SaveOrder(finalOrder);

            if (success)
            {
                MessageBox.Show("บันทึกคำสั่งซื้อเรียบร้อย!", "สำเร็จ");
                PrintReceipt();
                this.DialogResult = DialogResult.OK;
            }
        }
        // ปุ่มยกเลิก
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ตรวจสอบการกดปุ่มใน TextBox ให้กรอกได้เฉพาะตัวเลข
        private void phoneTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // อนุญาตให้กดปุ่ม Backspace (char 8) ได้เสมอ
            if (e.KeyChar == (char)Keys.Back)
            {
                return;
            }

            // ตรวจสอบว่าปุ่มที่กด ไม่ใช่ตัวเลข
            // Char.IsDigit() จะคืนค่า true ถ้าเป็นเลข 0-9
            if (!char.IsDigit(e.KeyChar))
            {
                // ถ้าไม่ใช่ตัวเลข ให้ยกเลิกการกดปุ่มนั้น
                // โดยตั้งค่า e.Handled = true
                e.Handled = true;
            }
        }
        // ตรวจสอบความยาวเบอร์โทรศัพท์
        private void phoneTextBox_Validating(object sender, CancelEventArgs e)
        {
            // ดึง TextBox ที่กำลังถูกตรวจสอบ
            TextBox currentTextBox = (TextBox)sender;

            // ไม่ต้องตรวจสอบถ้ายังไม่มีการกรอกข้อมูล
            if (string.IsNullOrWhiteSpace(currentTextBox.Text))
            {
                return;
            }

            // ตรวจสอบว่าความยาวไม่เท่ากับ 10
            if (currentTextBox.Text.Length != 10)
            {
                //  แสดง MessageBox เตือนผู้ใช้
                MessageBox.Show(
                    "หมายเลขโทรศัพท์ต้องมี 10 หลักเท่านั้น",
                    "ข้อมูลไม่ถูกต้อง",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

               
                //    จะบังคับให้ผู้ใช้ต้องแก้ไขข้อมูลให้ถูกต้องก่อนจึงจะไปที่อื่นได้
                e.Cancel = true;
            }
        }

        // ฟังก์ชันพิมพ์ใบเสร็จ
        private void PrintReceipt()
        {
            // สร้าง PrintDocument
            PrintDocument pd = new PrintDocument();



            // กำหนดขนาดกระดาษ 

            PaperSize pkCustomSize = new PaperSize("Receipt", 250, 1100);

            // สร้าง PageSettings และ PrinterSettings ขึ้นมาใหม่
            PageSettings pgSettings = new PageSettings();
            pgSettings.PaperSize = pkCustomSize;
            pgSettings.Margins = new Margins(0, 0, 0, 0); // ไม่มีขอบ

            PrinterSettings ps = new PrinterSettings();

            pd.DefaultPageSettings = pgSettings;
            pd.PrinterSettings = ps;

            //ผูก Event PrintPage
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

            // แสดงหน้าต่าง Print Preview
            PrintPreviewDialog printPreview = new PrintPreviewDialog();
            printPreview.Document = pd;

            // ตั้งค่าให้หน้าต่าง Preview แสดงผลแบบหน้าเดียวและซูมพอดี
            printPreview.UseAntiAlias = true;
            ((Form)printPreview).WindowState = FormWindowState.Maximized;

            printPreview.ShowDialog(this);
        }

        // ฟังก์ชันสำหรับวาดใบเสร็จ
        private void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            // ตั้งค่า Font ต่างๆ
            Font titleFont = new Font("TH SarabunPSK", 24, FontStyle.Bold);
            Font headerFont = new Font("TH SarabunPSK", 16, FontStyle.Bold);
            Font bodyFont = new Font("TH SarabunPSK", 14);
            Font smallBodyFont = new Font("TH SarabunPSK", 12);
            float yPos = 0;
            float leftMargin = 20;
            float topMargin = 20;
            float lineHeight = 0;

            // --- วาด Header ---
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
            e.Graphics.DrawString("โต๊ะ", headerFont, Brushes.Black, new RectangleF(leftMargin, topMargin, 200, 30), centerFormat);
            yPos = topMargin + headerFont.GetHeight(e.Graphics);
            e.Graphics.DrawString(finalOrder.TableNumber.Replace("โต๊ะ ", ""), titleFont, Brushes.Black, new RectangleF(leftMargin, yPos, 200, 40), centerFormat);
            yPos += titleFont.GetHeight(e.Graphics);

            e.Graphics.DrawString("ร้าน SmokyBite", headerFont, Brushes.Black, new RectangleF(leftMargin, yPos, 200, 30), centerFormat);
            yPos += headerFont.GetHeight(e.Graphics);
            e.Graphics.DrawString("123/456 ABC\nเบอร์โทร: 0888888888", smallBodyFont, Brushes.Black, new RectangleF(leftMargin, yPos, 200, 50), centerFormat);
            yPos += 50;

            // --- รายการสินค้า ---
            e.Graphics.DrawString("พิมพ์ใบกำกับอย่างย่อ\nใบรับเงิน", smallBodyFont, Brushes.Black, new RectangleF(leftMargin, yPos, 200, 50), centerFormat);
            yPos += 50;

            e.Graphics.DrawLine(Pens.Black, leftMargin, yPos, 220, yPos);
            yPos += 5;

            foreach (var item in finalOrder.Items)
            {
                e.Graphics.DrawString($"{item.Quantity} x {item.Name}", bodyFont, Brushes.Black, leftMargin, yPos);
                e.Graphics.DrawString($"{item.Price * item.Quantity:N2}", bodyFont, Brushes.Black, 220, yPos, new StringFormat { Alignment = StringAlignment.Far });
                yPos += bodyFont.GetHeight(e.Graphics);
            }
            // topping
            if (finalOrder.CustomOptions != null && (finalOrder.CustomOptions.SelectedVeggies.Any() || finalOrder.CustomOptions.SelectedSauces.Any()))
            {
                e.Graphics.DrawString("เพิ่มผักและซอส", smallBodyFont, Brushes.Black, leftMargin, yPos);
                yPos += smallBodyFont.GetHeight(e.Graphics);

                foreach (var veg in finalOrder.CustomOptions.SelectedVeggies)
                {
                    e.Graphics.DrawString($" + {veg.Name}", smallBodyFont, Brushes.Black, leftMargin + 10, yPos);
                    e.Graphics.DrawString($"{veg.Price:N2}", smallBodyFont, Brushes.Black, 220, yPos, new StringFormat { Alignment = StringAlignment.Far });
                    yPos += smallBodyFont.GetHeight(e.Graphics);
                }
                foreach (var sauce in finalOrder.CustomOptions.SelectedSauces)
                {
                    if (sauce.Price > 0 || sauce.Name != "ไม่ใส่ซอส")
                    {
                        e.Graphics.DrawString($" + {sauce.Name}", smallBodyFont, Brushes.Black, leftMargin + 10, yPos);
                        e.Graphics.DrawString($"{sauce.Price:N2}", smallBodyFont, Brushes.Black, 220, yPos, new StringFormat { Alignment = StringAlignment.Far });
                        yPos += smallBodyFont.GetHeight(e.Graphics);
                    }
                }
            }

            // --- ยอดรวม ---
            e.Graphics.DrawLine(Pens.Black, leftMargin, yPos, 220, yPos);
            yPos += 5;

            e.Graphics.DrawString("ยอดรวม", headerFont, Brushes.Black, leftMargin, yPos);
            e.Graphics.DrawString($"{finalOrder.Total:N2}", headerFont, Brushes.Black, 220, yPos, new StringFormat { Alignment = StringAlignment.Far });
            yPos += headerFont.GetHeight(e.Graphics) + 20;

            // --- Footer ---
            e.Graphics.DrawString($"วันที่ {finalOrder.Date:dd-MM-yyyy HH:mm:ss}", smallBodyFont, Brushes.Black, new RectangleF(leftMargin, yPos, 200, 30), centerFormat);
            yPos += smallBodyFont.GetHeight(e.Graphics);
            e.Graphics.DrawString("ขอบคุณที่ใช้บริการค่ะ", bodyFont, Brushes.Black, new RectangleF(leftMargin, yPos, 200, 30), centerFormat);

        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

       
        
    }
}
