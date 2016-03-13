using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using ProjectPoker.Models;
using ProjectPoker.ViewModels;

namespace ProjectPoker.Controllers
{
    public class PokerController : Controller
    {
        public ActionResult Index(Poker poker)
        {
            Session["state"] = "Home";
            MultiplayerGame.PokerController = this;
            return View("IndexSingleplayer");
        }
        [HttpPost]
        public ActionResult Index(PlayerViewModel viewModel)
        {
            Poker poker = new Poker();
            HttpContext.Session["Poker"] = poker;
            for (int i = 0; (i < viewModel.AmountOfBots && i < 5); i++)
            {
                poker.AddNewBot("Computer " + (i + 1));
            }
            poker.AddNewPlayer(viewModel.Name);
            for (int i = 5; i < viewModel.AmountOfBots; i++)
            {
                poker.AddNewBot("Computer " + (i + 1));
            }
            poker.InitGame();
            Session["state"] = "Playing";
            return View("Create", poker);
        }

        public ActionResult IndexGeneral(Poker poker)
        {
            Session["state"] = "Home";
            MultiplayerGame.PokerController = this;
            return View("Index");
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
                TempData["message"] = $"{e.Message}";
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
            Poker newPoker = null;
            try
            {
                IList<IPlayer> players = poker.Players;
                newPoker = new Poker(players);
                newPoker.PrepareForNextRound();
            }
            catch (Exception e)
            {
                TempData["message"] = $"{e.Message}";
            }


            HttpContext.Session["Poker"] = newPoker;
            return View("Create", newPoker);
        }
        public ActionResult IndexMultiplayer()
        {
            Session["state"] = "Home";
            MultiplayerGame.PokerController = this;
            MultiplayerGame.Names = new List<string>();
            return View("IndexMultiplayer");
        }
        [HttpPost]
        [ActionName("IndexMultiplayer")]
        public ActionResult IndexMultiplayerPost(Poker poker,PlayerViewModel playerViewModel)
        {
            if (playerViewModel.Name == null || playerViewModel.Name.IsEmpty() || MultiplayerGame.Names.Contains(playerViewModel.Name))
            {
                Session["state"] = "Home";
                TempData["message"] = String.Format("There is already a player with name: {0}. Choose another name!", playerViewModel.Name);
                return View("IndexMultiplayer");
            }      
            poker.Host = new ActivePlayer {Name = playerViewModel.Name};
            MultiplayerGame.Names.Add(playerViewModel.Name);
            poker.Host.Money = 1000;
            Session["state"] = "Playing";
            return View("CreateMultiplayer");
        }
        public ActionResult NewMultiplayerGame(string name)
        {
            Poker p;
            MultiplayerGame.PDictionary.TryGetValue(name, out p);

            HttpContext.Session["Poker"] = p;
            return PartialView("Multiplayer", p);
        }
        public void SetMultiplayerGame(PokerViewModel pvm)
        {
            Poker poker = new Poker(pvm.Players);
            MultiplayerGame.Poker = poker;
        }
        public ActionResult CheckMultiplayer(string name)
        {
            Poker poker;
            MultiplayerGame.PDictionary.TryGetValue(name, out poker);
            if (poker.IsEndGame)
            {
                return PartialView("EndGameMultiplayer", poker);
            }
            if (poker.IsServer)
            {
                poker.ActivePlayer.Check();
                poker.NextPlayer();
                MultiplayerGame.AccesLock = false;
            }
            else
            {
                while (poker.Players[(poker.IndexActivePlayer) % poker.Players.Count].Folded)
                {
                    poker.IndexActivePlayer++;
                }
                poker.ActivePlayer = poker.Players[poker.IndexActivePlayer++ % poker.Players.Count];
                while (MultiplayerGame.AccesLock)
                {
                    // wachten totdat de server door nextPlayer is gegaan (totdat hij een kaart heeft omgedraaid)
                }
            }

            return PartialView("Multiplayer", poker);
        }

        public ActionResult FoldMultiplayer(string name)
        {
            Poker poker;
            MultiplayerGame.PDictionary.TryGetValue(name, out poker);
            if (poker.IsEndGame)
            {
                return PartialView("EndGameMultiplayer", poker);
            }
            if (poker.IsServer)
            {
                poker.ActivePlayer.Fold();
                poker.NextPlayer();
                MultiplayerGame.AccesLock = false;
            }
            else
            {
                while (poker.Players[(poker.IndexActivePlayer) % poker.Players.Count].Folded)
                {
                    poker.IndexActivePlayer++;
                }
                poker.ActivePlayer = poker.Players[poker.IndexActivePlayer++ % poker.Players.Count];
                while (MultiplayerGame.AccesLock)
                {
                    // wachten totdat de server door nextPlayer is gegaan (totdat hij een kaart heeft omgedraaid)
                }
            }
            return PartialView("Multiplayer", poker);
        }

        public ActionResult BetMultiplayer(string name, int amount)
        {

            Poker poker;
            MultiplayerGame.PDictionary.TryGetValue(name, out poker);
            if (poker.IsEndGame)
            {
                return View("EndGame", poker);
            }
            try
            {
                if (poker.IsServer)
                {
                    poker.ActivePlayer.Bet(amount);
                    poker.NextPlayer();
                    MultiplayerGame.AccesLock = false;
                }
                else
                {
                    while (poker.Players[(poker.IndexActivePlayer) % poker.Players.Count].Folded)
                    {
                        poker.IndexActivePlayer++;
                    }
                    poker.ActivePlayer = poker.Players[poker.IndexActivePlayer++ % poker.Players.Count];
                    while (MultiplayerGame.AccesLock)
                    {
                        // wachten totdat de server door nextPlayer is gegaan (totdat hij een kaart heeft omgedraaid)
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                TempData["message"] = $"{e.Message}";
                return PartialView("Multiplayer", poker);
            }
            return PartialView("Multiplayer", poker);
        }

        public ActionResult EndGame(string name)
        {
            Poker poker;
            Poker serverPoker;
            MultiplayerGame.PDictionary.TryGetValue(name, out poker);
            MultiplayerGame.PDictionary.TryGetValue(MultiplayerGame.ServerPlayer, out serverPoker);
            poker.WinningHand = serverPoker.WinningHand;
            return View("EndGameMultiplayer", poker);
        }

        public ActionResult NextRoundMultiplayer(string name)
        {
            Poker poker;
            Poker serverPoker;

            MultiplayerGame.PDictionary.TryGetValue(name, out poker);
            try
            {
                if (poker.IsServer)
                {
                    MultiplayerGame.PDictionary.TryGetValue(MultiplayerGame.ServerPlayer, out serverPoker);
                    serverPoker = new Poker(poker.Players);
                    serverPoker.PrepareForNextRound();
                    serverPoker.IsEndGame = false;
                    serverPoker.IsServer = true;
                    serverPoker.Host = serverPoker.Players.FirstOrDefault(p => p.Name.Equals(name));
                    serverPoker.Host.Opponents = serverPoker.Players.Where(p => !p.Name.Equals(name)).ToList();
                    HttpContext.Session["Poker"] = serverPoker;
                    MultiplayerGame.PDictionary[MultiplayerGame.ServerPlayer] = serverPoker;
                    MultiplayerGame.AccesLock = false;
                    return PartialView("Multiplayer", MultiplayerGame.PDictionary[MultiplayerGame.ServerPlayer]);
                }
                while (MultiplayerGame.AccesLock)
                {
                    //Wachten totdat de servergame opnieuw geinitialiseerd is.
                }
                MultiplayerGame.PDictionary.TryGetValue(MultiplayerGame.ServerPlayer, out serverPoker);
                poker.Players = serverPoker.Players;
                poker.Table = serverPoker.Table;
                poker.Host = serverPoker.Players.FirstOrDefault(p => p.Name.Equals(name));
                poker.Host.Opponents = serverPoker.Players.Where(p => !p.Name.Equals(name)).ToList();
                poker.ActivePlayer = serverPoker.ActivePlayer;
                poker.IndexActivePlayer = serverPoker.IndexActivePlayer;

                poker.StartIndex = serverPoker.StartIndex;

                poker.RollingIndex = serverPoker.RollingIndex;

                poker.PreFlop = false;
                poker.IsServer = false;
                poker.IsEndGame = false;
                MultiplayerGame.PDictionary[name] = poker;
            }
            catch (Exception e)
            {
                TempData["message"] = $"{e.Message}";
            }

            return PartialView("Multiplayer", poker);
        }

        public bool GetIndexActivePlayer(Poker poker)
        {
            return poker.ActivePlayer.SeatNr != poker.SeatNrPlayer;
        }
        public ActionResult GetNameActivePlayer(Poker poker)
        {
            return Json(new MultiplayerViewModel(poker.Host.Name, poker.Host.Money), JsonRequestBehavior.AllowGet);
            //return System.Web.Helpers.Json(new MultiplayerViewModel(player.Name, player.Money), JsonRequestBehavior.AllowGet);
        }
        public string GetMinimumBet(Poker poker, string name)
        {
            if (name == null)
            {
                var data = poker.Table.CurrentBet.ToString() + ":" + poker.ActivePlayer.Money.ToString();
                return data;
            }
            Poker p;
            MultiplayerGame.PDictionary.TryGetValue(name, out p);
            var data1 = p.Table.CurrentBet.ToString() + ":" + p.ActivePlayer.Money.ToString();
            return data1;

        }
    }
}
