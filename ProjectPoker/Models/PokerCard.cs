using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ProjectPoker.Models
{
    public class PokerCard
    {
        public PokerCard(Suit suit, FaceValue faceValue)
        {
            Suit = suit;
            FaceValue = faceValue;
            FaceUp = false;
        }

        public bool FaceUp { get; private set; }

        public Suit Suit { get; private set; }

        public FaceValue FaceValue { get; private set; }

        public void TurnCard()
        {
            FaceUp = !FaceUp;
        }
    }
}