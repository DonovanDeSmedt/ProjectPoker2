using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace ProjectPoker.Models
{
    public class Deck
    {
        private static Random random = new Random();
        public Deck()
        {
            Pokercards = new List<PokerCard>();
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
            {
                foreach (FaceValue f in Enum.GetValues(typeof(FaceValue)))
                {
                    Pokercards.Add(new PokerCard(s, f));
                }
            }
            Shuffle();
        }

        public virtual IList<PokerCard> Pokercards { get; set; }

        public PokerCard Draw()
        {
            if(Pokercards.Count == 0)
                throw new InvalidOperationException("Deck is empty");
            PokerCard card = Pokercards[0];
            Pokercards.RemoveAt(0);
            return card;
        }

        public IList<PokerCard> GetCards()
        {
            return Pokercards;
        }

        public void Shuffle()
        {
            for (int i = 1; i < Pokercards.Count * 3; i++)
            {
                int randomPosition = random.Next(0, Pokercards.Count);
                PokerCard card = Pokercards[randomPosition];
                Pokercards.RemoveAt(randomPosition);
                Pokercards.Add(card);
            }
        }
    }
}