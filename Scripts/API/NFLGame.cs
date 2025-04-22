using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts
{
    public enum GameStatus
    {
        Scheduled,
        InProgress,
        Final,
        Canceled
    }

    public class NFLGame
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string HomeTeam { get; set; }  
        public string HomeTeamName { get; set; } 
        public string AwayTeam { get; set; }
        public string AwayTeamName { get; set; }
        public string HomeScore { get; set; }
        public string AwayScore { get; set; }
        public GameStatus Status { get; set; }
        public string Stadium { get; set; }
        public string Broadcast { get; set; }

        public string Matchup => $"{AwayTeam} @ {HomeTeam}";
        public string ScoreDisplay
        {
            get
            {
                switch (Status)
                {
                    case GameStatus.Final:
                        return $"{AwayScore} - {HomeScore}";
                    case GameStatus.InProgress:
                        return "LIVE";
                    default:
                        return "VS";
                }
            }
        }

        
    }
}
