using Calendar.Scripts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Calendar.Scripts
{
    public class EventRepository : IEventRepository
    {
        private static readonly Lazy<EventRepository> _instance = new Lazy<EventRepository>(() => new EventRepository());

        /// <summary>
        /// Gets the Singleton instance of the EventRepository.
        /// </summary>
        public static EventRepository Instance => _instance.Value;

        private readonly string _connectionString = "Data Source=events.db";

        private EventRepository()
        {
            CreateEventsTableIfNotExists();
        }

        /// <summary>
        /// Creates the "Events" table in the database if it does not already exist.
        /// </summary>
        private void CreateEventsTableIfNotExists()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Events (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, Time TEXT, Title TEXT, Tag TEXT)",
                    connection);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves all football events for a specific date.
        /// </summary>
        /// <param name="date">The date for which to retrieve events.</param>
        /// <returns>A list of FootballEvent objects for the given date.</returns>
        public List<FootballEvent> GetEvents(DateTime date)
        {
            var events = new List<FootballEvent>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Events WHERE Date = @date", connection);
                cmd.Parameters.AddWithValue("@date", date.ToShortDateString());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new FootballEvent
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Date = DateTime.Parse(reader["Date"].ToString()),
                            Time = TimeSpan.Parse(reader["Time"].ToString()),
                            Title = reader["Title"].ToString(),
                            Tag = reader["Tag"].ToString()
                        });
                    }
                }
            }

            return events;
        }

        /// <summary>
        /// Adds a new football event to the database.
        /// </summary>
        /// <param name="footballEvent">The FootballEvent to add.</param>
        public void AddEvent(FootballEvent footballEvent)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("INSERT INTO Events (Date, Time, Title, Tag) VALUES (@date, @time, @title, @tag)", connection);
                cmd.Parameters.AddWithValue("@date", footballEvent.Date.ToShortDateString());
                cmd.Parameters.AddWithValue("@time", footballEvent.Time.ToString());
                cmd.Parameters.AddWithValue("@title", footballEvent.Title);
                cmd.Parameters.AddWithValue("@tag", footballEvent.Tag);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates an existing football event in the database.
        /// </summary>
        /// <param name="footballEvent">The FootballEvent with updated info. The ID is used to find the event.</param>
        public void UpdateEvent(FootballEvent footballEvent)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("UPDATE Events SET Time = @time, Title = @title, Tag = @tag WHERE Id = @id", connection);
                cmd.Parameters.AddWithValue("@id", footballEvent.Id);
                cmd.Parameters.AddWithValue("@time", footballEvent.Time.ToString());
                cmd.Parameters.AddWithValue("@title", footballEvent.Title);
                cmd.Parameters.AddWithValue("@tag", footballEvent.Tag);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a football event from the database.
        /// </summary>
        /// <param name="footballEvent">The FootballEvent to delete. The ID is used to find the event.</param>
        public void DeleteEvent(FootballEvent footballEvent)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("DELETE FROM Events WHERE Id = @id", connection);
                cmd.Parameters.AddWithValue("@id", footballEvent.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}