using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.EnterpriseServices;

namespace ProjectPoker.Models
{
    public interface IPlayer
    {
        IList<PokerCard> PokerCards { get; set; }

        Table Table { get; set; }

        string Name { get; set; }
        int Money { get; set; }
        int SeatNr { get; set; }
        bool Folded { get; set; }
        bool Winner { get; set; }
        Hands CurrentHand { get; set; }
        bool IsDealer { get; set; }
        bool IsBigBlind { get; set; }
        bool IsSmallBlind { get; set; }
        int BigBlind { get; }
        int CurrentBet { get; set; }
        int PrizeMoney { get; set; }
        IList<PokerCard> WinningCards { get; set; }
        IList<IPlayer> Opponents { get; set; }
        void AddCard(PokerCard card);

        void AddMoney(int money);


        void Bet(int amount);

        void Check();

        void Fold();

        void Raise(int amount);

        void TurnAllCards();
        bool CardsVisible();
    }
}