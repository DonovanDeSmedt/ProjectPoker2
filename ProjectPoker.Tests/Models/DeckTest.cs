using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectPoker.Models;

namespace ProjectPoker.Tests.Models
{
    [TestClass]
    public class DeckTest
    {
        private Deck deck;
        [TestInitialize]
        public void Initialize()
        {
            deck = new Deck();
        }
        [TestMethod]
        public void TestNewDeck()
        {
            Assert.AreEqual(52, deck.Pokercards.Count);
        }
        [TestMethod]
        public void TestDrawCard()
        {
            Assert.AreEqual(typeof(PokerCard), deck.Draw().GetType());
        }

    }
}
