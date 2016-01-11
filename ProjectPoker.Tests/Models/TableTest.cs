using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectPoker.Models;

namespace ProjectPoker.Tests.Models
{
    [TestClass]
    public class TableTest
    {
        private Table table;
        [TestInitialize]
        public void Initialize()
        {
            table = new Table();
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void TestAddMoreThan5Cards()
        {
            for (int i = 0; i < 6; i++)
            {
                table.AddCard(new PokerCard(Suit.Clubs, FaceValue.Five));
            }
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void TestNegativeBet()
        {
            table.AddBet(-20);
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void TestBetOfZero()
        {
            table.AddBet(0);
        }
    }
}
