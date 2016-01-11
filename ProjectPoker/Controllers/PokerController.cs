using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ProjectPoker.Models;
using ProjectPoker.ViewModels;

namespace ProjectPoker.Controllers
{
    public class PokerController : Controller
    {
        // GET: Poker
        private Poker poker;
        private PlayerViewModel viewModel;

        public ActionResult Index(Poker poker)
        {
            Session["state"] = "Home";
            return View();
        }
        [HttpPost]
        public ActionResult Index(PlayerViewModel viewModel)
        {
            Poker poker = new Poker();
            HttpContext.Session["Poker"] = poker;
            for (int i = 0; (i < viewModel.AmountOfBots && i < 5); i++)
            {
                poker.AddNewBot("Computer " + (i+1));
            }
            poker.AddNewPlayer(viewModel.Name);
            for (int i = 5; i < viewModel.AmountOfBots; i++)
            {
                poker.AddNewBot("Computer " + i);
            }
            poker.InitGame();
            Session["state"] = "Playing";
            return View("Create", poker);
        }

        public ActionResult Check(Poker poker)
        {
            if (poker.IsEndGame)
            {
                return View("EndGame", poker);
            }
            poker.ActivePlayer.Check();
            poker.NextPlayer();
            return PartialView("PartialGame", poker);
            //return View("Create", poker);
        }
        [HttpPost]
        public ActionResult Bet(int amount, Poker poker)
        {
            if (poker.IsEndGame)
            {
                return View("EndGame", poker);
            }
            try
            {
                poker.ActivePlayer.Bet(amount);
                poker.NextPlayer();
            }
            catch (InvalidOperationException e)
            {
                TempData["message"] = $"{e.Message}";
                return View("Create", poker);
            }
            return View("Create", poker);
        }
        public ActionResult Raise(int amount, Poker poker)
        {
            if (poker.IsEndGame)
            {
                return View("EndGame", poker);
            }
            try
            {
                poker.ActivePlayer.Bet(amount);
                poker.NextPlayer();
            }
            catch (InvalidOperationException e)
            {
                TempData["money"] = $"{e.Message}";
                return View("Create", poker);
            }
            return View("Create", poker);
        }

        public ActionResult Fold(Poker poker)
        {
            if (poker.IsEndGame)
            {
                return View("EndGame", poker);
            }
            poker.ActivePlayer.Fold();
            poker.NextPlayer();
            TempData["PokerController"] = this;

            return View("Create", poker);
        }

        public ActionResult BotDecision(Poker poker)
        {
            if (poker.IsEndGame)
            {
                return View("EndGame", poker);
            }
            if (!poker.ActivePlayer.Folded)
            {
                Bot b = poker.ActivePlayer as Bot;
                b.MakeDecision(poker.Players.Count);
            }
            poker.NextPlayer();
            return PartialView("PartialGame", poker);
        }

        public ActionResult NextRound(Poker poker)
        {
            IList<IPlayer> players = poker.Players;
            Poker newPoker = new Poker(players);
            newPoker.PrepareForNextRound();
            
            HttpContext.Session["Poker"] = newPoker;
            return View("Create", newPoker);
        }

        public Boolean GetIndexActivePlayer(Poker poker)
        {
            return poker.ActivePlayer.SeatNr != poker.SeatNrPlayer;
        }

        public string GetMinimumBet(Poker poker)
        {
            var data = poker.Table.CurrentBet.ToString() +":"+ poker.ActivePlayer.Money.ToString();
            return data;
        }
    }
}