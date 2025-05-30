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

                if (mlbData?.Dates != null)
                {
                    foreach (var dateEntry in mlbData.Dates) 
                    {
                        if (dateEntry.Games != null)
                        {
                            foreach (var gameDetail in dateEntry.Games) 
                            {
                                
                                DateTime localGameDateTime = gameDetail.GameDate.ToLocalTime();

                                string gameTimeDisplay = localGameDateTime.ToString("HH:mm");

                                bool isOnNextLocalDay = localGameDateTime.Date > startDate.Date;

                                string finalGameTimeDisplay = gameTimeDisplay;
                                if (isOnNextLocalDay)
                                {
                                    finalGameTimeDisplay += " (Next Day)";
                                }

                                games.Add(new MLBGame
                                {
                                    Date = gameDetail.GameDate,
                                    AwayTeam = gameDetail.Teams?.Away?.Team?.Name ?? "N/A",
                                    HomeTeam = gameDetail.Teams?.Home?.Team?.Name ?? "N/A",
                                    GameTime = finalGameTimeDisplay 
                                });
                            }
                        }
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
