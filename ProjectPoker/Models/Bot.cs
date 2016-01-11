using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using ProjectPoker.Models.Calculations;

namespace ProjectPoker.Models
{
    public class Bot : IPlayer
    {
        private string username;
        private Hands previousHand = Hands.HighestCard;
        private IList<PokerCard> allCards;
        private Calculations.WinningHand winningHand;
        private Chances chances;
        private bool[] currentHands;
        private double highestBet = 0;
        private static Random random = new Random();
        private int allChecker = random.Next(1, 250);
        public Bot(string name, Table table, int seatNr)
        {
            Name = name;
            Table = table;
            SeatNr = seatNr;
            Money += 1000;
            PokerCards = new List<PokerCard>();
            Folded = false;
        }
        public string Name { get; set; }

        public int SeatNr { get; set; }
        public IList<PokerCard> PokerCards { get; set; }

        public int Money { get; set; }
        public Table Table { get; set; }

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
            if (amount > Money)
            {
                Table.Check(Money);
                CurrentBet = amount + CurrentBet;
                Money = 0;
            }
            else
            {
                Table.Check(amount);
                if (amount != 0)
                {
                    CurrentBet += amount;
                    Money -= amount;
                }               
            }
        }

        public void Fold()
        {
            Folded = true;
        }

        public void Raise(int amount)
        {
            throw new System.NotImplementedException();
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
            return false;
        }


        public void MakeDecision(int amoutOfPlayers)
        {
            int wait = random.Next(500, 5000);
            System.Threading.Thread.Sleep(wait);
            double current = 0;
            double futureChance = 0;
            CheckWinningHand(amoutOfPlayers);
            double[] chances = CalculateChances(amoutOfPlayers);
            //int allChecker = random.Next(1, 250);
            // op basis van deze zaken/kansen raisen/checken/folden
            // inzetten op basis van de huidige winning hand
            if (previousHand != CurrentHand)
            {
                previousHand = CurrentHand;
                current = Money;
                switch (CurrentHand)
                {
                    case Hands.HighestCard:
                    {
                        current *= ((random.NextDouble()*(0.02 - 0.01)) + 0.01);
                        break;
                    }
                    case Hands.Pair:
                    {
                        current *= ((random.NextDouble() * (0.06 - 0.025)) + 0.025);
                            break;
                    }
                    case Hands.TwoPair:
                    {
                        current *= ((random.NextDouble() * (0.18 - 0.06)) + 0.03);
                            break;
                    }
                    case Hands.ThreeOfAKind:
                    {
                        current *= ((random.NextDouble() * (0.2 - 0.09)) + 0.09);
                            break;
                    }
                    case Hands.Straight:
                    {
                        current *= ((random.NextDouble() * (0.35 - 0.05)) + 0.05);
                            break;
                    }
                    case Hands.Flush:
                    {
                        current *= ((random.NextDouble() * (0.4 - 0.05)) + 0.05);
                            break;
                    }
                    case Hands.FullHouse:
                    {
                        current *= ((random.NextDouble() * (0.5 - 0.05)) + 0.05);
                            break;
                    }
                    case Hands.FourOfAKind:
                    {
                        current *= ((random.NextDouble() * (0.6 - 0.05)) + 0.05);
                            break;
                    }
                    case Hands.StraightFlush:
                    {
                        current *= 1;
                            break;
                    }
                }
            }
            //inzetten op basis van de kansen
            double df = Money * ((random.NextDouble() * (0.02 - 0.01)) + 0.01);
            //Pair
            futureChance = chances[0] > 20 ? Money*((random.NextDouble()*(0.02 - 0.01)) + 0.01) : futureChance;
            //Two Pair
            futureChance = chances[1] > 14 ? Money* ((random.NextDouble()*(0.035 - 0.02)) + 0.02) : futureChance;
            //Three-of-a-kind
            futureChance = chances[2] > 8 ? Money * ((random.NextDouble() * (0.045 - 0.03)) + 0.03) : futureChance;
            //Straigth
            futureChance = chances[5] > 12 ? Money * ((random.NextDouble() * (0.1 - 0.05)) + 0.05) : futureChance;
            futureChance = chances[5] > 17 ? Money * ((random.NextDouble() * (0.14 - 0.07)) + 0.07) : futureChance;
            //Flush
            futureChance = chances[3] > 20 ? Money * ((random.NextDouble() * (0.085 - 0.04)) + 0.04) : futureChance;
            //Full house
            futureChance = chances[4] > 10 ? Money * ((random.NextDouble() * (0.12 - 0.05)) + 0.05) : futureChance;
            //Four-of-a-kind
            futureChance = chances[6] > 4 ? Money * ((random.NextDouble() * (0.13 - 0.05)) + 0.05) : futureChance;
            //Straight flush
            futureChance = chances[7] > 4 ? Money * ((random.NextDouble() * (0.1 - 0.05)) + 0.05) : futureChance;
            int bet = (int)(current + futureChance);
            // Wanneer de bot raised met 2 of 3, hem laten checken. Anders blijft er steeds met beetjes hoger gebet.
            if (bet <= Table.CurrentBet-CurrentBet * 1.2 && bet >= Table.CurrentBet-CurrentBet)
            {
                bet = Table.CurrentBet-CurrentBet;
            }
            // Wanneer we dezelfde ronde 2 keer spelen, omdat de bets verschillen, de bot niet opnieuw een hoger bet laten maken
            // Anders kunnen we bezig blijven.
            if (CurrentBet != 0)
            {
                bet = 0;
            }
            highestBet = bet > highestBet ? bet : highestBet;

            //indien er nog geen kaarten zijn omgedraaid.
            if (Table.Pokercards.Where(c => c.FaceUp).ToList().Count == 0)
            {
                // Als het bedrag dat gebet werd in de eerste ronde < 15% van het huidige budget               
                // ander wordt er gefold
                if ((Table.CurrentBet <= ((random.NextDouble() * (100 - 50)) + 50)) && (allChecker < 60 || allChecker > 70))
                {
                    Check();
                }
                //als de allchecker tussen 60 en 70 ligt zal er een bet geplaatst worden
                //(1 speler die bluft en voortdurend volgt = allchecker)
                //Initieel zal hij een bet plaatsen
                else if ((allChecker >= 60 && allChecker <= 70))
                {
                    if (Table.CurrentBet >= Money)
                    {
                        Bet(Money);
                    }
                    else
                    {
                        Bet(random.Next(Table.CurrentBet, (int)(Money*0.05)));
                    }
                   
                }
                else
                {
                    Fold();
                }
                
            }
            //Indien er al kaarten zijn omgedraaid
            else
            {
                if (Table.CurrentBet -CurrentBet > (bet*1.5) && (Money * ((random.NextDouble()*(0.15-0.01))+0.01)) <= Table.CurrentBet-CurrentBet && Table.CurrentBet-CurrentBet > (highestBet * 1.2) )
                {
                    Fold();
                }
                else
                {
                    if (bet >= Money && Table.CurrentBet-CurrentBet < bet && (allChecker > 60 && allChecker < 70))
                    {
                        Bet(Money);
                    }
                    //Als het huidige bet minder dan 5% van het huidige budget bevat wordt er ook gechecked
                    else if ((bet <= (Table.CurrentBet-CurrentBet) || (allChecker > 60 && allChecker < 70)))
                    {
                        Check();
                    }
                    else
                    {
                        Bet(bet);             
                    }

                }
            }
        }
        private double[] CalculateChances(int amountOfPlayers)
        {

            chances = new Chances(allCards);
            bool isPair = currentHands[1];
            bool isTwoPair = currentHands[2];
            bool isThreeOfAKind = currentHands[3];
            bool isStraight = currentHands[4];
            bool isFlush = currentHands[5];
            bool isFullHouse = currentHands[6];
            bool isFourOfAKind = currentHands[7];
            bool isStraightFlush = currentHands[8];

            double pair = isPair ? 100.0 : chances.CalculateChanceForPair(amountOfPlayers, allCards);
            double twoPair = isTwoPair ? 100.0 : chances.CalculateChanceForTwoPair(amountOfPlayers, allCards);
            double threeOfAKind = isThreeOfAKind ? 100.0 : chances.CalculateChanceForThreeOfAKind(amountOfPlayers, isPair, isTwoPair, allCards);
            double straight = isStraight ? 100.0 : chances.CalculateChanceForStraight(amountOfPlayers, allCards);
            double flush = isFlush ? 100.0 : chances.CalculateChanceForFlush(amountOfPlayers, allCards);
            double fullHouse = isFullHouse ? 100 : chances.CalculateChanceForFullHouse(amountOfPlayers, allCards);
            double fourOfAKind = isFourOfAKind ? 100 : chances.CalculateChanceForFourOfAKind(amountOfPlayers, isThreeOfAKind, allCards);
            double straightFlush = isStraightFlush
                ? 100
                : chances.CalculateChanceForStraightFlush(amountOfPlayers, allCards);
            double[] arrayChances = { pair, twoPair, threeOfAKind, straight, flush, fullHouse, fourOfAKind, straightFlush };
            return arrayChances;
        }

        private void CheckWinningHand(int amountOfPlayers)
        {
            allCards = Table.Pokercards.Where(c => c.FaceUp).ToList();
            foreach (var card in PokerCards)
            {
                allCards.Add(card);
            }
            winningHand = new Calculations.WinningHand(allCards);
            // currentHands toont welke winninghands we allemaal al hebben
            // bv pair & two pair & three-of-a-kind als we een full house hebben.
            currentHands = winningHand.checkWinningHand();
            // currentHand = toont ons de hoogste winningHand;
            CurrentHand = winningHand.GetWinningHands();
            WinningCards = winningHand.WinningCards;
        }
    }
}