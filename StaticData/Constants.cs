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

        namespace States
        {

            public class TradeUser
            {
                public enum ID {IOI = 1, WATCHING, JOINED, CONFIRMED, EXCLUDED, EXITED, ADDED };
                public const string IOI = "IOI";
                public const string WATCHING = "Watching";
                public const string JOINED = "Joined";
                public const string CONFIRMED = "Confirmed";
                public const string EXCLUDED = "Excluded";
                public const string EXITED = "Exited";
                public const string ADDED = "Added";
            }
        }
        public class RelationshipType
        {
            public enum ID {TRADE_USER = 1};
        }

        public class Actions
        {
            public enum ID {GET=1, EXCLUDE_USER, SPLIT, FINALIZE, WITHDRAW, WATCH, JOIN };
        }
        public class Permissions
        {
            public enum ID {PARTICIPATE=4, VIEW, WATCH, JOIN };
            public const string WATCH = "Watch";
            public const string JOIN = "Join";
            public const string VIEW = "View";
        }

        public class Tables
        {
            public const string TRADE_USER= "TradeUser";
        }
        public class ActionRoutes
        {
            public const string WATCH_TRADE= "/trade/watch/{tradeId}/{userId}";
            public const string JOIN_TRADE= "/trade/join/{tradeId}/{userId}";
        }
    }
}
