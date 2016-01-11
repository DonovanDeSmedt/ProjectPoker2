using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPoker;
using ProjectPoker.Models;
using ProjectPoker.Models.TestDecks;

namespace ProjectPoker.Tests.Models
{
    [TestClass()]
    public class PokerTests
    {
        private Poker poker;
        [TestInitialize]
        public void Initialize()
        {
            poker = new Poker();
            poker.AddNewPlayer("Player1");
            poker.AddNewBot("Pc1");
            poker.AddNewBot("Pc2");
            poker.AddNewBot("Pc3");
            poker.AddNewBot("Pc4");
            poker.InitGame();
        }
        [TestMethod()]
        public void TestNewPokerInitialize()
        {
            // Poker werd geinitialiseerd, 1 bot en 1 activeplayer werd gemaakt. 1ste player is activeplayer.
            Assert.AreEqual(2, poker.Players.Count);
            Assert.AreEqual(1, poker.Players.Count(c => c.GetType() == typeof(ActivePlayer)));
            Assert.AreEqual(1, poker.Players.Count(c => c.GetType() == typeof(Bot)));
            Assert.AreEqual(poker.Players[0], poker.ActivePlayer);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod()]
        public void Test2NewActivePlayer()
        {
            // Er kunnen geen twee actieve spelers aangemaakt worden.
            poker.AddNewPlayer("Player 2");
            poker.AddNewPlayer("Player 3");
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod()]
        public void TestNewBotPlayerWithoutName()
        {
            // Er kunnen geen twee actieve spelers aangemaakt worden.
            poker.AddNewBot("");
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod()]
        public void TestNewBotPlayerSameName()
        {
            // Er kunnen geen twee bot spelers aangemaakt worden met dezelfde naam
            poker.AddNewBot("Player 2");
            poker.AddNewBot("Player 2");
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod()]
        public void TestNewBotPlayerSameNameAsActivePlayer()
        {
            // Er kunnen geen twee spelers aangemaakt worden met dezelfde naam
            poker.AddNewBot("Player1");
        }
        [TestMethod()]
        public void TestNextPlayer()
        {
            poker.NextPlayer();
            Assert.AreEqual(poker.Players[1], poker.ActivePlayer);
            poker.NextPlayer();
            // aangezien er maar 2 spelers zijn gaat de beurt terug naar de eerste speler
            Assert.AreEqual(poker.Players[0], poker.ActivePlayer);
        }
        [TestMethod()]
        public void TestAddCardToTable()
        {          
            //Eerste speler is player1
            poker.NextPlayer(); // Player pc1
            Assert.AreEqual(poker.Players[1], poker.ActivePlayer);         
            Assert.AreEqual(3, poker.Table.Pokercards.Count());
            poker.NextPlayer(); // Player player1
            // 1ste ronde voorbij, de 3 kaarten die op tafel liggen worden zichtbaar
            poker.NextPlayer(); // Player pc1
            poker.NextPlayer(); // Player player1
            // Na de volgende ronde wordt er weer een kaart omgedraaidt
            Assert.AreEqual(4, poker.Table.Pokercards.Count());
            poker.NextPlayer();
            poker.NextPlayer();
            Assert.AreEqual(5, poker.Table.Pokercards.Count());
            poker.NextPlayer();
            poker.NextPlayer();
            // Hierna zitten we in endGame

        }
        [TestMethod()]
        public void TestPlayerFold()
        {
            // Player 1 is aan de beurt
            poker.ActivePlayer.Fold(); // Player 1 wordt nu een toeschouwer
            poker.NextPlayer();
            // Bot is nu aan de beurt
            poker.NextPlayer();
            // Normaal gezien zou Player 1 terug aan de beurt zijn,
            //maar aangezien deze gefold heeft is het terug aan Bot 1.
            Assert.AreEqual(poker.Players[1], poker.ActivePlayer);
            Assert.AreEqual(1, poker.Players.Count(p => p.Folded));
        }
        [TestMethod()]
        public void TestPokerWithExistingListPlayers()
        {
            IPlayer p1 = new ActivePlayer("Player1",poker.Table,1);
            IPlayer p2 = new Bot("Player2", poker.Table, 2);
            IPlayer p3 = new Bot("Player3", poker.Table, 3);
            IPlayer p4 = new Bot("Player4", poker.Table, 4);
            IList<IPlayer> players = new IPlayer[] {p1, p2, p3, p4}.ToList();
            poker = new Poker(players);
            Assert.AreEqual(4, poker.Players.Count);
        }

        [TestMethod()]
        public void TestPair()
        {
            poker = new ProjectPoker.Models.Poker(new TestDeck());
            poker.AddNewPlayer("Player1");
            poker.AddNewBot("Pc1");
            poker.InitGame();
            // Kaarten op de tafel worden omgedraaid
            poker.ActivePlayer.Check();
            poker.NextPlayer();
            poker.ActivePlayer.Check();       
            poker.NextPlayer();

            if (poker.ActivePlayer.GetType() == typeof (Bot))
            {
                Bot b = (Bot) poker.ActivePlayer;
                b.MakeDecision(poker.Players.Count);               
            }
        }

        [TestMethod]
        public void testBlind()
        {
            
        }
    }
}