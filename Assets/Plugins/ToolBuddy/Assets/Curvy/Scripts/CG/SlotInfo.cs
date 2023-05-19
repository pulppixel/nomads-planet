// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

namespace FluffyUnderware.Curvy.Generator
{
    /// <summary>
    /// Attribute to define slot properties
    /// </summary>
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1813:AvoidUnsealedAttributes"
    )]
    [AttributeUsage(AttributeTargets.Field)]
    public class SlotInfo : Attribute, IComparable
    {
        /// <summary>
        /// Defines what type of Array is used
        /// </summary>
        public enum SlotArrayType
        {
            Unknown,

            /// <summary>
            /// An array that behaves like an array code wise and UI wise
            /// </summary>
            Normal,

            /// <summary>
            /// An array that behave like an array code wise, but is displayed as a single instance of CGData UI wise.
            /// This allows for CG modules to send/receive arrays, without giving the user the possibility to link multiple modules to the slot
            /// </summary>
            Hidden
        }

        [ItemNotNull]
        [JetBrains.Annotations.NotNull]
        public readonly Type[] DataTypes;

        /// <summary>
        /// If empty Field's name will be used, with slight modifications
        /// </summary>
        public string Name;

        private string displayName;

        /// <summary>
        /// If not null, this string will be used in the UI, while <see cref="Name"/> will be used in the data serialization and slots linking logic
        /// </summary>
        public string DisplayName
        {
            get => displayName ?? Name;
            set => displayName = value;
        }

        public string Tooltip;

        /// <summary>
        /// Whether or not the slot accepts an array of CGData instances or a single instance of it
        /// </summary>
        public bool Array; //DESIGN should be renamed to IsArray

        /// <summary>
        /// When <see cref="Array"/> is true, this value defines what type of Array is used
        /// </summary>
        public SlotArrayType ArrayType = SlotArrayType.Normal;

        protected SlotInfo(string name, [ItemNotNull] [JetBrains.Annotations.NotNull] params Type[] type)
        {
            DataTypes = type;
            Name = name;
        }

        protected SlotInfo([ItemNotNull] [JetBrains.Annotations.NotNull] params Type[] type) : this(
            null,
            type
        ) { }

        public int CompareTo(object obj)
            => String.Compare(
                ((SlotInfo)obj).Name,
                Name,
                StringComparison.Ordinal
            );

        //TODO code analysis (CA1036) says that Equal, !=, <, == and > should be defined since IComparable is implemented

        public void CheckDataTypes()
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            for (int x = 0; x < DataTypes.Length; x++)
            {
                Type[] slotInfoDataTypes = DataTypes;
                if (!slotInfoDataTypes[x].IsSubclassOf(typeof(CGData)))
                    Debug.LogError(
                        string.Format(
                            invariantCulture,
                            "Slot '{0}': Data type needs to be subclass of CGData!",
                            DisplayName
                        )
                    );
            }
        }
    }
}