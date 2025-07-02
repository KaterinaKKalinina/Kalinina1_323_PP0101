using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sfr123
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int drawingScore = 0; //рис
            int singingScore = 0; //песн
            int dancingScore = 0; //танцы
            int craftingScore = 0; //рук

            // Вопрос 1
            if (radioButton1a.Checked) drawingScore++;
            else if (radioButton1b.Checked) singingScore++;
            else if (radioButton1c.Checked) dancingScore++;
            else if (radioButton1d.Checked) craftingScore++;

            // 2
            if (radioButton2a.Checked) drawingScore++;
            else if (radioButton2b.Checked) singingScore++;
            else if (radioButton2c.Checked) dancingScore++;
            else if (radioButton2d.Checked) craftingScore++;

            // 3
            if (radioButton3a.Checked) drawingScore++;
            else if (radioButton3b.Checked) singingScore++;
            else if (radioButton3c.Checked) dancingScore++;
            else if (radioButton3d.Checked) craftingScore++;

            // 4
            if (radioButton4a.Checked) drawingScore++;
            else if (radioButton4b.Checked) singingScore++;
            else if (radioButton4c.Checked) dancingScore++;
            else if (radioButton4d.Checked) craftingScore++;

            // 5
            if (radioButton5a.Checked) drawingScore++;
            else if (radioButton5b.Checked) singingScore++;
            else if (radioButton5c.Checked) dancingScore++;
            else if (radioButton5d.Checked) craftingScore++;

            // 6
            if (radioButton6a.Checked) drawingScore++;
            else if (radioButton6b.Checked) singingScore++;
            else if (radioButton6c.Checked) dancingScore++;
            else if (radioButton6d.Checked) craftingScore++;

            // 7
            if (radioButton7a.Checked) drawingScore++;
            else if (radioButton7b.Checked) singingScore++;
            else if (radioButton7c.Checked) dancingScore++;
            else if (radioButton7d.Checked) craftingScore++;


            // Резт
            string result = "Ваше хобби: ";

            if (drawingScore > singingScore && drawingScore > dancingScore && drawingScore > craftingScore)
                result += "Рисование!";
            else if (singingScore > drawingScore && singingScore > dancingScore && singingScore > craftingScore)
                result += "Пение!";
            else if (dancingScore > drawingScore && dancingScore > singingScore && dancingScore > craftingScore)
                result += "Танцы!";
            else if (craftingScore > drawingScore && craftingScore > singingScore && craftingScore > dancingScore)
                result += "Рукоделие!";
            else
                result += "У вас несколько интересов!";

            
            label9.Text = result;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5(); //обр
            f5.Show();
            this.Hide();
        }
    }
}
