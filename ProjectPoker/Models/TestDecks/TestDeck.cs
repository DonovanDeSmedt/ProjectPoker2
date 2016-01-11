using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectPoker.Models.TestDecks
{
    public class TestDeck:Deck
    {
        public TestDeck()
        {
            ////PAIR
            //Pokercards = new List<PokerCard>()
            //{
            //    // 2 KAARTEN VOOR PLAYER
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Clubs, FaceValue.Three),
            //    // PAIR 2 KAARTEN VOOR BOT
            //    new PokerCard(Suit.Hearts, FaceValue.Six),
            //    new PokerCard(Suit.Clubs, FaceValue.Eight),
            //    // KAARTEN VOOR TAFEL
            //    new PokerCard(Suit.Clubs, FaceValue.Nine),
            //    new PokerCard(Suit.Diamonds, FaceValue.Six),
            //    new PokerCard(Suit.Spiders, FaceValue.Seven),
            //    new PokerCard(Suit.Hearts, FaceValue.Ace),
            //    new PokerCard(Suit.Hearts, FaceValue.Four),
            //};
            ////TWO PAIR
            //Pokercards = new List<PokerCard>()
            //{
            //    // 2 KAARTEN VOOR PLAYER
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Clubs, FaceValue.Three),
            //    // PAIR 2 KAARTEN VOOR BOT
            //    new PokerCard(Suit.Hearts, FaceValue.Six),
            //    new PokerCard(Suit.Clubs, FaceValue.Eight),
            //    // KAARTEN VOOR TAFEL
            //    new PokerCard(Suit.Clubs, FaceValue.Eight),
            //    new PokerCard(Suit.Diamonds, FaceValue.Six),
            //    new PokerCard(Suit.Spiders, FaceValue.Seven),
            //    new PokerCard(Suit.Hearts, FaceValue.Ace),
            //    new PokerCard(Suit.Hearts, FaceValue.Four),
            //};
            //// THREE-OF-A-KIND
            //Pokercards = new List<PokerCard>()
            //{
            //    // 2 KAARTEN VOOR PLAYER
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Clubs, FaceValue.Three),
            //    // PAIR 2 KAARTEN VOOR BOT
            //    new PokerCard(Suit.Hearts, FaceValue.Six),
            //    new PokerCard(Suit.Clubs, FaceValue.Eight),
            //    // KAARTEN VOOR TAFEL
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Diamonds, FaceValue.Six),
            //    new PokerCard(Suit.Spiders, FaceValue.Six),
            //    new PokerCard(Suit.Hearts, FaceValue.Ace),
            //    new PokerCard(Suit.Hearts, FaceValue.Four),
            //};
            ////STRAIGHT
            //Pokercards = new List<PokerCard>()
            //{
            //    // 2 KAARTEN VOOR PLAYER
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Clubs, FaceValue.Three),
            //    // PAIR 2 KAARTEN VOOR BOT
            //    new PokerCard(Suit.Hearts, FaceValue.Six),
            //    new PokerCard(Suit.Clubs, FaceValue.Four),
            //    // KAARTEN VOOR TAFEL
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Diamonds, FaceValue.Three),
            //    new PokerCard(Suit.Spiders, FaceValue.Five),
            //    new PokerCard(Suit.Hearts, FaceValue.Ace),
            //    new PokerCard(Suit.Hearts, FaceValue.Four),
            //};
            ////FLUSH
            //Pokercards = new List<PokerCard>()
            //{
            //    // 2 KAARTEN VOOR PLAYER
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Clubs, FaceValue.Three),
            //    // PAIR 2 KAARTEN VOOR BOT
            //    new PokerCard(Suit.Hearts, FaceValue.Six),
            //    new PokerCard(Suit.Hearts, FaceValue.King),
            //    // KAARTEN VOOR TAFEL
            //    new PokerCard(Suit.Hearts, FaceValue.Two),
            //    new PokerCard(Suit.Hearts, FaceValue.Eight),
            //    new PokerCard(Suit.Hearts, FaceValue.Five),
            //    new PokerCard(Suit.Hearts, FaceValue.Ace),
            //    new PokerCard(Suit.Hearts, FaceValue.Four),
            //};
            ////FULL HOUSE
            //Pokercards = new List<PokerCard>()
            //{
            //    // 2 KAARTEN VOOR PLAYER
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Clubs, FaceValue.Three),
            //    // 2 KAARTEN VOOR BOT
            //    new PokerCard(Suit.Hearts, FaceValue.Six),
            //    new PokerCard(Suit.Clubs, FaceValue.Four),
            //    //KAARTEN VOOR TAFEL
            //    new PokerCard(Suit.Clubs, FaceValue.Six),
            //    new PokerCard(Suit.Diamonds, FaceValue.Four),
            //    new PokerCard(Suit.Spiders, FaceValue.Four),
            //    new PokerCard(Suit.Hearts, FaceValue.Ace),
            //    new PokerCard(Suit.Hearts, FaceValue.Four),
            //};
            ////FOUR-OF-A-KIND
            //Pokercards = new List<PokerCard>()
            //{
            //    // 2 KAARTEN VOOR PLAYER
            //    new PokerCard(Suit.Clubs, FaceValue.Two),
            //    new PokerCard(Suit.Clubs, FaceValue.Three),
            //    // PAIR 2 KAARTEN VOOR BOT
            //    new PokerCard(Suit.Hearts, FaceValue.Six),
            //    new PokerCard(Suit.Clubs, FaceValue.Six),
            //    // KAARTEN VOOR TAFEL
            //    new PokerCard(Suit.Spiders, FaceValue.Six),
            //    new PokerCard(Suit.Diamonds, FaceValue.Six),
            //    new PokerCard(Suit.Spiders, FaceValue.Four),
            //    new PokerCard(Suit.Hearts, FaceValue.Ace),
            //    new PokerCard(Suit.Hearts, FaceValue.Four),
            //};
            //STRAIGHT FLUSH
            Pokercards = new List<PokerCard>()
            {
                // 2 KAARTEN VOOR PLAYER
                new PokerCard(Suit.Clubs, FaceValue.Two),
                new PokerCard(Suit.Clubs, FaceValue.Three),
                // PAIR 2 KAARTEN VOOR BOT
                new PokerCard(Suit.Hearts, FaceValue.Six),
                new PokerCard(Suit.Hearts, FaceValue.Four),
                // KAARTEN VOOR TAFEL
                new PokerCard(Suit.Hearts, FaceValue.Five),
                new PokerCard(Suit.Hearts, FaceValue.Three),
                new PokerCard(Suit.Hearts, FaceValue.Seven),
                new PokerCard(Suit.Hearts, FaceValue.Ace),
                new PokerCard(Suit.Hearts, FaceValue.Four),
            };
        }
    }
}