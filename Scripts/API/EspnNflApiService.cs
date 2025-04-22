using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    public class EspnNflApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://site.api.espn.com/apis/site/v2/sports/football/nfl/scoreboard";

        public EspnNflApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<List<NFLGame>> GetGamesAsync(DateTime date)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{BaseUrl}?dates={date:yyyyMMdd}");
                var espnResponse = JsonConvert.DeserializeObject<EspnScoreboard>(response);

                var games = new List<NFLGame>();
                foreach (var eventItem in espnResponse.Events)
                {
                    var competition = eventItem.Competitions.FirstOrDefault();
                    if (competition == null) continue;

                    var homeTeam = competition.Competitors?.FirstOrDefault(c => c.HomeAway == "home");
                    var awayTeam = competition.Competitors?.FirstOrDefault(c => c.HomeAway == "away");

                    if (homeTeam == null || awayTeam == null) continue;

                    var game = new NFLGame
                    {
                        Id = eventItem.Id,
                        Date = DateTime.Parse(eventItem.Date),
                        HomeTeam = homeTeam.Team?.Abbreviation ?? "HOME",
                        HomeTeamName = homeTeam.Team?.DisplayName ?? "Home Team",
                        AwayTeam = awayTeam.Team?.Abbreviation ?? "AWAY",
                        AwayTeamName = awayTeam.Team?.DisplayName ?? "Away Team",
                        HomeScore = homeTeam.Score ?? "0",
                        AwayScore = awayTeam.Score ?? "0",
                        Status = ParseGameStatus(eventItem.Status?.Type?.Name),
                        Stadium = competition.Venue?.FullName ?? "Unknown Stadium",
                        Broadcast = competition.Broadcasts?.FirstOrDefault()?.Names?.FirstOrDefault() ?? "Unknown"
                    };

                    games.Add(game);
                }
                return games;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching NFL games: {ex.Message}");
                return new List<NFLGame>();
            }
        }

        private GameStatus ParseGameStatus(string status)
        {
            switch (status)
            {
                case "STATUS_FINAL":
                    return GameStatus.Final;
                case "STATUS_IN_PROGRESS":
                    return GameStatus.InProgress;
                case "STATUS_CANCELED":
                    return GameStatus.Canceled;
                default:
                    return GameStatus.Scheduled;
            }
        }
    }
}
