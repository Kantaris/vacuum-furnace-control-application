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
    public partial class Login : Form
    {
        UserDetails uu;
        List<UserDetails> userList = new List<UserDetails>();

        public Login()
        {
            InitializeComponent();
            string[] uspwd = Properties.Settings.Default.users.Split(';');
            for (int i = 0; i < uspwd.Length; i++)
            {
                if(uspwd[i].Length > 0)
                {
                    UserDetails ud = new UserDetails();
                    string[] ss = uspwd[i].Split(':');
                    ud.username = ss[0];
                    ud.password = ss[1];
                    userList.Add(ud);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool found = false;
            for (int i = 0; i < userList.Count; i++)
            {
                if(textBox1.Text.Equals(userList[i].username))
                {
                    string pwd5 = "";
                    var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(maskedTextBox1.Text.ToCharArray()));
                    foreach (byte h in hash)
                    {
                        pwd5 += h.ToString("x2");
                    }
                    if (pwd5.Equals(userList[i].password))
                    {
                        uu = userList[i];
                        found = true;
                    }
                }
            }
            if (found)
            {
                this.DialogResult = DialogResult.OK;    
            }
            else
            {
                toolStripStatusLabel1.Text = "Username or password is wrong";
            }
        }
        public UserDetails getUser()
        {
            return uu;
        }
    }
    public class UserDetails
    {
        public string username;
        public string password;
        public UserDetails() { }
    }
}
