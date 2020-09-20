namespace StaticData
{
    namespace Constants
    {
        public class Categories
        {
            public const string VIEW = "View";
            public const string CHAT = "Chat";
            public const string SALES= "Sales";
            public const string TRADES= "Trades";
            public const string MEMBERS= "Members";
            public const string COMMUNITIES= "Communities";
            public const string TRADE_ACTION= "TradeAction";
        }

        public class TradeUserStates
        {
            public const string IOI = "IOI";
            public const string JOINED = "Joined";
            public const string CONFIRMED = "Confirmed";
            public const string EXCLUDED = "Excluded";
            public const string EXITED = "Exited";
            public const string ADDED = "Added";
        }

        public class Permissions
        {
            public const string JOIN = "Join";
            public const string VIEW = "View";
        }

        public class Tables
        {
            public const string TRADE_USER= "TradeUser";
        }
        public class ActionRoutes
        {
            public const string JOIN_TRADE= "/trade/join/{tradeId}/{userId}";
        }
    }
}
