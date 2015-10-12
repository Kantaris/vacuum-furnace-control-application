using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FurnaceController
{
    public partial class Users : Form
    {
        UserDetails user;
        List<UserDetails> userList = new List<UserDetails>();
        Login uu;
        public Users()
        {
            InitializeComponent();

            string[] uspwd = Properties.Settings.Default.users.Split(';');
            uu = new Login();
            if (uu.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                user = uu.getUser();
                for (int i = 0; i < uspwd.Length; i++)
                {
                    if (uspwd[i].Length > 0)
                    {
                        UserDetails ud = new UserDetails();
                        string[] ss = uspwd[i].Split(':');
                        ud.username = ss[0];
                        ud.password = ss[1];
                        userList.Add(ud);
                        listView1.Items.Add(ud.username);
                    }
                }
            }
            else
            {
                Close();
            }
        }

        UserAdd ua;
        private void addButton_Click(object sender, EventArgs e)
        {
            ua = new UserAdd();
            if (uu.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UserDetails ud = uu.getUser();
                userList.Add(ud);
                listView1.Items.Add(ud.username);
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedIndices.Count > 0 && 
                !(userList[listView1.SelectedIndices[0]].username.Equals(user.username) && 
                userList[listView1.SelectedIndices[0]].password.Equals(user.password))){

                listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
                userList.RemoveAt(listView1.SelectedIndices[0]);
            }
        }

      
        private void saveButton_Click(object sender, EventArgs e)
        {
            string ss = "";
            for (int i = 0; i < userList.Count; i++)
            {
                ss += userList[i].username + ":" + userList[i].password + ";";
            }
            Properties.Settings.Default.users = ss;
            Properties.Settings.Default.Save();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

   
}
