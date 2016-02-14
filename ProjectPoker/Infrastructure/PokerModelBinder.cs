using System;
using System.Web.Mvc;
using ProjectPoker.Models;

namespace ProjectPoker.Infrastructure
{
    public class PokerModelBinder : IModelBinder
    {
        private const string Key = "Poker";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Poker poker = controllerContext.HttpContext.Session[Key] as Poker;

            if (poker == null)
            {
                poker = new Poker();
                controllerContext.HttpContext.Session[Key] = poker;
            }
            return poker;
        }
    }
}