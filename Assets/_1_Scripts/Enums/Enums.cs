using System;

namespace NomadsPlanet.Utils
{
    public enum TrafficSign
    {
        Red,
        Yellow,
        Green
    }
    
    [Flags]
    public enum TrafficType
    {
        Left = 1,
        Right = 2,
        Forward = 4,
    }
}