using System;

namespace NomadsPlanet.Utils
{
    public enum MatchmakerPollingResult
    {
        Success,
        TicketCreationError,
        TicketCancellationError,
        TicketRetrievalError,
        MatchAssignmentError
    }

    public enum CharacterType
    {
        Null = -1,
        Aqua,
        Cute,
        Green,
        Idol,
        Mint,
        Orange,
        Pink,
        Red,
    }

    public enum CarType
    {
        Null = -1,
        Black,
        Blue,
        Carrot,
        Grey,
        Orange,
        Red,
        Sports,
        White,
    }

    public enum AuthState
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Error,
        TimeOut,
    }

    public enum Map
    {
        Default,
    }

    public enum GameMode
    {
        Default,
    }

    public enum GameQueue
    {
        Solo,
        Team,
    }

    public enum LightType
    {
        Red,
        Yellow,
        Green
    }

    public enum LaneType
    {
        First,
        Second,
    }

    [Flags]
    public enum TrafficType
    {
        Left = 1,
        Right = 2,
        Forward = 4,
    }
}