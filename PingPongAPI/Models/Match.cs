using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PingPongAPI.Models
{
    public class Match
	{
        public Match(string player1, string player2, string groupName, string matchType)
        {
            Player1 = player1;
            Player2 = player2;
            GroupName = groupName;
            MatchType = matchType;
        }

        public Match(string player1, string player2, int playoffMatchNr, string matchType)
        {
            Player1 = player1;
            Player2 = player2;
            PlayoffMatchNr = playoffMatchNr;
            MatchType = matchType;
        }

        public Match()
        {
        }

        public Match(int resultPlayer1, int resultPlayer2)
        {
            ResultPlayer1 = resultPlayer1;
            ResultPlayer2 = resultPlayer2;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Player1 { get; set; }

        [Required]
        public string Player2 { get; set; }

        public int ResultPlayer1 { get; set; } = 0;

        public int ResultPlayer2 { get; set; } = 0;

        public string GroupName { get; set; }

        public string MatchType { get; set; }

        public int PlayoffMatchNr { get; set; }

        public string Winner { get; set; }
    } 
}

