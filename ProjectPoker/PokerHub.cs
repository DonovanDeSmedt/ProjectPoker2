using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ProjectPoker.Models;
using WebGrease.Css.Extensions;

namespace ProjectPoker
{
    [HubName("pokerBox")]
    public class PokerHub : Hub
    {
        private static List<ActivePlayer> activePlayers = new List<ActivePlayer>();
        private static List<IPlayer> pokerPlayers = new List<IPlayer>();
        private object _syncRoot = new Object();

        public void RegisterPlayer(string data, int money = 1000)
        {
            lock (_syncRoot)
            {
                if (activePlayers.FirstOrDefault(p => p.Name.Equals(data)) != null)
                {
                    Clients.All.otherName(activePlayers);
                }
                var activePlayer = activePlayers.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
                bool exist = activePlayers.Any(p => p.Name.Equals(data));
                if (!activePlayers.Any() || (MultiplayerGame.ServerPlayer != null && MultiplayerGame.ServerPlayer.Equals(data)))
                {
                    Clients.Client(Context.ConnectionId).showButton();
                }
                if (activePlayer == null && !exist)
                {
                    activePlayer = new ActivePlayer
                    {
                        Name = data,
                        PokerCards = new List<PokerCard>(),
                        ConnectionId = Context.ConnectionId,
                        Money = money,
                        Folded = false,
                        JoinDateTime = DateTime.Now.ToString("HH:mm:ss"),
                        LookingForOpponent = true
                    };
                    activePlayers.Add(activePlayer);
                }
                //Wanneer er iemand in de lobby komt wordt deze geüpdatet
                Clients.All.registerComplete(activePlayers);
            }
        }

        public void RemoveMultiplayer()
        {
            if (MultiplayerGame.DeleteMultiplayer)
            {
                activePlayers = new List<ActivePlayer>();
                pokerPlayers = new List<IPlayer>();
                MultiplayerGame.DeleteMultiplayer = false;
            }
        }
        public void FindOpponent()
        {
            IPlayer Opponent = null;
            MultiplayerGame.DeleteMultiplayer = true;
            var player = activePlayers.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (player == null) return;

            //Look for a random opponent if there is more than one looking for a game.
            List<ActivePlayer> opponents = activePlayers.Where(c => c.ConnectionId != Context.ConnectionId && c.LookingForOpponent)
                .OrderBy(c => Guid.NewGuid()).ToList();
            // If no opponents found
            if (!opponents.Any())
            {
                Clients.Client(Context.ConnectionId).noOpponents();
                return;
            }
            //When the opponent is found
            player.LookingForOpponent = false;
            opponents.ForEach(o => o.LookingForOpponent = false);
            if (MultiplayerGame.PDictionary == null)
                MultiplayerGame.PDictionary = new Dictionary<string, Poker>();

            //De poker die als server fungeert wordt geinitialiseerd
            Poker PokerPlayer = CreateFirstPoker(player.Name);
            PokerPlayer.IsServer = true;
            MultiplayerGame.PDictionary.Add(player.Name, PokerPlayer);

            //Voor elke opponent wordt er een poker gemaakt
            Poker PokerOpponent;
            opponents.ForEach(o =>
            {
                PokerOpponent = CreatePoker(o.Name);
                PokerOpponent.Table = PokerPlayer.Table;
                PokerOpponent.ActivePlayer = PokerPlayer.ActivePlayer;

                //De bigBlind van de server gelijk stellen aan de bigBlind van de andere spelers
                IPlayer bigblind = PokerPlayer.Players.FirstOrDefault(p => p.IsBigBlind);
                Opponent = PokerOpponent.Players.FirstOrDefault(p => p.Name.Equals(bigblind.Name));
                Opponent = bigblind;


                //De smallblind van de server gelijk stellen aan de smallblind van de andere spelers
                IPlayer smallBlind = PokerPlayer.Players.FirstOrDefault(p => p.IsSmallBlind);
                Opponent = PokerOpponent.Players.FirstOrDefault(p => p.Name.Equals(smallBlind));
                Opponent = smallBlind;

                //De dealer van de server gelijk stellen aan de dealer van de andere spelers
                IPlayer dealer = PokerPlayer.Players.FirstOrDefault(p => p.IsDealer);
                Opponent = PokerOpponent.Players.FirstOrDefault(p => p.Name.Equals(dealer));
                Opponent = dealer;

                PokerOpponent.IndexActivePlayer = PokerPlayer.IndexActivePlayer;

                PokerOpponent.StartIndex = PokerPlayer.StartIndex;

                PokerOpponent.RollingIndex = PokerPlayer.RollingIndex;

                PokerOpponent.PreFlop = false;
                PokerOpponent.IsServer = false;
                MultiplayerGame.PDictionary.Add(o.Name, PokerOpponent);

            });
            activePlayers.ForEach(p =>
            {
                Clients.Client(p.ConnectionId).startGame(p.Name);
            });
        }

        public void InitGame(string name)
        {

            var opponent = activePlayers.FirstOrDefault(c => c.Name != name);
            var pname = activePlayers.FirstOrDefault(p => p.ConnectionId.Equals(Context.ConnectionId)).Name;

            Poker pokerGame;
            MultiplayerGame.PDictionary.TryGetValue(pname, out pokerGame);
            Clients.Client(Context.ConnectionId).startGame(pname);
        }
        public Poker CreatePoker(string playerName)
        {
            Poker poker = new Poker();
            poker.Players = pokerPlayers;
            poker.Host = poker.Players.FirstOrDefault(p => p.Name.Equals(playerName));
            poker.Host.Opponents = poker.Players.Where(p => !p.Name.Equals(playerName)).ToList();
            return poker;
        }

        public Poker CreateFirstPoker(string playerName)
        {
            Poker poker = new Poker();
            activePlayers.ForEach(p => poker.AddExistingPlayer(p));
            poker.InitGame();
            poker.Host = poker.Players.FirstOrDefault(p => p.Name.Equals(playerName));
            poker.Host.Opponents = poker.Players.Where(p => !p.Name.Equals(playerName)).ToList();
            //Naam van de serverplayer setten zodat de controller hier ook aan kan
            MultiplayerGame.ServerPlayer = playerName;
            pokerPlayers.AddRange(poker.Players.ToList());
            return poker;

        }

        public bool IsActive(string name)
        {
            Poker poker;
            MultiplayerGame.PDictionary.TryGetValue(name, out poker);
            return poker.ActivePlayer.Name.Equals(name);
        }
        public void Check(string playerName)
        {
            // De hub krijgt een request binnen ven een bepaalde speler voor een bepaalde actie (hier check)
            // De hub zal elke speler verwittigen, welke op hun beurt de juiste vieuws opvragen.
            // Eerst wordt er gekeken of de player die request stuurde effectief de speler aan de beurt is.
            Poker poker;
            MultiplayerGame.PDictionary.TryGetValue(playerName, out poker);
            // De poker van de server opvragen om te kijken of het daar endGame is.
            Poker serverPoker;
            MultiplayerGame.PDictionary.TryGetValue(MultiplayerGame.ServerPlayer, out serverPoker);
            if (IsActive(playerName))
            {
                //Check if the server isEndgame, zoja, endGame van deze poker op true zetten.
                if (poker.IsEndGame || serverPoker.IsEndGame)
                {
                    poker.IsEndGame = true;

                    activePlayers.ForEach(p =>
                    {
                        Clients.Client(p.ConnectionId).onEndGame(p.Name);
                    });
                    return;
                }
                MultiplayerGame.AccesLock = true;
                var player = activePlayers.FirstOrDefault(p => p.Name.Equals(playerName));
                Clients.Client(player.ConnectionId).onCheck(player.Name);

                activePlayers.Where(p => !p.Name.Equals(playerName)).ForEach(p =>
                {
                    Clients.Client(p.ConnectionId).onCheck(p.Name);
                });

            }
        }
        public void Bet(string playerName, int amount)
        {
            Poker poker;
            MultiplayerGame.PDictionary.TryGetValue(playerName, out poker);
            Poker serverPoker;
            MultiplayerGame.PDictionary.TryGetValue(MultiplayerGame.ServerPlayer, out serverPoker);
            if (IsActive(playerName))
            {
                //Check if the server isEndgame, zoja, endGame van deze poker op true zetten.
                if (poker.IsEndGame || serverPoker.IsEndGame)
                {
                    poker.IsEndGame = true;
                    activePlayers.ForEach(p =>
                    {
                        Clients.Client(p.ConnectionId).onEndGame(p.Name);
                    });
                    return;
                }
                MultiplayerGame.AccesLock = true;
                var player = activePlayers.FirstOrDefault(p => p.Name.Equals(playerName));
                Clients.Client(player.ConnectionId).onBet(player.Name, amount);

                activePlayers.Where(p => !p.Name.Equals(playerName)).ForEach(p =>
                {
                    Clients.Client(p.ConnectionId).onBet(p.Name, amount);
                });
            }
        }
        public void Fold(string playerName)
        {
            Poker poker;
            MultiplayerGame.PDictionary.TryGetValue(playerName, out poker);
            Poker serverPoker;
            MultiplayerGame.PDictionary.TryGetValue(MultiplayerGame.ServerPlayer, out serverPoker);
            if (IsActive(playerName))
            {
                //Check if the server isEndgame, zoja, endGame van deze poker op true zetten.
                if (poker.IsEndGame || serverPoker.IsEndGame)
                {
                    poker.IsEndGame = true;
                    activePlayers.ForEach(p =>
                    {
                        Clients.Client(p.ConnectionId).onEndGame(p.Name);
                    });
                    return;
                }
                MultiplayerGame.AccesLock = true;
                var player = activePlayers.FirstOrDefault(p => p.Name.Equals(playerName));
                Clients.Client(player.ConnectionId).onFold(player.Name);

                activePlayers.Where(p => !p.Name.Equals(playerName)).ForEach(p =>
                {
                    Clients.Client(p.ConnectionId).onFold(p.Name);
                });
            }
        }

        public void NextRound(string playerName)
        {
            MultiplayerGame.AccesLock = true;
            activePlayers.ForEach(p => Clients.Client(p.ConnectionId).NextRound(p.Name));
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            MultiplayerGame.PDictionary = new Dictionary<string, Poker>();
            this.Clients.All.removeStorage();
            return base.OnDisconnected(stopCalled);
        }
    }
}