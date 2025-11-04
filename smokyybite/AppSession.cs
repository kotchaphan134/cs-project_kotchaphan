// In AppSession.cs
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace smokyybite
{
   
    
    public class AppSession
    {
        // ตะกร้าสินค้า
        public List<CartItem> Cart { get; set; }
        public string CustomerName { get; set; }
        public string TableNumber { get; set; }
        // รายการ Toppings ที่เลือกสำหรับแต่ละประเภท
        public Dictionary<string, List<Topping>> SelectedToppings { get; set; }

        public List<Topping> GlobalSelectedToppings { get; set; } = new List<Topping>();
        // ตัวเลือกการปรับแต่ง (Toppings)
        public Customizations CustomOptions { get; set; } = new Customizations();
        // สถานะการตัดสินใจเลือก Toppings สำหรับแต่ละประเภท
        public Dictionary<string, bool> ToppingDecisionMade { get; set; }
        // ตัวสร้าง

        public AppSession()
        {
            // เริ่มต้น Session ด้วยตะกร้าที่ว่างเปล่า
            Cart = new List<CartItem>();
            SelectedToppings = new Dictionary<string, List<Topping>>
            {
                { "Vegetable", new List<Topping>() },
                { "Sauce", new List<Topping>() }
            };
            ToppingDecisionMade = new Dictionary<string, bool>
        {
            { "Vegetable", false }, // เริ่มต้นว่ายังไม่ตัดสินใจ
            { "Sauce", false }      // เริ่มต้นว่ายังไม่ตัดสินใจ
        };
        
        }


        // เพิ่มสินค้าไปยังตะกร้า
        public void AddToCart(MenuItem item, int quantity)
        {
            var existing = Cart.FirstOrDefault(c => c.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                Cart.Add(new CartItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Photo = item.Photo,
                    Quantity = quantity
                });
            }
        }



        public decimal GetCartTotal()
        {
            decimal itemsTotal = Cart.Sum(item => item.Price * item.Quantity);
            decimal toppingsTotal = CustomOptions.GetTotalPrice();
            return itemsTotal + toppingsTotal;
        }
        public int GetTotalQuantityOfItemById(int itemId) => Cart.Where(ci => ci.Id == itemId).Sum(ci => ci.Quantity);



        // เคลียร์ข้อมูล Session ทั้งหมด
        public void Clear()
        {
            Cart.Clear();
            CustomOptions = new Customizations(); // เคลียร์ Topping ด้วย
            CustomerName = null;
            TableNumber = null;
            SelectedToppings["Vegetable"].Clear();
            SelectedToppings["Sauce"].Clear();
            ToppingDecisionMade["Vegetable"] = false;
            ToppingDecisionMade["Sauce"] = false;
        }
        public void ClearCart()
        {
            Cart.Clear();
        }
    }
}