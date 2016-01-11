using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ProjectPoker;
using ProjectPoker.Models;

namespace ProjectPoker.Models
{
    public class Table
    {

        public Table()
        {
            Pokercards = new List<PokerCard>();
            Players = new List<IPlayer>();
            TableMoney = 0;
            CurrentBet = 0;
        }

        public virtual IList<PokerCard> Pokercards { get; private set; }

        public virtual IList<IPlayer> Players { get; private set; }
        public int CurrentBet { get; set; }
        public int TableMoney { get; set; }

        public void AddBet(int amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Bet cannot be zero or negative !");
            }
            else if (amount <= CurrentBet)
            {
                throw new InvalidOperationException("Bet cannot be lower/equal than previous bet, which was "+CurrentBet);
            }
            
            TableMoney += amount;
            if (amount > CurrentBet)
            {
                CurrentBet = amount;
            }
        }

        public void Raise(int amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Bet cannot be zero or negative !");
            }
            else if (amount <= CurrentBet)
            {
                throw new InvalidOperationException("Bet cannot be lower/equal than previous bet, which was " + CurrentBet);
            }

            TableMoney += amount;
            if (amount > CurrentBet)
            {
                CurrentBet += (amount - CurrentBet);
            }
        }
        public void Check(int amount)
        {
            TableMoney += amount;
        }

        public void AddCard(PokerCard card)
        {
            if (Pokercards.Count == 5)
            {
                throw new InvalidOperationException("There are already 5 cards on the table");
            }
            Pokercards.Add(card);
        }

        public void TurnAllCards()
        {
            foreach (var card in Pokercards)
            {
                card.TurnCard();
            }
        }
    }
}