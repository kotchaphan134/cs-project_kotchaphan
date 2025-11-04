// In Connect_db.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace smokyybite 
{


    public static class Connect_db
    {
        // เชื่อมต่อฐานข้อมูล MySQL
        private static string connectionString = "server=localhost;Port=3309;database=smokyybite;uid=root;pwd=;";
       
        //--- โครงสร้างข้อมูลที่ใช้ในโปรแกรม ---
        // แปลงรูปภาพเป็น byte array และกลับกัน
        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null) return null;
            using (var ms = new MemoryStream()) { image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); return ms.ToArray(); }
        }

        // แปลง byte array กลับเป็น Image
        public static Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;
            using (var ms = new MemoryStream(byteArray)) { return Image.FromStream(ms); }
        }


        // --- จัดการข้อมูลในตาราง stock  ---
        public static List<MenuItem> GetMenuItems()
        {
            var list = new List<MenuItem>();
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, name, description, amount, price, photo FROM stock ORDER BY name", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new MenuItem { Id = reader.GetInt32("id"), Name = reader.GetString("name"), Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"), Stock = reader.GetInt32("amount"), Price = reader.GetDecimal("price"), Photo = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : ByteArrayToImage((byte[])reader["photo"]) });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error loading menu items: " + ex.Message); }
            }
            return list;
        }

        // ดึงข้อมูลเมนูตาม ID
        public static MenuItem GetMenuItemById(int id)
        {
            MenuItem item = null;
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, name, description, amount, price, photo FROM stock WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // ใช้ if เพราะมีข้อมูลแค่แถวเดียว
                        {
                            item = new MenuItem
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                                Stock = reader.GetInt32("amount"),
                                Price = reader.GetDecimal("price"),
                                Photo = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : ByteArrayToImage((byte[])reader["photo"])
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading menu item by ID: " + ex.Message);
                }
            }
            return item;
        }
        // --- จัดการข้อมูลในตาราง topping ---
        public static Dictionary<string, List<Topping>> GetToppingsByType()
        {

            
            var toppings = new Dictionary<string, List<Topping>>
             {
        { "Vegetable", new List<Topping>() },
        { "Sauce", new List<Topping>() }
             };

            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, name, type, price, amount FROM toppings ORDER BY type, name", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var topping = new Topping
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Type = reader.GetString("type"),
                                Price = reader.GetDecimal("price"),
                                Amount = reader.GetInt32("amount")
                            };

                            // ตรวจสอบก่อนว่า Key (Type) มีอยู่ใน Dictionary หรือไม่
                            if (toppings.ContainsKey(topping.Type))
                            {
                                // ถ้ามี ก็เพิ่มข้อมูลเข้าไปใน List
                                toppings[topping.Type].Add(topping);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading toppings: " + ex.Message);
                }
            }

            // คืนค่า Dictionary ที่มีข้อมูล (หรือว่างเปล่า) กลับไปเสมอ
            return toppings;
        }

        // -------เพิ่ม, แก้ไข, ลบ เมนู----------
        public static bool AddMenuItem(MenuItem item)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO stock (name, description, amount, price, photo) VALUES (@name, @desc, @amount, @price, @photo)", conn);
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@desc", item.Description);
                    cmd.Parameters.AddWithValue("@amount", item.Stock);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@photo", ImageToByteArray(item.Photo));
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { MessageBox.Show("Error adding item: " + ex.Message); return false; }
            }
        }

        public static bool UpdateMenuItem(MenuItem item)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("UPDATE stock SET name=@name, description=@desc, amount=@amount, price=@price, photo=@photo WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@desc", item.Description);
                    cmd.Parameters.AddWithValue("@amount", item.Stock);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@photo", ImageToByteArray(item.Photo));
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { MessageBox.Show("Error updating item: " + ex.Message); return false; }
            }
        }

        public static bool DeleteMenuItem(int id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM stock WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { MessageBox.Show("Error deleting item: " + ex.Message); return false; }
            }
        }


        // เพิ่ม, แก้ไข, ลบ Topping
        public static List<Topping> GetToppings()
        {
            var list = new List<Topping>();
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, name, type, price, amount FROM toppings ORDER BY type, name", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Topping
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Type = reader.GetString("type"),
                                Price = reader.GetDecimal("price"),
                                Amount = reader.GetInt32("amount")
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error loading toppings: " + ex.Message); }
            }
            return list;
        }
        // เพิ่ม, แก้ไข, ลบ Topping
        public static bool AddTopping(Topping topping)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("INSERT INTO toppings (name, type, price, amount) VALUES (@name, @type, @price, @amount)", conn);
                    cmd.Parameters.AddWithValue("@name", topping.Name);
                    cmd.Parameters.AddWithValue("@type", topping.Type);
                    cmd.Parameters.AddWithValue("@price", 0); // บังคับราคาเป็น 0
                    cmd.Parameters.AddWithValue("@amount", topping.Amount);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { MessageBox.Show("Error adding topping: " + ex.Message); return false; }
            }
        }

        public static bool UpdateTopping(Topping topping)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("UPDATE toppings SET name=@name, type=@type, price=@price, amount=@amount WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@name", topping.Name);
                    cmd.Parameters.AddWithValue("@type", topping.Type);
                    cmd.Parameters.AddWithValue("@price", 0); // บังคับราคาเป็น 0
                    cmd.Parameters.AddWithValue("@id", topping.Id);
                    cmd.Parameters.AddWithValue("@amount", topping.Amount);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { MessageBox.Show("Error updating topping: " + ex.Message); return false; }
            }
        }

        public static bool DeleteTopping(int id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM toppings WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex) { MessageBox.Show("Error deleting topping: " + ex.Message); return false; }
            }
        }

        //---เตรียมอาหาร-------------
        // บันทึกคำสั่งซื้อใหม่
        public static bool SaveOrder(Order order)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                MySqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    
                    transaction = conn.BeginTransaction();

                    string orderId = $"ORD-{DateTime.Now.Ticks}";

                    // เตรียมข้อมูล Toppings จาก CustomOptions 
                    string toppingsText = "";

                    // เพิ่มข้อมูลซอส
                    // ตรวจสอบว่ามี CustomOptions และมีการเลือกซอส 
                    if (order.CustomOptions != null && order.CustomOptions.SelectedSauces.Any(s => s.Name != "ไม่ใส่ซอส"))
                    {
                        // นำชื่อซอสที่เลือกทั้งหมดมาต่อกัน
                        string sauces = string.Join(", ", order.CustomOptions.SelectedSauces.Select(s => s.Name));
                        toppingsText += $"ซอส: {sauces}";
                    }

                    // เพิ่มข้อมูลผัก (ถ้ามี)
                    if (order.CustomOptions != null && order.CustomOptions.SelectedVeggies.Any())
                    {
                        // ถ้ามีซอสอยู่แล้ว ให้ขึ้นบรรทัดใหม่
                        if (!string.IsNullOrEmpty(toppingsText))
                        {
                            toppingsText += Environment.NewLine; // ขึ้นบรรทัดใหม่
                        }
                        // นำชื่อผักที่เลือกทั้งหมดมาต่อกัน
                        string veggies = string.Join(", ", order.CustomOptions.SelectedVeggies.Select(v => v.Name));
                        toppingsText += $"ผัก: {veggies}";
                    }

                    // ถ้าไม่มี Toppings เลย ให้ใช้ DBNull.Value แทน string ว่าง
                    object toppingsValue = string.IsNullOrWhiteSpace(toppingsText) ? (object)DBNull.Value : toppingsText;


                    // วนลูปบันทึกรายการสินค้า
                    foreach (var item in order.Items)
                    {
                        // แก้ไขคำสั่ง SQL ให้มีคอลัมน์ toppings_selected
                        var historyCmd = new MySqlCommand(
                            "INSERT INTO history (order_date, order_time, name_customer, table_customer, phone, name_food, price, amount, `sum`, toppings_selected, photo) " +
                            "VALUES (@date, @time, @customer, @table, @phone, @food, @price, @amount, @sum, @toppings, @slip)",
                            conn, transaction);

                        historyCmd.Parameters.AddWithValue("@orderId", orderId);
                        historyCmd.Parameters.AddWithValue("@date", order.Date); // ส่ง DateTime object
                        historyCmd.Parameters.AddWithValue("@time", order.Date); // ส่ง DateTime object
                        historyCmd.Parameters.AddWithValue("@customer", order.CustomerName);
                        historyCmd.Parameters.AddWithValue("@table", order.TableNumber);
                        historyCmd.Parameters.AddWithValue("@phone", order.Phone ?? ""); // ใช้ Phone จาก Order
                        historyCmd.Parameters.AddWithValue("@food", item.Name);
                        historyCmd.Parameters.AddWithValue("@price", item.Price); // ใช้ราคาจาก CartItem
                        historyCmd.Parameters.AddWithValue("@amount", item.Quantity);
                        historyCmd.Parameters.AddWithValue("@sum", item.Price * item.Quantity); // คำนวณยอดรวมของรายการสินค้านี้
                        historyCmd.Parameters.AddWithValue("@slip", ImageToByteArray(order.PaymentSlip));

                        // เพิ่ม Parameter ใหม่สำหรับ toppings ที่เรารวบรวมไว้
                        historyCmd.Parameters.AddWithValue("@toppings", toppingsValue);

                        historyCmd.ExecuteNonQuery();

                        // ตัดสต็อก 
                        var stockCmd = new MySqlCommand("UPDATE stock SET amount = amount - @quantity WHERE id = @id", conn, transaction);
                        stockCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                        stockCmd.Parameters.AddWithValue("@id", item.Id);
                        stockCmd.ExecuteNonQuery();
                    }
                    // --- วนลูปตัดสต็อก Toppings (ตาราง toppings) ---
                    if (order.CustomOptions != null)
                    {
                        //1 รวบรวม Topping ที่ถูกเลือกทั้งหมด (ทั้งผักและซอส) มาไว้ใน List เดียวกัน
                        List<Topping> allSelectedToppings = new List<Topping>();
                        if (order.CustomOptions.SelectedVeggies != null)
                        {
                            allSelectedToppings.AddRange(order.CustomOptions.SelectedVeggies);
                        }
                        if (order.CustomOptions.SelectedSauces != null)
                        {
                            allSelectedToppings.AddRange(order.CustomOptions.SelectedSauces);
                        }

                        //2 วนลูปใน Topping ที่เลือกรวมทั้งหมดเพื่อตัดสต็อก
                        foreach (var topping in allSelectedToppings)
                        {
                            // สร้างคำสั่ง UPDATE สำหรับตาราง toppings
                            // โดยจะตัดสต็อกครั้งละ 1 หน่วยต่อ 1 ออเดอร์
                            // (ถ้าต้องการให้ตัดตามจำนวนสินค้าหลัก ให้เปลี่ยน @quantity เป็น order.Items.Sum(i => i.Quantity))
                            var toppingStockCmd = new MySqlCommand("UPDATE toppings SET amount = amount - @quantity WHERE id = @id", conn, transaction);
                            toppingStockCmd.Parameters.AddWithValue("@quantity", 1); // ตัดครั้งละ 1
                            toppingStockCmd.Parameters.AddWithValue("@id", topping.Id);
                            toppingStockCmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    MessageBox.Show("เกิดข้อผิดพลาดในการบันทึกคำสั่งซื้อ: " + ex.Message, "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        // อัปเดตสถานะคำสั่งซื้อ
        public static bool UpdateOrderStatus(DateTime orderDateTime, string customerName, string newStatus)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // อัปเดตทุกรายการที่มี วันที่, เวลา, และชื่อลูกค้าตรงกัน
                    string query = "UPDATE history SET status = @newStatus WHERE order_date = @date AND order_time = @time AND name_customer = @customer";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@newStatus", newStatus);
                        cmd.Parameters.AddWithValue("@date", orderDateTime.Date);
                        cmd.Parameters.AddWithValue("@time", orderDateTime.TimeOfDay);
                        cmd.Parameters.AddWithValue("@customer", customerName);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error updating order status: " + ex.Message); return false; }
            }
        }


        // ดึงข้อมูลประวัติการสั่งซื้อโดยกรองตามสถานะ
        public static List<RawHistoryItem> GetRawHistoryByStatus(string status)
        {
            var rawList = new List<RawHistoryItem>();
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // ดึงข้อมูลเฉพาะออเดอร์ของวันนี้ที่มีสถานะตรงกับที่ต้องการ
                    string query = @"
                SELECT id, order_date, order_time, name_customer, table_customer,phone, photo,
                       toppings_selected, name_food, amount, `sum`, status
                FROM history
                WHERE status = @status 
                ORDER BY order_date ASC, order_time ASC;"; // เรียงจากเก่าไปใหม่

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rawList.Add(new RawHistoryItem
                                {
                                    Id = reader.GetInt32("id"),
                                    OrderDateTime = reader.GetDateTime("order_date").Add(reader.GetTimeSpan("order_time")),

                                    CustomerName = reader.GetString("name_customer"),
                                    TableCustomer = reader.GetString("table_customer"),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                                    PaymentSlip = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : ByteArrayToImage((byte[])reader["photo"]),
                                    ToppingsSelected = reader.IsDBNull(reader.GetOrdinal("toppings_selected")) ? "" : reader.GetString("toppings_selected"),
                                    ItemName = reader.GetString("name_food"),
                                    ItemAmount = reader.GetInt32("amount"),
                                    ItemSum = reader.GetDecimal("sum"),
                                    Status = reader.GetString("status") // ดึงค่า Status
                                });
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error loading history by status: " + ex.Message); }
            }
            return rawList;
        }

        // --- คลาสสำหรับแสดงผลในตาราง 'ประวัติการสั่งซื้อ' แบบสรุปรวม ---
        // --- ดึงข้อมูลประวัติการสั่งซื้อ ---
        public class RawHistoryItem
        {
            public int Id { get; set; }
            public DateTime OrderDateTime { get; set; }
            public string CustomerName { get; set; }
            public string TableCustomer { get; set; }
            public string Phone { get; set; }
            public Image PaymentSlip { get; set; }
            public string ToppingsSelected { get; set; }
            public string ItemName { get; set; } // ชื่อสินค้า 1 รายการ
            public int ItemAmount { get; set; }  // จำนวนของสินค้านั้น
            public decimal ItemSum { get; set; }     // ยอดรวมของสินค้านั้น
            public string Status { get; set; }

        }

        // ดึงข้อมูลประวัติการสั่งซื้อแบบสรุปรวม
        public static List<OrderHistoryItem> GetGroupedOrderHistory(DateTime startDate, DateTime endDate)
        {
            var list = new List<OrderHistoryItem>();
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // คำสั่ง SQL สำหรับดึงข้อมูลแบบสรุปรวม
                    string query = @"
                           SELECT 
                    MIN(id) as id,
                    order_date, order_time, name_customer, table_customer, phone, photo, status, toppings_selected,
                    SUM(`sum`) as total_sum,
                    GROUP_CONCAT(CONCAT(name_food, ' x', amount) SEPARATOR '\r\n') as items_summary
                FROM history
                WHERE order_date BETWEEN @startDate AND @endDate
                GROUP BY order_date, order_time, name_customer, table_customer, phone, photo, status, toppings_selected
                ORDER BY order_date DESC, order_time DESC;";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new OrderHistoryItem
                                {
                                    Id = reader.GetInt32("id"),
                                    OrderDateTime = reader.GetDateTime("order_date").Add(reader.GetTimeSpan("order_time")),
                                    CustomerName = reader.GetString("name_customer"),
                                    TableCustomer = reader.IsDBNull(reader.GetOrdinal("table_customer")) ? "" : reader.GetString("table_customer"),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                                    ItemsSummary = reader.IsDBNull(reader.GetOrdinal("items_summary")) ? "" : reader.GetString("items_summary"),
                                    ToppingsSelected = reader.IsDBNull(reader.GetOrdinal("toppings_selected")) ? "" : reader.GetString("toppings_selected"),
                                    TotalSum = reader.GetDecimal("total_sum"),
                                    PaymentSlip = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : ByteArrayToImage((byte[])reader["photo"]),
                                    Status = reader.GetString("status") // ดึงค่า Status
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading grouped order history: " + ex.Message);
                }
            }
            return list;
        }
   
        // ดึงรูปภาพสลิปจากประวัติการสั่งซื้อ
        public static Image GetSlipImage(int historyId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT photo FROM history WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", historyId);

                    object result = cmd.ExecuteScalar(); // ดึงข้อมูลแค่คอลัมน์เดียว
                    if (result != null && result != DBNull.Value)
                    {
                        return ByteArrayToImage((byte[])result);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error loading slip image: " + ex.Message); }
            }
            return null;
        }

        // ------------------- คลาสสำหรับรายงานยอดขาย --------------------------------
        public static List<SalesReportItem> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            var reportList = new List<SalesReportItem>();
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // คำสั่ง SQL สำหรับสรุปยอดขาย:
                    // GROUP BY name_food เพื่อรวมยอดขายของสินค้าแต่ละชนิด
                    // SUM(amount) เพื่อรวมจำนวนที่ขายได้ทั้งหมด
                    // SUM(sum) เพื่อรวมยอดขายทั้งหมด
                    // ORDER BY TotalSales DESC เพื่อจัดอันดับสินค้าขายดี
                    string query = @"
                SELECT 
                    name_food,
                    price, -- ราคาต่อหน่วย 
                    SUM(amount) AS TotalQuantity,
                    SUM(sum) AS TotalSales
                FROM history
                WHERE order_date BETWEEN @startDate AND @endDate
                GROUP BY name_food, price
                ORDER BY TotalQuantity DESC;";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                    
                        cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

                        using (var reader = cmd.ExecuteReader())
                        {
                            int rank = 1; // ตัวนับสำหรับจัดอันดับ
                            while (reader.Read())
                            {
                                reportList.Add(new SalesReportItem
                                {
                                    Rank = rank++,
                                    ItemName = reader.GetString("name_food"),
                                    PricePerUnit = reader.GetDecimal("price"),
                                    TotalQuantitySold = reader.GetInt32("TotalQuantity"),
                                    TotalSales = reader.GetDecimal("TotalSales")
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating sales report: " + ex.Message);
                }
            }
            return reportList;
        }






















        public static List<RawHistoryItem> GetRawOrderHistory(DateTime startDate, DateTime endDate)
        {
            var rawList = new List<RawHistoryItem>();
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    id, order_date, order_time, name_customer,table_customer, phone, photo,
                    toppings_selected, name_food, amount, `sum`
                FROM history
                WHERE order_date BETWEEN @startDate AND @endDate
                ORDER BY id DESC, order_date DESC, order_time DESC;";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rawList.Add(new RawHistoryItem
                                {
                                    Id = reader.GetInt32("id"),
                                    OrderDateTime = reader.GetDateTime("order_date").Add(reader.GetTimeSpan("order_time")),
                                    CustomerName = reader.GetString("name_customer"),
                                    TableCustomer = reader.GetString("table_customer"),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                                    PaymentSlip = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : ByteArrayToImage((byte[])reader["photo"]),
                                    ToppingsSelected = reader.IsDBNull(reader.GetOrdinal("toppings_selected")) ? "" : reader.GetString("toppings_selected"),
                                    ItemName = reader.GetString("name_food"),
                                    ItemAmount = reader.GetInt32("amount"),
                                    ItemSum = reader.GetDecimal("sum")
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading raw order history: " + ex.Message);
                }
            }
            return rawList;

        }
        public static List<OrderHistoryItem> GetAllOrderHistory()
        {
            var list = new List<OrderHistoryItem>();
            using (var conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // คำสั่ง SQL ที่ไม่มี WHERE clause สำหรับกรองวันที่
                    string query = @"
                SELECT 
                    MIN(id) as id, order_date, order_time, name_customer, 
                    table_customer, phone, photo, SUM(`sum`) as total_sum,
                    GROUP_CONCAT(CONCAT(name_food, ' (', amount, ')') SEPARATOR ', ') as items_summary
                   

                FROM history
                GROUP BY order_date, order_time, name_customer, table_customer, phone, photo
                ORDER BY order_date DESC, order_time DESC;";

                    var cmd = new MySqlCommand(query, conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime orderDate = reader.GetDateTime("order_date");
                            TimeSpan orderTime = reader.GetTimeSpan("order_time");
                            list.Add(new OrderHistoryItem
                            {
                                Id = reader.GetInt32("id"),
                                OrderDateTime = orderDate.Add(orderTime),
                                CustomerName = reader.GetString("name_customer"),
                                TableCustomer = reader.GetString("table_customer"),
                                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                                ItemsSummary = reader.GetString("items_summary"),
                                TotalSum = reader.GetDecimal("total_sum"),
                                PaymentSlip = reader.IsDBNull(reader.GetOrdinal("photo")) ? null : ByteArrayToImage((byte[])reader["photo"])
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error loading all order history: " + ex.Message); }
            }
            return list;
        }

    }
}
