using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar.Scripts.API
{
    public class MLBApiService
    {
        private readonly HttpClient _httpClient;

        public MLBApiService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Asynchronously retrieves a list of upcoming MLB games between the specified start and end dates.
        /// </summary>
        /// <param name="startDate">The start sate for the schedule query.</param>
        /// <param name="endDate">The end date for the schedule query.</param>
        /// <returns>
        /// A list of MLB games.
        /// Returns an empty list if an error occurs during fetching or deserialization.
        /// </returns>
        public async Task<List<MLBGame>> GetUpcomingGamesAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // API
                string url = $"https://statsapi.mlb.com/api/v1/schedule?sportId=1&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&language=en";
                var response = await _httpClient.GetStringAsync(url);
                var mlbData = JsonConvert.DeserializeObject<MLBResponse>(response);

                List<MLBGame> games = new List<MLBGame>();

                foreach (var date in mlbData.Dates)
                {
                    foreach (var game in date.Games)
                    {
                        games.Add(new MLBGame
                        {
                            Date = game.GameDate,
                            AwayTeam = game.Teams.Away.Team.Name,
                            HomeTeam = game.Teams.Home.Team.Name,
                            GameTime = game.GameDate.ToLocalTime().ToString("hh:mm tt")
                        });
                    }
                }

                return games;
            }
            catch (Exception ex)
            {
                // Error handling
                MessageBox.Show("Error fetching MLB games: " + ex.Message);
                return new List<MLBGame>();
            }
        }
    }
}
