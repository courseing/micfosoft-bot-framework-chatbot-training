using RockPaperScissor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RockPaperScissor.ViewModel
{
    [Serializable]
    public class Game
    {
        public PlayScore score;

        public Game()
        {
            score = new PlayScore();
        }

        readonly Dictionary<PlayType, string> rockPlays = new Dictionary<PlayType, string>
        {
            [PlayType.Paper] = "Paper covers rock - you lose",
            [PlayType.Scissor] = "Rock crushes scissors -You win!"
        };

        readonly Dictionary<PlayType, string> paperPlays = new Dictionary<PlayType, string>
        {
            [PlayType.Rock] = "Paper covers rock - you win",
            [PlayType.Scissor] = "Scissor cuts paper - you lose"
        };

        readonly Dictionary<PlayType, string> scissorPlays = new Dictionary<PlayType, string>
        {
            [PlayType.Paper] = "Scissor cuts paper - you win",
            [PlayType.Rock] = "Rock crushes scissors - you lost"
        };

        public string Play(string userText)
        {
            string message = "";
            PlayType userPlay;

            bool isValidPlay = Enum.TryParse(userText, ignoreCase: true, result: out userPlay);
            if(isValidPlay)
            {
                PlayType botPlay = GetBotPlay();
                message = Compare(userPlay, botPlay);
            }
            else
            {
                message = "Type \"Rock\", \"Paper\", or\"Scissors\" to play.";
            }
            return message;
        }

        private string Compare(PlayType userPlay, PlayType botPlay)
        {
            string plays = $"You: { userPlay}, Bot: { botPlay}";
            string result = "";
            if (userPlay == botPlay)
                result = "Tie.";
            else
                switch (userPlay)
                {
                    case PlayType.Rock:
                        result = rockPlays[botPlay];
                        break;
                    case PlayType.Paper:
                        result = paperPlays[botPlay];
                        break;
                    case PlayType.Scissor:
                        result = scissorPlays[botPlay];
                        break;
                }
            return $"{ plays}. { result}";
        }

        public PlayType GetBotPlay()
        {
            long seed = DateTime.Now.Ticks;
            var rnd = new Random(unchecked((int)seed));
            int position = rnd.Next(maxValue: 3);
            return (PlayType)position;
        }

    }
}