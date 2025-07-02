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
    public partial class Form7 : Form
    {
        private string connectionString = "Data Source=DESKTOP-87GLJFV;Initial Catalog=СФР1;Integrated Security=True";

        
        // Спис дат
        private List<DateTime> eventDates = new List<DateTime>();

        // хран тек меропр
        private int? selectedEventId = null;
        private DateTime selectedDate;

        public Form7()
        {
            InitializeComponent();
            // Инициализация
            LoadEventDates();

            //// Обработчики
            monthCalendar1.DateSelected += monthCalendar1_DateChanged;
            button1.Click += button1_Click;

            // жирный
            MarkEventDates();

            


        }


        // Загруз дат
        private void LoadEventDates()
        {
            eventDates.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT DISTINCT CAST(Дата AS DATE) FROM Мероприятия", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        eventDates.Add(reader.GetDateTime(0));
                    }
                }
            }
        }

        // жирн нужн даты
        private void MarkEventDates()
        {
            monthCalendar1.BoldedDates = eventDates.ToArray();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            selectedDate = e.Start.Date;
            ShowEventsForDate(selectedDate);
        }

        // Показыв меропр на опр дату
        private void ShowEventsForDate(DateTime date)
        {
            listBoxEvents.Items.Clear();
            selectedEventId = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT Id_мероприятия, Название, Время, Вместимость, Записалось 
                    FROM Мероприятия 
                    WHERE CAST(Дата AS DATE) = @date", conn);
                cmd.Parameters.AddWithValue("@date", date);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int eventId = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        TimeSpan time = (TimeSpan)reader.GetValue(2);
                        int capacity = reader.GetInt32(3);
                        int registered = reader.GetInt32(4);

                        string display = $"{title} | {time} | {registered}/{capacity} записано";
                        listBoxEvents.Items.Add(display);
                        listBoxEvents.Tag = eventId; // сохраняем ид
                    }
                }
            }
        }

        private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxEvents.SelectedIndex >= 0)
            {
                // Получаем выб мер
                int index = listBoxEvents.SelectedIndex;

                // повторно получ список меропр
                List<(int id, string title, TimeSpan time, int capacity, int registered)> events = GetEventsForDate(selectedDate);

                var selectedEvent = events[index];
                selectedEventId = selectedEvent.id;

                lblEventDetails.Text = $"Мероприятие: {selectedEvent.title}\n" +
                                       $"Время: {selectedEvent.time}\n" +
                                       $"Записалось: {selectedEvent.registered} из {selectedEvent.capacity}";
            }

        }



        // Получ список мероприятий для опр даты
        private List<(int id, string title, TimeSpan time, int capacity, int registered)> GetEventsForDate(DateTime date)
        {
            var list = new List<(int, string, TimeSpan, int, int)>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT Id_мероприятия, Название, Время, Вместимость, Записалось 
                    FROM Мероприятия 
                    WHERE CAST(Дата AS DATE) = @date", conn);
                cmd.Parameters.AddWithValue("@date", date);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        TimeSpan time = (TimeSpan)reader.GetValue(2);
                        int capacity = reader.GetInt32(3);
                        int registered = reader.GetInt32(4);
                        list.Add((id, title, time, capacity, registered));
                    }
                }
            }
            return list;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedEventId == null)
            {
                MessageBox.Show("Выберите мероприятие из списка");
                return;
            }

            string participantName = txtParticipantName.Text.Trim();
            if (string.IsNullOrEmpty(participantName))
            {
                MessageBox.Show("Введите ваше ФИО"); //в текстбойс
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Проверить есть место
                var cmdCheck = new SqlCommand("SELECT Вместимость, Записалось FROM Мероприятия WHERE Id_мероприятия = @id", conn);
                cmdCheck.Parameters.AddWithValue("@id", selectedEventId);
                int capacity = 0, registered = 0;
                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        capacity = reader.GetInt32(0);
                        registered = reader.GetInt32(1);
                    }
                }

                if (registered >= capacity)
                {
                    MessageBox.Show("Мероприятие заполнено");
                    return;
                }


                // Пров не записан ли пользователь уже
                int currentUserId = GetCurrentUserId(); // получ тек польз это внизу
                var cmdCheckUser = new SqlCommand("SELECT COUNT(*) FROM Записи WHERE Id_мероприятия = @eventId AND Id_польз = @userId", conn);
                cmdCheckUser.Parameters.AddWithValue("@eventId", selectedEventId);
                cmdCheckUser.Parameters.AddWithValue("@userId", currentUserId);
                int alreadyRegistered = (int)cmdCheckUser.ExecuteScalar();
                if (alreadyRegistered > 0)
                {
                    MessageBox.Show("Вы уже записаны на это мероприятие");
                    return;
                }

                // Вст запись
                var cmdInsert = new SqlCommand("INSERT INTO Записи (Id_мероприятия, Id_польз) VALUES (@eventId, @userId)", conn);
                cmdInsert.Parameters.AddWithValue("@eventId", selectedEventId);
                cmdInsert.Parameters.AddWithValue("@userId", currentUserId);
                cmdInsert.ExecuteNonQuery();

                // Обновляем люд записанных
                var cmdUpdate = new SqlCommand("UPDATE Мероприятия SET Записалось = Записалось + 1 WHERE Id_мероприятия = @id", conn);
                cmdUpdate.Parameters.AddWithValue("@id", selectedEventId);
                cmdUpdate.ExecuteNonQuery();

                MessageBox.Show("Вы успешно записались");
            }

            // Обнов список меропр
            ShowEventsForDate(selectedDate);


            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();


        }


        //Тек польз
        private int GetCurrentUserId()
        {
            return CurrentUser.Id;
        }

        private void txtParticipantName_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblEventDetails_Click(object sender, EventArgs e)
        {

        }

        private void Form7_Load(object sender, EventArgs e)
        {
            monthCalendar1.Width = 400; //мб оставить мален //мб удалить
            monthCalendar1.Height = 200;
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }
    }
}
