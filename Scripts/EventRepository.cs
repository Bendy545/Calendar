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
        void UpdateEvent(FootballEvent footballEvent);
        void DeleteEvent(FootballEvent footballEvent);
    }

    public class EventRepository : IEventRepository
    {
        private static readonly Lazy<EventRepository> _instance = new Lazy<EventRepository>(() => new EventRepository());
        public static EventRepository Instance { get { return _instance.Value; } }

        private readonly SQLiteConnection _connection;

        private EventRepository()
        {
            _connection = new SQLiteConnection("Data Source=events.db");
            _connection.Open();

            var cmd = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS Events (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, Time TEXT, Title TEXT, Tag TEXT)",
                _connection);
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
                        Time = TimeSpan.Parse(reader["Time"].ToString()),
                        Title = reader["Title"].ToString(),
                        Tag = reader["Tag"].ToString()
                    });
                }
            }

            return list;
        }

        public void AddEvent(FootballEvent footballEvent)
        {
            var cmd = new SQLiteCommand("INSERT INTO Events (Date, Time, Title, Tag) VALUES (@date, @time, @title, @tag)", _connection);
            cmd.Parameters.AddWithValue("@date", footballEvent.Date.ToShortDateString());
            cmd.Parameters.AddWithValue("@time", footballEvent.Time.ToString());
            cmd.Parameters.AddWithValue("@title", footballEvent.Title);
            cmd.Parameters.AddWithValue("@tag", footballEvent.Tag);
            cmd.ExecuteNonQuery();
        }

        public void UpdateEvent(FootballEvent footballEvent)
        {
            var cmd = new SQLiteCommand("UPDATE Events SET Time = @time, Title = @title, Tag = @tag WHERE Id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", footballEvent.Id);
            cmd.Parameters.AddWithValue("@time", footballEvent.Time.ToString());
            cmd.Parameters.AddWithValue("@title", footballEvent.Title);
            cmd.Parameters.AddWithValue("@tag", footballEvent.Tag);
            cmd.ExecuteNonQuery();
        }

        public void DeleteEvent(FootballEvent footballEvent)
        {
            var cmd = new SQLiteCommand("DELETE FROM Events WHERE Id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", footballEvent.Id);
            cmd.ExecuteNonQuery();
        }
    }
}