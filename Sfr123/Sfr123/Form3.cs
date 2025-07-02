using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sfr123
{
    public partial class Form3 : Form
    {
        private string connectionString = "Data Source=DESKTOP-87GLJFV;Initial Catalog=СФР1;Integrated Security=True";
        public Form3()
        {
            InitializeComponent();
        }

        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox2.Text;
            string password = textBox3.Text;

            if (AuthenticateUser(login, password))
            {
                MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Уст тек польз (7ф)
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SELECT Id_польз FROM Кабинет WHERE Логин = @login", connection);
                    cmd.Parameters.AddWithValue("@login", login);
                    var userId = (int)cmd.ExecuteScalar();
                    CurrentUser.Id = userId;
                    CurrentUser.Login = login;
                }

                
                //на инф
                Form5 f5 = new Form5();
                f5.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AuthenticateUser(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Кабинет WHERE Логин = @login AND Пароль = @password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count == 1; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                    return false;
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
            this.Hide();
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        




    }
    
}
