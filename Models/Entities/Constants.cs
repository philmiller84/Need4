using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Constants
    {
        public const string CHAT_CATEGORY = "Chat";
        public const string SALES_CATEGORY = "Sales";
        public const string TRADES_CATEGORY = "Trades";
        public const string MEMBERS_CATEGORY = "Members";
        public const string COMMUNITIES_CATEGORY = "Communities";

        public const string TRADE_ACTION_CATEGORY = "TradeAction";

        public const string VIEW_PERMISSION = "View";
        public const string TRADE_USER_TABLE = "TradeUser";
        public const string JOIN_TRADE_ROUTE = "/trade/join/{tradeId}/{userId}";
        public const string VIEW_CATEGORY = "View";
    }
}
