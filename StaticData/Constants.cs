namespace StaticData
{
    namespace Constants
    {
        public class _Categories
        {
            public const string VIEW = "View";
            public const string CHAT = "Chat";
            public const string SALES= "Sales";
            public const string TRADES= "Trades";
            public const string MEMBERS= "Members";
            public const string COMMUNITIES= "Communities";
            public const string TRADE_ACTION= "TradeAction";
        }

        namespace _States
        {
            public class _TradeUser
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
        public class _RelationshipType
        {
            public enum ID {TRADE_USER = 1, COMMUNITY_USER};
            public const string TRADE_USER = "TradeUser";
            public const string COMMUNITY_USER = "CommunityUser";
        }

        public class _Permissions
        {
            public enum ID {ADMINISTER = 1, BASIC, OWN, REVIEW, PARTICIPATE, VIEW, WATCH, JOIN };
            public const string ADMINISTER = "Administer";
            public const string BASIC = "Basic";
            public const string OWN = "Own";
            public const string REVIEW = "Review";
            public const string PARTICIPATE = "Watch";
            public const string WATCH = "Watch";
            public const string JOIN = "Join";
            public const string VIEW = "View";
        }

        public class _Tables
        {
            public const string TRADE_USER= "TradeUser";
        }

        namespace _Actions
        {
            public class _Trade
            {
                public enum ID {GET=1, EXCLUDE_USER, SPLIT, FINALIZE, WITHDRAW, WATCH, JOIN, IGNORE };

                public const string GET = "TradeViewDetails/{0}";
                public const string EXCLUDE_USER = "/trade/exclude/{0}/{1}";
                public const string SPLIT = "/trade/split/{0}";
                public const string FINALIZE = "/trade/finalize/{0}";
                public const string WITHDRAW = "/trade/withdraw/{0}/{1}";
                public const string WATCH = "/trade/watch/{tradeId}/{userId}";
                public const string JOIN = "/trade/join/{tradeId}/{userId}";
                public const string IGNORE = "/trade/ignore/{tradeId}/{userId}";


            }
        }
    }
}
