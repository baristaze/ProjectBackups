using System;

namespace Shebeke.ObjectModel
{
    public enum Action
    {
        // following values must match with the DB values... see Admin_SPs.sql
        Unknown = 0,
        View = 1,
        Signup = 2,
        Signin = 3,
        Install = 4,
        Invite = 5,
        Create = 6,
        Update = 7,
        Delete = 8,
        Search = 9,
        Share = 10,
        Vote = 11,
        React = 12,
        Comment = 13,
    }
}

