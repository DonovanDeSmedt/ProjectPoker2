using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectPoker.Models;

namespace ProjectPoker.ViewModels
{
    public class PokerViewModel
    {
        public IPlayer ActivePlayer { get; set; }
        public Deck Deck { get; set; }
        public IList<IPlayer> ListBots { get; set; }
        public IList<IPlayer> Players { get; set; }
        public Table Table { get; set; }

        public PokerViewModel()
        {

        }

        public PokerViewModel(IPlayer activePlayer, Deck deck, IList<IPlayer> listBots, IList<IPlayer> players, Table table)
        {
            ActivePlayer = activePlayer;
            Deck = deck;
            ListBots = listBots;
            Players = players;
            Table = table;
        }
    }
}