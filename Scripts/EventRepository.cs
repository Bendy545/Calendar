using Calendar.Scripts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Calendar.Scripts
{
    public interface IEventRepository
    {
        List<FootballEvent> GetEvents(DateTime date);
        void AddEvent(FootballEvent footballEvent);
    }

    public class EventRepository : IEventRepository
    {
        private static readonly Lazy<EventRepository> _instance = new Lazy<EventRepository>(() => new EventRepository()); public static EventRepository Instance => _instance.Value;

        private readonly SQLiteConnection _connection;

        private EventRepository()
        {
            _connection = new SQLiteConnection("Data Source=events.db");
            _connection.Open();
            var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Events (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, Title TEXT, Tag TEXT)", _connection);
            cmd.ExecuteNonQuery();
        }

        public List<FootballEvent> GetEvents(DateTime date)
        {
            var list = new List<FootballEvent>();
            var cmd = new SQLiteCommand("SELECT * FROM Events WHERE Date = @date", _connection);
            cmd.Parameters.AddWithValue("@date", date.ToShortDateString());
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new FootballEvent
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        Title = reader["Title"].ToString(),
                        Tag = reader["Tag"].ToString()
                    });
                }
            }

            return list;
        }

        public void AddEvent(FootballEvent footballEvent)
        {
            var cmd = new SQLiteCommand("INSERT INTO Events (Date, Title, Tag) VALUES (@date, @title, @tag)", _connection);
            cmd.Parameters.AddWithValue("@date", footballEvent.Date.ToShortDateString());
            cmd.Parameters.AddWithValue("@title", footballEvent.Title);
            cmd.Parameters.AddWithValue("@tag", footballEvent.Tag);
            cmd.ExecuteNonQuery();
        }
    }
}