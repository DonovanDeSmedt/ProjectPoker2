using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace ProjectPoker.Models.Calculations
{
    public class WinningHand
    {
        private IList<PokerCard> allCards;
        private Hands currentHand;
        public IList<PokerCard> WinningCards { get; set; }
        public WinningHand(IList<PokerCard> cards)
        {
            allCards = cards;
        }

        public bool[] checkWinningHand()
        {
            bool[] winningHands = new bool[9];
            currentHand = Hands.HighestCard;
            bool pair = false, twoPair = false, threeOfAKind = false, straight = false, flush = false;
            WinningCards = GetHighestCard();
            pair = IsPair();
            if (pair)
            {
                winningHands[1] = true;
                currentHand = Hands.Pair;
                WinningCards = GetPair();
                twoPair = IsTwoPair();
                if (twoPair)
                {
                    winningHands[2] = true;
                    currentHand = Hands.TwoPair;
                    WinningCards = GetTwoPair();
                }
            }
            if (pair)
            {
                threeOfAKind = IsThreeOfAKind();
                if (threeOfAKind)
                {
                    winningHands[3] = true;
                    currentHand = Hands.ThreeOfAKind;
                    WinningCards = GetThreeOfAKind();
                }
            }
            straight = IsStraight();
            if (straight)
            {
                winningHands[4] = true;
                currentHand = Hands.Straight;
                WinningCards = GetStraight();
            }
            flush = IsFlush();
            if (flush)
            {
                winningHands[5] = true;
                currentHand = Hands.Flush;
                WinningCards = GetFlush();
            }
            if (pair & threeOfAKind)
            {
                if (IsFullHouse())
                {
                    winningHands[6] = true;
                    currentHand = Hands.FullHouse;
                    WinningCards = GetFullHouse();
                }
            }
            if (threeOfAKind)
            {
                if (IsFourOfAKind())
                {
                    winningHands[7] = true;
                    currentHand = Hands.FourOfAKind;
                    WinningCards = GetThreeOfAKind();
                }
            }
            if (flush && straight)
            {
                if (IsStraightFulsh())
                {
                    winningHands[8] = true;
                    currentHand = Hands.StraightFlush;
                    WinningCards = GetStraightFlush();
                }
            }
            return winningHands;
        }

        public Hands GetWinningHands()
        {
            return currentHand;
        }

        public IList<PokerCard> GetHighestCard()
        {
            return allCards.OrderBy(c => c.FaceValue).ToList();
        } 
        public bool IsPair()
        {
            IList<PokerCard> cards = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList();
            return cards.Count > 0;
        }

        private IList<PokerCard> GetPair()
        {
            return allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList();
        } 
        public bool IsTwoPair()
        {
            IList<PokerCard> cards = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList();
            return cards.Count > 1;
        }

        private IList<PokerCard> GetTwoPair()
        {
            return allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList();
        } 
        private bool IsThreeOfAKind()
        {
            IList<PokerCard> cards = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(2)).ToList();
            return cards.Count > 0;
        }

        private IList<PokerCard> GetThreeOfAKind()
        {
            return allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(2)).ToList();
        } 
        public bool IsStraight()
        {
            //int streak = 1;
            //int value = -1;

            //allCards = allCards.OrderBy(c => (int)c.FaceValue).ToList();
            //foreach (var card in allCards)
            //{
            //    if (value == -1 || value == (int)card.FaceValue)
            //    {
            //        value = (int)card.FaceValue;
            //    }
            //    else if ((value + 1) == (int)card.FaceValue)
            //    {
            //        streak++;
            //    }
            //    else
            //    {
            //        streak = 0;
            //    }
            //    value = (int)card.FaceValue;
            //}
            //if (streak >= 4)
            //{
            //    return true;
            //}
            return GetStraight().Count > 4;
            //return false;
        }

        private IList<PokerCard> GetStraight()
        {
            int streak = 1;
            int value = -1;
            IList<PokerCard> pokerCards = new List<PokerCard>();
            allCards = allCards.OrderBy(c => (int)c.FaceValue).ToList();
            foreach (var card in allCards)
            {
                if (value == -1 || value == (int)card.FaceValue)
                {
                    value = (int)card.FaceValue;
                    pokerCards.Add(card);
                }
                else if ((value + 1) == (int)card.FaceValue)
                {
                    streak++;
                    pokerCards.Add(card);
                }
                else
                {
                    streak = 0;
                    pokerCards = new List<PokerCard>();
                }
                value = (int)card.FaceValue;
            }
            return pokerCards;
        } 
        public bool IsFlush()
        {
            return allCards.GroupBy(c => c.Suit).Any(c => c.Count() > 4);
        }

        private IList<PokerCard> GetFlush()
        {
            IList<PokerCard> pokerCards = new List<PokerCard>();
            Suit suit = allCards.GroupBy(c => c.Suit).Where(group => group.Count() > 4).Select(group => group.Key).First();
            foreach (var card in allCards)
            {
                if (card.Suit == suit)
                {
                    pokerCards.Add(card);
                }
            }
            return pokerCards;
        } 
        public bool IsFullHouse()
        {
            //IList<PokerCard> pairs = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList();
            //IList<PokerCard> three = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(2)).ToList();
            //foreach (var pair in pairs)
            //{
            //    if (three.First().FaceValue != pair.FaceValue)
            //    {
            //        return true;
            //    }
            //}
            //return false;
            return GetFullHouse().Count == 2;
        }

        public IList<PokerCard> GetFullHouse()
        {
            List<PokerCard> pokerCards = new List<PokerCard>();
            IList<PokerCard> pairs = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList();
            IList<PokerCard> three = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(2)).ToList();
            foreach (var pair in pairs)
            {
                if (three.First().FaceValue != pair.FaceValue)
                {
                    pokerCards.Add(pair);
                    //return true;
                }
            }
            pokerCards.AddRange(three);
            return pokerCards;
            //return false;
        } 
        public bool IsFourOfAKind()
        {
            IList<PokerCard> cards = allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(3)).ToList();
            return cards.Count > 0;
        }

        public IList<PokerCard> GetFourOfAKind()
        {
            return allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(3)).ToList();
        } 
        public bool IsStraightFulsh()
        {
            //int streak = 1;
            //int suit = -1;

            //allCards = allCards.OrderBy(c => (int)c.Suit).ToList();
            //foreach (var card in allCards)
            //{
            //    if (suit == -1)
            //    {
            //        suit = (int)card.Suit;
            //    }
            //    else if (suit != (int)card.Suit)
            //    {
            //        streak = 0;
            //    }
            //    else
            //    {
            //        streak++;
            //    }
            //    suit = (int)card.Suit;
            //}
            //if (streak >= 4)
            //{
            //    return true;
            //}
            //return false;
            return GetStraightFlush().Count > 4;
        }

        public IList<PokerCard> GetStraightFlush()
        {
            int streak = 1;
            int suit = -1;
            IList<PokerCard> pokerCards = new List<PokerCard>();
            allCards = allCards.OrderBy(c => (int)c.Suit).ToList();
            foreach (var card in allCards)
            {
                if (suit == -1)
                {
                    suit = (int)card.Suit;
                    pokerCards.Add(card);
                }
                else if (suit != (int)card.Suit)
                {
                    streak = 0;
                    pokerCards = new List<PokerCard>();
                }
                else
                {
                    streak++;
                    pokerCards.Add(card);
                }
                suit = (int)card.Suit;
            }
            return pokerCards;
        } 
    }
    
}