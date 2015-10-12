using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace FurnaceController
{
    public partial class UserAdd : Form
    {
        UserDetails ud = new UserDetails();

        public UserAdd()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            ud.username = textBox1.Text;

            string pwd5 = "";
            var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(maskedTextBox1.Text.ToCharArray()));
            foreach (byte h in hash)
            {
                pwd5 += h.ToString("x2");
            }

            ud.password = pwd5;
        }

        public UserDetails getUser()
        {
            return ud;
        }
    }
}
