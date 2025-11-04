using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public Image Photo { get; set; }
    public string Status { get; set; }
}

// --- คลาสสำหรับ 'ตะกร้าสินค้า'  ---
public class CartItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal BasePrice { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Image Photo { get; set; }
    public List<Topping> SelectedToppings { get; set; } = new List<Topping>();
    public decimal TotalPrice
    {
        get
        {
            decimal toppingsPrice = SelectedToppings.Sum(t => t.Price);
            return BasePrice + toppingsPrice;
        }
    }
}
// --- คลาสสำหรับ 'การปรับแต่ง' (Toppings) ---

public class Customizations
{

    public List<Topping> SelectedVeggies { get; set; } = new List<Topping>();
    public List<Topping> SelectedSauces { get; set; } = new List<Topping>();

    // คำนวณราคารวมของ Topping ทั้งหมด
    public decimal GetTotalPrice()
    {
        decimal veggiePrice = SelectedVeggies.Sum(t => t.Price);
        decimal saucePrice = SelectedSauces.Sum(t => t.Price);
        return veggiePrice + saucePrice;
    }
}

// --- คลาสสำหรับบันทึก 'Order' ทั้งหมด ---
public class Order
{
    public string Id { get; set; }
    public System.DateTime Date { get; set; }
    public string CustomerName { get; set; }
    public string TableNumber { get; set; }
    public string Phone { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal Total { get; set; }
    public Image PaymentSlip { get; set; }
    public Customizations CustomOptions { get; set; }
}

// --- คลาสสำหรับแสดงผลในตาราง 'ประวัติการสั่งซื้อ' ---
public class OrderHistoryItem
{
    public int Id { get; set; }
    public System.DateTime OrderDateTime { get; set; }
    public string CustomerName { get; set; }
    public string TableCustomer { get; set; }
    public string ItemsSummary { get; set; }
   
    public List<string> ItemsList { get; set; } = new List<string>();

    public string ToppingsSelected { get; set; }
    public decimal TotalSum { get; set; }
    public Image PaymentSlip { get; set; }
    public string Phone { get; set; }
    public string FormattedItems
    {
        get
        {
            return string.Join(Environment.NewLine, ItemsList);
        }
    }
    public string Status { get; set; }

}
// --- คลาสสำหรับตาราง 'stock' ---
public class SalesReportItem
{
    public int Rank { get; set; }
    public string ItemName { get; set; }
    public decimal PricePerUnit { get; set; }
    public int TotalQuantitySold { get; set; }

    public decimal TotalSales { get; set; }
    public decimal TotalRevenue { get; internal set; }
}
// --- คลาสสำหรับ 'Topping' ---
public class Topping
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public decimal Price { get; set; }
    public int Amount { get; set; }
}
