using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sfr123
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();

            //эт общ инфа
            ShowPanel(panel1);
        }

        private void label42_Click(object sender, EventArgs e)
        {
            ShowPanel(panel1);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Ссылка вк
            string vkLink = "https://vk.com/sfr.saratovskayaoblast";

            // Откр сс в бр
            Process.Start(new ProcessStartInfo
            {
                FileName = vkLink,
                UseShellExecute = true 
            });
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Ссылка ок
            string okLink = "https://ok.ru/sfr.saratovskayaoblast";

            
            Process.Start(new ProcessStartInfo
            {
                FileName = okLink,
                UseShellExecute = true 
            });
        
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Сылка тг
            string telegramLink = "https://t.me/sfr_saratov";

            
            Process.Start(new ProcessStartInfo
            {
                FileName = telegramLink,
                UseShellExecute = true 
            });
        
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            ShowPanel(panel2);
        }

        private void ShowPanel(Panel panelToShow)
        {
            // Скр панельки
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            

            // Показ опр панель
            panelToShow.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ShowPanel(panel3); //сеть плести
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
            this.Hide();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form6 f6 = new Form6();
            f6.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 f7 = new Form7();
            f7.Show();
        }
    }
}
