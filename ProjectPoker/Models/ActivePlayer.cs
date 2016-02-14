using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using ProjectPoker.Models;
using ProjectPoker.Models.Calculations;

namespace ProjectPoker.Models
{
    public class ActivePlayer : IPlayer
    {
        //private IList<PokerCard> pokerCards; 

        public ActivePlayer(string name, Table table, int seatNr)
        {
            Name = name;
            SeatNr = seatNr;
            Table = table;
            PokerCards = new List<PokerCard>();
            Money += 1000;
            Folded = false;
        }

        public ActivePlayer(string name, Table table, int seatNr, int money)
        {
            Name = name;
            SeatNr = seatNr;
            Table = table;
            PokerCards = new List<PokerCard>();
            Money = money;
            Folded = false;
        }
        public ActivePlayer()
        {

        }

        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public int Money { get; set; }
        public int SeatNr { get; set; }
        public IList<PokerCard> PokerCards { get; set; }
        public Table Table { get; set; }
        public string Email { get; set; }
        public bool LookingForOpponent { get; set; }
        public bool Folded { get; set; }

        public bool Winner { get; set; }

        public Hands CurrentHand { get; set; }
        public bool IsDealer { get; set; }
        public bool IsBigBlind { get; set; }
        public bool IsSmallBlind { get; set; }

        public int BigBlind { get { return 10; } }
        public int CurrentBet { get; set; }
        public int PrizeMoney { get; set; }
        public IList<PokerCard> WinningCards { get; set; }
        public IList<IPlayer> Opponents { get; set; }
        public string JoinDateTime { get; set; }

        public void AddCard(PokerCard card)
        {
            if (PokerCards.Count > 2)
                throw new InvalidOperationException("Pokerplayer cannot have more than 2 cards in his hand");
            PokerCards.Add(card);
        }

        public void AddMoney(int amount)
        {
            Money += amount;
            PrizeMoney += amount;
        }

        public void Bet(int amount)
        {
            if (amount > Money)
            {
                throw new InvalidOperationException("You cannot bet more money than you have, which is " + Money);
            }
            Table.Raise(amount);
            Money -= (amount - CurrentBet);
            CurrentBet += (amount - CurrentBet);
        }

        public void Check()
        {
            int amount = Table.CurrentBet - CurrentBet;
            if (amount >= Money)
            {
                Table.Check(Money);
                Money -= Money;
                CurrentBet = amount + CurrentBet;
            }
            else
            {
                Table.Check(amount);
                if (amount != 0)
                {
                    Money -= amount;
                    CurrentBet += amount;
                }
            }
        }

        public void Fold()
        {
            Folded = true;
        }

        public void Raise(int amount)
        {
            if (amount > Money)
            {
                throw new InvalidOperationException("You cannot bet more money than you have");
            }
            Table.AddBet(amount);
        }

        public void TurnAllCards()
        {
            foreach (var p in PokerCards)
            {
                p.TurnCard();
            }
        }

        public bool CardsVisible()
        {
            return true;
        }

        public void SetCurrentHand()
        {
            List<PokerCard> allCards = Table.Pokercards.Where(c => c.FaceUp).ToList();
            foreach (var card in PokerCards)
            {
                allCards.Add(card);
            }
            WinningHand winningHand = new WinningHand(allCards);
            winningHand.checkWinningHand();
            CurrentHand = winningHand.GetWinningHands();
            WinningCards = winningHand.WinningCards;
        }
    }
}