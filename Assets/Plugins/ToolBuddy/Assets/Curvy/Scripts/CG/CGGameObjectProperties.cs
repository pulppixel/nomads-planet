// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using FluffyUnderware.DevTools;
using JetBrains.Annotations;
using UnityEngine;

namespace FluffyUnderware.Curvy.Generator
{
    /// <summary>
    /// Helper class used by InputGameObject module
    /// </summary>
    [Serializable]
    public class CGGameObjectProperties
    {
        [SerializeField]
        [CanBeNull]
        private GameObject m_Object;

        [SerializeField]
        [VectorEx]
        private Vector3 m_Translation;

        [SerializeField]
        [VectorEx]
        private Vector3 m_Rotation;

        [SerializeField]
        [VectorEx]
        private Vector3 m_Scale = Vector3.one;

        [CanBeNull]
        public GameObject Object
        {
            get => m_Object;
            set => m_Object = value;
        }

        public Vector3 Translation
        {
            get => m_Translation;
            set => m_Translation = value;
        }

        public Vector3 Rotation
        {
            get => m_Rotation;
            set => m_Rotation = value;
        }

        public Vector3 Scale
        {
            get => m_Scale;
            set => m_Scale = value;
        }

        public Matrix4x4 Matrix => Matrix4x4.TRS(
            Translation,
            Quaternion.Euler(Rotation),
            Scale
        );

        public CGGameObjectProperties() { }

        public CGGameObjectProperties(GameObject gameObject) =>
            Object = gameObject;
    }
}