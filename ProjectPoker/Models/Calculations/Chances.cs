using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace ProjectPoker.Models.Calculations
{
    public class Chances
    {
        private IList<PokerCard> allCards;

        public Chances(IList<PokerCard> cards)
        {
            allCards = cards;
        }
        public double CalculateChanceForPair(int amount, IList<PokerCard> cards)
        {
            allCards = cards;
            int noemer1 = (52 - allCards.Count - ((amount - 1) * 2));
            int noemer2 = (52 - allCards.Count - 1 - ((amount - 1) * 2));
            switch (allCards.Count)
            {

                //Wanneer er nog 2 kaarten onbekend zijn
                case 5:
                    switch (amount)
                    {
                        case 2: return (6.0 / noemer1) * 100 + (6.0 / noemer2) * 100;
                        case 3: case 4: return (5.0 / noemer1) * 100 + (5.0 / noemer2) * 100;
                        default: return (4.0 / noemer1) * 100 + (4.0 / noemer2) * 100;
                    }
                //Wanneer er nog 1 kaart onbekend is
                case 6:
                    switch (amount)
                    {
                        case 2: return (6.0 / noemer2) * 100;
                        case 3: case 4: return (5.0 / noemer2) * 100;
                        default: return (4.0 / noemer2) * 100;
                    }
            }
            return 0;
        }
        public double CalculateChanceForTwoPair(int amount, IList<PokerCard> cards)
        {
            allCards = cards;
            int noemer1 = (52 - allCards.Count - ((amount - 1) * 2));
            int noemer2 = (52 - allCards.Count - 1 - ((amount - 1) * 2));
            switch (allCards.Count)
            {
                case 5:
                    switch (amount)
                    {
                        case 2: return (3.0 / noemer1) * 100 + (3.0 / noemer2) * 100;
                        case 3: case 4: return (2.0 / noemer1) * 100 + (2.0 / noemer2) * 100;
                        default: return (1.0 / noemer1) * 100 + (1.0 / noemer2) * 100;
                    }
                case 6:
                    switch (amount)
                    {
                        case 2: return (3.0 / noemer2) * 100;
                        case 3: case 4: return (2.0 / noemer2) * 100;
                        default: return (1.0 / noemer2) * 100;
                    }
            }
            return 0;
        }
        public double CalculateChanceForThreeOfAKind(int amount, bool pair, bool twoPair, IList<PokerCard> cards)
        {
            allCards = cards;
            int noemer1 = (52 - allCards.Count - ((amount - 1) * 2));
            int noemer2 = (52 - allCards.Count - 1 - ((amount - 1) * 2));
            if (pair)
            {
                switch (allCards.Count)
                {
                    case 5:
                        switch (amount)
                        {
                            case 2: return (2.0 / noemer1) * 100 + (2.0 / noemer2) * 100;
                            case 3: case 4: return (2.0 / noemer1) * 100 + (2.0 / noemer2) * 100;
                            default: return (1.0 / noemer1) * 100 + (1 / noemer2) * 100;
                        }
                    case 6:
                        switch (amount)
                        {
                            case 2: return (2.0 / noemer2) * 100;
                            case 3: case 4: return (2.0 / noemer2) * 100;
                            default: return (1.0 / noemer2) * 100;
                        }
                }
            }
            else if (twoPair)
            {
                switch (allCards.Count)
                {
                    case 5:
                        switch (amount)
                        {
                            case 2: return (4.0 / noemer1) * 100 + (4.0 / noemer2) * 100;
                            case 3: case 4: return (3.0 / noemer1) * 100 + (3.0 / noemer2) * 100;
                            default: return (1.0 / noemer1) * 100 + (1.0 / noemer2) * 100;
                        }
                    case 6:
                        switch (amount)
                        {
                            case 2: return (4.0 / noemer2) * 100;
                            case 3: case 4: return (3.0 / noemer2) * 100;
                            default: return (1.0 / noemer2) * 100;
                        }
                }
            }
            return 0.0;
        }
        public IList<PokerCard>[] CalculateOutsForStraight(int amountOfUnknwonCards, IList<PokerCard> cards)
        {
            allCards = cards;
            int value = -1;
            int streak = 1;
            int index = 0;
            int indexFirstCardOfStreak = 0;
            IList<PokerCard>[] secors = new IList<PokerCard>[99];
            IList<PokerCard> missingCards = new List<PokerCard>();
            IList<PokerCard> allCardsCopy = allCards.OrderBy(c => (int)c.FaceValue).ToList();

            while (true)
            {
                foreach (var card in allCardsCopy)
                {
                    if (streak != 5)
                    {
                        // Bij een nieuwe kaart wordt de value = kaartValue oF
                        // Wanneer er 2 gelijke kaarten zijn gebeurt er niets met de streak.
                        if (value == -1 || value == (int)card.FaceValue)
                        {
                            indexFirstCardOfStreak = allCardsCopy.IndexOf(card);
                            value = (int)card.FaceValue;
                        }
                        // Wanneer de huidige kaart volgt op de vorige -> streak + 1
                        else if ((value + 1) == (int)card.FaceValue)
                        {
                            streak++;
                        }
                        else
                        {
                            // Zolang de vorige kaart != de huidige kaart gaan we 
                            // er kunstmatig kaarten tussensteken totdat de huidige volgt op de vorige.
                            // Later wordt op basis hiervan de kans berekend.
                            while (value + 1 != (int)card.FaceValue && streak != 5)
                            {
                                missingCards.Add(new PokerCard(Suit.Clubs, ((FaceValue)value + 1)));
                                value++;
                                streak++;
                            }
                            streak++;
                        }
                    }
                    // als we een streak van 5 hebben (Straat) gaan we over tot de volgende kaart.    
                    // hier mag GEEN ELSE voor gebruikt worden want het kan nodig zijn om eerst 
                    // in de eerste if te gaan en dan in de tweede!!   
                    if (streak >= 5)
                    {
                        secors[index++] = missingCards.ToList();
                        missingCards = new List<PokerCard>();
                        for (int i = 0; i < indexFirstCardOfStreak; i++)
                        {
                            allCardsCopy.RemoveAt(0);
                        }
                        value = -1;
                        break;
                    }
                    value = (int)card.FaceValue;
                }

                // Wanneer de streak < 5 is en alle kaarten zijn van de foreach zijn overlopen,
                //  moeten er nog kaarten buiten de range worden toegevoegt.Dit lukt niet in de foreach
                if (allCardsCopy.Count >= 2 && streak < 5)
                {
                    while (streak != 5)
                    {
                        if (value + 1 > 14)
                        {
                            return secors.Where(s => s != null && s.Count() <= amountOfUnknwonCards).ToArray();
                        }
                        missingCards.Add(new PokerCard(Suit.Clubs, ((FaceValue)value + 1)));
                        value++;
                        streak++;
                    }
                    secors[index++] = missingCards.ToList();
                    for (int i = 0; i < indexFirstCardOfStreak; i++)
                    {
                        allCardsCopy.RemoveAt(0);
                    }
                }
                // Wanneer er minder dan 5 kaarten overschieten stoppen we
                // ->een straat bestaat uit 5 kaarten.
                if (allCardsCopy.Count <= 2)
                {
                    break;
                }

                // We nemen steeds 1 kaart uit de lijst
                streak = 1;
                allCardsCopy.RemoveAt(0);
            }
            return secors.Where(s => s != null && s.Count() <= amountOfUnknwonCards).ToArray();
        }
        public double CalculateChanceForStraight(int amount, IList<PokerCard> cards)
        {
            allCards = cards;
            IList<PokerCard>[] sectors;
            double[] chances = null;
            double chance = 1.0;
            double totalChance = 0;
            int index = 0;
            // Kijken hoeveel kaarten er al op de tafel+hand liggen
            switch (allCards.Count)
            {
                case 5:
                    {
                        sectors = CalculateOutsForStraight(2, allCards);
                        if (sectors == null)
                        {
                            return 0.0;
                        }
                        chances = new double[sectors.Length];
                        foreach (var sector in sectors)
                        {
                            for (int i = 1; i <= sector.Count; i++)
                            {
                                switch (amount)
                                {
                                    case 2:
                                        chance *= (4.0 * i) / (52 - 5 - (i - 1) - (amount * 2));
                                        // i-1 ->eerst /45 en dan /44 want na de eerste keer is er een kaart minder in het spel
                                        break;
                                    case 3:
                                    case 4:
                                        chance *= (3.5 * i) / (52 - 5 - (i - 1) - (amount * 2));
                                        break;
                                    default:
                                        chance *= (3.5 * i) / (52 - 5 - (i - 1) - (amount * 2));
                                        break;
                                }
                            }
                            chances[index++] = chance;
                            chance = 1.0;
                        }
                    }
                    break;
                case 6:
                    {
                        sectors = CalculateOutsForStraight(1, allCards);
                        if (sectors == null)
                        {
                            return 0.0;
                        }
                        chances = new double[sectors.Length];
                        foreach (var sector in sectors)
                        {
                            for (int i = 1; i <= sector.Count; i++)
                            {
                                switch (amount)
                                {
                                    case 2:
                                        chance *= (4.0) / (52 - 6 - (amount * 2));
                                        break;
                                    case 3:
                                    case 4:
                                        chance *= (3.5) / (52 - 6 - (amount * 2));
                                        break;
                                    default:
                                        chance *= (2.5) / (52 - 6 - (amount * 2));
                                        break;
                                }
                            }
                            chances[index++] = chance;
                            chance = 1.0;
                        }
                    }
                    break;
                default:
                    return 0.0;
            }
            // kansen optellen.
            foreach (var c in chances)
            {
                totalChance += (c * 100);
            }
            return totalChance;
        }

        private IList<PokerCard> CalculateOutForFlush(IList<PokerCard> cards, int missingCards)
        {
            allCards = cards;

            Suit suit;
            IList<PokerCard> suitCards = new List<PokerCard>();

            var groups = allCards.GroupBy(c => c.Suit);
            foreach (var group in groups)
            {
                if (group.ToList().Count() >= (5 - missingCards))
                {
                    suit = group.Key;
                    suitCards = group.ToList();
                }

            }
            return suitCards;
        }
        public double CalculateChanceForFlush(int amount, IList<PokerCard> cards)
        {
            allCards = cards;
            IList<PokerCard> suitCards;
            int noemer1 = (52 - allCards.Count - ((amount - 1) * 2));
            int noemer2 = (52 - allCards.Count - 1 - ((amount - 1) * 2));
            switch (cards.Count())
            {
                case 5:
                    {
                        // suitcards geeft ons de hoogste lijst van kaarten met dezelfde suit
                        suitCards = CalculateOutForFlush(cards, 2);
                        if (suitCards.Count == 4)
                        {
                            switch (amount)
                            {
                                case 2:
                                    return ((9.0/noemer1)+(9.0/noemer2))*100;
                                case 3:
                                case 4:
                                    return ((7.0/noemer1)+(7.0/noemer2))*100;
                                default:
                                    return ((5.0/noemer1)+(5.0/noemer2))*100;
                            }
                        }
                        return 0.0;
                    }
                case 6:
                    {
                        suitCards = CalculateOutForFlush(cards, 1);
                        if (suitCards.Count == 4)
                        {
                            switch (amount)
                            {
                                case 2:
                                    return ((8.0/noemer2)*100);
                                case 3:
                                case 4:
                                    return ((7.0/noemer2)*100);
                                default:
                                    return ((6.0/noemer2)*100);
                            }
                        }
                        return 0.0;
                    }
            }
            return 0.0;
        }

        public double CalculateChanceForFullHouse(int amount, IList<PokerCard> cards)
        {
            allCards = cards;
            int noemer1 = (52 - allCards.Count - ((amount - 1) * 2));
            int noemer2 = (52 - allCards.Count - 1 - ((amount - 1) * 2));
            // Kijken of er een Three-of-a-kind is.
            if (allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(2)).ToList().Count > 0)
            {
                return CalculateChanceForTwoPair(amount, cards);
            }
            // Indien er 2 pairs op tafel liggen.
            else if (allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList().Count > 1)
            {
                switch (allCards.Count)
                {
                    case 5:
                        switch (amount)
                        {
                            case 2: case 3: case 4: return (((3.0 / noemer1) + (3.0 / noemer2)) * 100) * 2;
                            default: return (((2.0 / noemer1) * (1.0 / noemer2)) * 100) * 2;
                        }
                    case 6:
                        return 0.0;
                }          
            }
            // Indien er 1 pair op tafel ligt
            else if (allCards.GroupBy(c => c.FaceValue).SelectMany(g => g.Skip(1)).ToList().Count > 0)
            {
                switch (allCards.Count)
                {
                    case 5:
                        switch (amount)
                        {
                            case 2: case 3: case 4: return (((3.0 / noemer1) * (2.0 / noemer2)) * 100) * 2;
                            default: return (((2.0 / noemer1) * (1.0 / noemer2)) * 100) * 2;
                        }
                    case 6:
                        return 0.0;
                }
            }
            // Indien er noch een Pair noch een Three-of-a-kind is.
            return 0.0;
        }

        public double CalculateChanceForFourOfAKind(int amount, bool isThreeOfAKind, IList<PokerCard> cards)
        {
            allCards = cards;
            int noemer1 = (52 - allCards.Count - ((amount - 1) * 2));
            int noemer2 = (52 - allCards.Count - 1 - ((amount - 1) * 2));
            if (isThreeOfAKind)
            {
                switch (allCards.Count)
                {
                    case 5:
                        switch (amount)
                        {
                            case 2:
                                return ((1.0 / noemer1) + (1.0 / noemer2)) * 100;
                            case 3:
                            case 4:
                                return (1.0 / noemer2) * 100;
                            default:
                                return ((1.0 / noemer1) * 100)*0.5;
                        }
                    case 6:
                        switch (amount)
                        {
                            case 2:
                                return ((1.0/noemer2)*100);
                            case 3: case 4: return ((1.0 / noemer2) * 100)*0.75;
                            default: return ((1.0 / noemer2) * 100) * 0.5;
                        }
                }
            }
            return 0.0;
        }

        public IList<PokerCard>[] CalculateOutsForStraightsFlush(int amountOfUnknwonCards, IList<PokerCard> cards)
        {
            allCards = cards;
            int value = -1;
            int suit = -1;
            int streak = 1;
            int index = 0;
            int indexFirstCardOfStreak = 0;
            IList<PokerCard>[] secors = new IList<PokerCard>[99];
            IList<PokerCard> missingCards = new List<PokerCard>();
            IList<PokerCard> allCardsCopy = allCards.OrderBy(c => (int)c.FaceValue).ToList();

            //Kijken bij welke suit een straightFlush gehaald kan worden.
            var groups = allCards.GroupBy(c => c.Suit);
            foreach (var group in groups)
            {
                if (group.ToList().Count() >= (5 - amountOfUnknwonCards))
                {
                    suit = (int) group.Key;
                }
            }
            if (suit == -1)
            {
                return null;
            }
            while (true)
            {
                foreach (var card in allCardsCopy)
                {
                    if (streak != 5)
                    {
                        // Bij een nieuwe kaart wordt de value = kaartValue oF
                        // Wanneer er 2 gelijke kaarten zijn gebeurt er niets met de streak.
                        if (value == -1 || value == (int)card.FaceValue)
                        {
                            if (suit == (int) card.Suit)
                            {
                                indexFirstCardOfStreak = allCardsCopy.IndexOf(card);
                                value = (int) card.FaceValue;
                            }
                        }
                        // Wanneer de huidige kaart volgt op de vorige -> streak + 1
                        else if ((value + 1) == (int)card.FaceValue && suit == (int) card.Suit)
                        {
                            streak++;
                        }
                        else if(suit == (int)card.Suit)
                        {
                            // Zolang de vorige kaart != de huidige kaart gaan we 
                            // er kunstmatig kaarten tussensteken totdat de huidige volgt op de vorige.
                            // Later wordt op basis hiervan de kans berekend.
                            while (value + 1 != (int)card.FaceValue && streak != 5)
                            {
                                missingCards.Add(new PokerCard(Suit.Clubs, ((FaceValue)value + 1)));
                                value++;
                                streak++;
                            }
                            streak++;
                        }
                    }
                    // als we een streak van 5 hebben (Straat) gaan we over tot de volgende kaart.    
                    // hier mag GEEN ELSE voor gebruikt worden want het kan nodig zijn om eerst 
                    // in de eerste if te gaan en dan in de tweede!!   
                    if (streak >= 5)
                    {
                        secors[index++] = missingCards.ToList();
                        missingCards = new List<PokerCard>();
                        for (int i = 0; i < indexFirstCardOfStreak; i++)
                        {
                            allCardsCopy.RemoveAt(0);
                        }
                        value = -1;
                        break;
                    }
                    if(suit == (int)card.Suit)
                        value = (int)card.FaceValue;
                }

                // Wanneer de streak < 5 is en alle kaarten zijn van de foreach zijn overlopen,
                //  moeten er nog kaarten buiten de range worden toegevoegt.Dit lukt niet in de foreach
                if (allCardsCopy.Count >= 2 && streak < 5)
                {
                    while (streak != 5)
                    {
                        if (value + 1 > 14)
                        {
                            return secors.Where(s => s != null && s.Count() <= amountOfUnknwonCards).ToArray();
                        }
                        missingCards.Add(new PokerCard(Suit.Clubs, ((FaceValue)value + 1)));                       
                        value++;
                        streak++;
                    }
                    secors[index++] = missingCards.ToList();
                    for (int i = 0; i < indexFirstCardOfStreak; i++)
                    {
                        allCardsCopy.RemoveAt(0);
                    }
                }
                // Wanneer er minder dan 5 kaarten overschieten stoppen we
                // ->een straat bestaat uit 5 kaarten.
                if (allCardsCopy.Count <= 2)
                {
                    break;
                }

                // We nemen steeds 1 kaart uit de lijst
                streak = 1;
                allCardsCopy.RemoveAt(0);
            }
            return secors.Where(s => s != null && s.Count() <= amountOfUnknwonCards).ToArray();
        } 
        public double CalculateChanceForStraightFlush(int amount, IList<PokerCard> cards)
        {
            allCards = cards;
            IList<PokerCard>[] sectors;
            double[] chances = null;
            double chance = 1.0;
            double totalChance = 0;
            int index = 0;
            // Kijken hoeveel kaarten er al op de tafel+hand liggen
            switch (allCards.Count)
            {
                case 5:
                    {
                        sectors = CalculateOutsForStraightsFlush(2, allCards);
                        if (sectors == null)
                        {
                            return 0.0;
                        }
                        chances = new double[sectors.Length];
                        foreach (var sector in sectors)
                        {
                            for (int i = 1; i <= sector.Count; i++)
                            {
                                chance *= 1.0 / (52 - 5 - (i - 1) - (amount * 2));
                            }
                            chances[index++] = chance;
                            chance = 1.0;
                        }
                    }
                    break;
                case 6:
                    {
                        sectors = CalculateOutsForStraightsFlush(1, allCards);
                        if (sectors == null)
                        {
                            return 0.0;
                        }
                        chances = new double[sectors.Length];
                        foreach (var sector in sectors)
                        {
                            for (int i = 1; i <= sector.Count; i++)
                            {
                                switch (amount)
                                {
                                    case 2:
                                        chance *= (4.0) / (52 - 6 - (amount * 2));
                                        break;
                                    case 3:
                                    case 4:
                                        chance *= (3.5) / (52 - 6 - (amount * 2));
                                        break;
                                    default:
                                        chance *= (2.5) / (52 - 6 - (amount * 2));
                                        break;
                                }
                            }
                            chances[index++] = chance;
                            chance = 1.0;
                        }
                    }
                    break;
                default:
                    return 0.0;
            }
            // kansen optellen.
            foreach (var c in chances)
            {
                totalChance += (c * 100);
            }
            return totalChance;
        }
    }
}