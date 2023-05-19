using System;

namespace GameCreator.Runtime.Characters
{
    public class Footstep
    {
        [field: NonSerialized] public bool WasGrounded { get; set; } = true;
    }
}