using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectPoker.ViewModels
{
    public class PlayerViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name required")]
        public string Name { get; set; }
        [Display(Name = "Amount of bots")]
        [Required(ErrorMessage = "Required")]
        [Range(1,7,ErrorMessage = "Add min 1 and max 7 components")]
        public int AmountOfBots { get; set; }
        public int Check { get; set; }
        public int Raise { get; set; }
        public int Bet { get; set; }
        public bool Fold { get; set; }

        public PlayerViewModel()
        {
            
        }
        public PlayerViewModel(string name, int amountOfBots, int check, int raise, int bet, bool fold)
        {
            Name = name;
            AmountOfBots = amountOfBots;
            Check = check;
            Raise = raise;
            Bet = bet;
            Fold = fold;
        }
    }
    public class MultiplayerViewModel
    {
        public string Name { get; set; }
        public int Money { get; set; }
        public MultiplayerViewModel(string name, int money)
        {
            Name = name;
            Money = money;
        }
    }
}