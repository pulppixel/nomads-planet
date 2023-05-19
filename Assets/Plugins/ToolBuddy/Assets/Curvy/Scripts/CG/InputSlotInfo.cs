// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;

namespace FluffyUnderware.Curvy.Generator
{
    /// <summary>
    /// Attribute to define input sot properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InputSlotInfo : SlotInfo
    {
        public bool RequestDataOnly = false;
        public bool Optional = false;

        /// <summary>
        /// Whether this data is altered by the module.
        /// If true, the module providing data to this slot will return a copy of its data, and not the original copy, so you can safely modify it.
        /// </summary>
        // DESIGN should this be removed, and ask users to just clone the data when they need to modify it?
        public bool ModifiesData = false;

        public InputSlotInfo(string name, params Type[] type) : base(
            name,
            type
        ) { }

        public InputSlotInfo(params Type[] type) : this(
            null,
            type
        ) { }

        /// <summary>
        /// Gets whether outType is of same type or a subtype of one of our input types
        /// </summary>
        public bool IsValidFrom(Type outType)
        {
            for (int x = 0; x < DataTypes.Length; x++)
                if (outType == DataTypes[x] || outType.IsSubclassOf(DataTypes[x]))
                    return true;
            return false;
        }
    }
}