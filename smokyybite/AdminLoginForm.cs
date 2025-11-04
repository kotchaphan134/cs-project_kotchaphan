using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1
{
    public partial class AdminLoginForm : Form
    {
        public AdminLoginForm()
        {
            InitializeComponent();
            // ตั้งค่าให้กด Enter ใน TextBox แล้วทำงานเหมือนกดปุ่ม Login
            this.AcceptButton = loginButton;
        }

        private void AdminLoginForm_Load(object sender, EventArgs e)
        {

        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // รหัสผ่าน Admin (ในระบบจริงควรเก็บในฐานข้อมูลแบบเข้ารหัส)
            if (passwordBox.Text == "123456")
            {
                this.DialogResult = DialogResult.OK; // ส่งสัญญาณว่าล็อกอินสำเร็จ
                this.Close();
            }
            else
            {
                MessageBox.Show("รหัสผ่านไม่ถูกต้อง", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                passwordBox.Clear();
                passwordBox.Focus();
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
