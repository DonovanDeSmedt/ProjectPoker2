using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Linq;
using ProjectPoker;
using ProjectPoker.Models;

namespace ProjectPoker.Models
{
    public class Poker
    {
        private int indexActivePlayer;
        private int startIndex;
        private bool preFlop = true;
        private int rollingIndex = 0;
        public Poker()
        {
            InitVariables();
            //AddNewPlayer("Player1");
            //AddNewBot("Pc1");
            //InitGame();
        }
        public Poker(IList<IPlayer> players)
        {
            InitVariables();
            List<IPlayer> deletePlayer = new List<IPlayer>();
            Players = players;
            foreach (var p in Players)
            {
                if (p.Money == 0)
                {
                    if (p.GetType() == typeof(ActivePlayer))
                    {
                        throw new InvalidOperationException("You lost all your money, please leave the table!");
                    }
                    deletePlayer.Add(p);
                }
                p.CurrentBet = 0;
                p.Table = Table;
            }
            deletePlayer.ForEach(p => Players.Remove(p));
            InitGame();
        }

        public Poker(Deck deck)
        {
            InitVariables();
            this.Deck = deck;
            //AddNewPlayer("Player1");
            //AddNewBot("Pc1");
            //InitGame();
        }

        private void InitVariables()
        {
            Table = new Table();
            Deck = new Deck();
            Players = new List<IPlayer>();
        }
        public void InitGame()
        {
            int dealerIndex;
            int bigBlindIndex;
            int smallBlindIndex;

            ListBots = new List<IPlayer>();
            for (int i = 0; i < 3; i++)
            {
                Table.AddCard(Deck.Draw());
            }
            // De verschillende bots in een lijst plaatsen
            foreach (var player in Players)
            {
                if (player.GetType() == typeof(Bot))
                {
                    ListBots.Add(player);
                }
            }
            // De Big -en Small Blind + de dealer bepalen
            // Kijken of er reeds een dealer is aangesteld,
            // Zoja: schuif de dealer 1 pos nr links, Zonee: Selecteer een random dealer
            if (Players.FirstOrDefault(p => p.IsDealer) == null)
            {
                dealerIndex = new Random().Next(0, Players.Count - 1);
            }
            // Indien er reeds een dealer was in het vorige spel schuift de dealer 1 pos nr links
            else
            {
                dealerIndex = Players.IndexOf(Players.FirstOrDefault(p => p.IsDealer));
                dealerIndex = dealerIndex + 1 == Players.Count ? 0 : dealerIndex + 1;
            }
            smallBlindIndex = dealerIndex + 1 == Players.Count ? 0 : dealerIndex + 1;
            if (Players.Count != 2)
            {
                bigBlindIndex = smallBlindIndex + 1 == Players.Count ? 0 : smallBlindIndex + 1;
            }
            else
            {
                bigBlindIndex = dealerIndex;
            }
            Players[dealerIndex].IsDealer = true;
            Players[smallBlindIndex].IsSmallBlind = true;
            Players[smallBlindIndex].Bet(5);
            Players[bigBlindIndex].IsBigBlind = true;
            Players[bigBlindIndex].Bet(10);
            startIndex = bigBlindIndex + 1 == Players.Count ? 0 : bigBlindIndex + 1;
            ActivePlayer = Players[startIndex];
            indexActivePlayer = startIndex + 1;
        }
        public virtual Deck Deck { get; private set; }

        public virtual Table Table { get; private set; }
        public IList<IPlayer> ListBots { get; private set; }
        public bool IsEndGame { get; private set; }
        public Hands WinningHand { get; set; }
        public IList<IPlayer> WinningPlayers { get; set; }

        public int SeatNrPlayer
        {
            get { return Players.FirstOrDefault(p => p.GetType() == typeof(ActivePlayer)).SeatNr; }
        }

        public void NewGame()
        {

        }
        public IPlayer ActivePlayer { get; private set; }


        public virtual IList<IPlayer> Players { get; private set; }

        public IPlayer getPlayer()
        {
            return Players.First(p => p.GetType() == typeof(ActivePlayer));
        }
        public void AddNewPlayer(string name)
        {
            // Als er reeds een actieve player in het spel zit kan er geen meer toegevoegd worden
            if (Players.Count(c => c.GetType() == typeof(ActivePlayer)) == 1)
            {
                throw new InvalidOperationException("There is already a acitvePlayer in the game");
            }
            // Kijken of er al een player met die naam voorkomt. Naam moet uniek zijn
            if (Players.Count(c => c.Name.Equals(name)) == 1)
            {
                throw new InvalidOperationException("Player with name " + name + " already exist");
            }
            if (name.Trim().Equals(""))
            {
                throw new InvalidOperationException("Name cannot be empty");
            }
            // Bij de initialisatie van een player krijgt deze 2 kaarten.
            IPlayer p = new ActivePlayer(name, Table, Players.Count + 1);
            p.AddCard(Deck.Draw());
            p.AddCard(Deck.Draw());
            p.TurnAllCards();
            Players.Add(p);
        }

        public void AddExistingPlayer(IPlayer player)
        {

        }
        public void AddNewBot(string name)
        {
            if (Players.Count(c => c.Name.Equals(name)) == 1)
            {
                throw new InvalidOperationException("Player with name " + name + " already exist");
            }
            if (name.Trim().Equals(""))
            {
                throw new InvalidOperationException("Name cannot be empty");
            }
            IPlayer p = new Bot(name, Table, Players.Count + 1);
            p.AddCard(Deck.Draw());
            p.AddCard(Deck.Draw());
            p.TurnAllCards();
            Players.Add(p);
        }

        public void AddExistingBot(IPlayer player)
        {

        }

        public void AddCardToTable()
        {
            Table.AddCard(Deck.Draw());
        }

        public void NextPlayer()
        {
            // Als er nog 1 of minder spelers overblijven is het spel gedaan
            if (Players.Count(p => p.Folded) >= Players.Count - 1)
            {
                EndGame();
            }
            // Eerst wordt er gekeken of de vorige player gefold heeft.
            // Zoja, dan wordt hij overgeslagen.

            while (Players[(indexActivePlayer) % Players.Count].Folded)
            {
                indexActivePlayer++;
                rollingIndex++;
            }
            rollingIndex++;



            // Als elke speler aan de beurt is geweest en elke niet gefolde speler heeft evenveel gebet
            int i = Players.Count(p => (p.CurrentBet == Table.CurrentBet && !p.Folded));
            int j = Players.Count(p => !p.Folded);
            int x = Players.Count(p => p.Folded);
            // normaal moet het rollingindex % players.count zijn dit levert problemen op
            if ((rollingIndex >= Players.Count) && (Players.Count(p => (p.CurrentBet == Table.CurrentBet && !p.Folded)) == Players.Count(p => !p.Folded)))
            {
                try
                {
                    Table.CurrentBet = 0;
                    // Na de eerste ronde worden de 3 org kaarten op de tafel omgedraaid               
                    if (preFlop)
                    {
                        Table.TurnAllCards();
                        preFlop = false;
                    }
                    else
                    {
                        PokerCard card = Deck.Draw();
                        card.TurnCard();
                        Table.AddCard(card);
                    }
                    foreach (var p in Players)
                    {
                        p.CurrentBet = 0;
                    }
                    indexActivePlayer = startIndex;
                    rollingIndex = 0;
                    //Wanneer er op een gefolde speler gespawnd wordt, zal deze overgeslagen worden.
                    while (Players[(indexActivePlayer) % Players.Count].Folded)
                    {
                        indexActivePlayer++;
                        rollingIndex++;
                    }
                    
                }
                catch (InvalidOperationException e)
                {
                    EndGame();
                }
            }
            ActivePlayer = Players[indexActivePlayer++ % Players.Count];

        }

        public void Deal()
        {
            Table.AddCard(Deck.Draw());
            Table.AddCard(Deck.Draw());
        }

        private void FormatPlayers()
        {
            var index = 0;
            foreach (var p in Players)
            {
                if (p.Money == 0)
                {
                    Players.Remove(p);
                }
                else
                {
                    p.SeatNr = index++;
                }
            }
        }
        public void EndGame()
        {
            // Deze methode zal zorgen dat de seatNr terug op elkaar volgen, moest een player het spel verlaten.
                
            SetWinner();
            HandMoneyToPlayers();
            FormatPlayers();
            IsEndGame = true;
            //PrepareForNextRound();
        }

        private void SetWinner()
        {
            WinningPlayers = new List<IPlayer>();
            WinningHand = Hands.HighestCard;
            foreach (var player in Players.Where(p => !p.Folded))
            {
                var x = Players.Where(p => !p.Folded);
                //De current hand setten bij de activePlayer
                if (player.GetType() == typeof(ActivePlayer))
                {
                    ActivePlayer p = player as ActivePlayer;
                    p.SetCurrentHand();
                }
                if (player.CurrentHand > WinningHand)
                {
                    WinningHand = player.CurrentHand;
                }
            }
            foreach (var player in Players.Where(p => p.CurrentHand == WinningHand && !p.Folded).ToList())
            {
                if (WinningPlayers.Count != 0)
                {
                    if (player.WinningCards.OrderBy(c => c.FaceValue).LastOrDefault().FaceValue >
                        WinningPlayers[0].WinningCards.OrderBy(c => c.FaceValue).LastOrDefault().FaceValue)
                    {
                        foreach (var p in WinningPlayers)
                        {
                            p.Winner = false;
                        }
                        WinningPlayers = new List<IPlayer>();
                        player.Winner = true;
                        WinningPlayers.Add(player);
                    }
                    else if (player.WinningCards.OrderBy(c => c.FaceValue).LastOrDefault().FaceValue ==
                             WinningPlayers[0].WinningCards.OrderBy(c => c.FaceValue).LastOrDefault().FaceValue)
                    {
                        player.Winner = true;
                        WinningPlayers.Add(player);
                    }
                }
                else
                {
                    player.Winner = true;
                    WinningPlayers.Add(player);
                }
            }

        }

        private void HandMoneyToPlayers()
        {
            int amountOfWinners = WinningPlayers.Count;
            foreach (var player in WinningPlayers)
            {
                player.AddMoney(Table.TableMoney/amountOfWinners);
            }
        }
        public void PrepareForNextRound()
        {
            foreach (var player in Players)
            {
                player.Folded = false;
                player.Winner = false;
                player.PrizeMoney = 0;
                player.Table = Table;
                player.PokerCards = new List<PokerCard>();
                player.AddCard(Deck.Draw());
                player.AddCard(Deck.Draw());
            }
        }

    }
}
