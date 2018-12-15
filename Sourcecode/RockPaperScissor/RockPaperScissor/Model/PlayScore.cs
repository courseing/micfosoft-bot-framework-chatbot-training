using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RockPaperScissor.Model
{
    [Serializable]
    public class PlayScore
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public int UserScore { get; set; }
        public int BotScore { get; set; }
    }
}