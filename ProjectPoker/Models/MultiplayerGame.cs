using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectPoker.Controllers;

namespace ProjectPoker.Models
{
    public static class MultiplayerGame
    {
        public static Poker Poker { get; set; }
        public static PokerController PokerController { get; set; }
        public static Dictionary<string, Poker> PDictionary { get; set; }
        public static string ServerPlayer { get; set; }
        public static bool AccesLock { get; set; }
        public static List<string> Names { get; set; }
        public static bool DeleteMultiplayer { get; set; }
    }
}