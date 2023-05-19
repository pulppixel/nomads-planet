// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

namespace FluffyUnderware.Curvy.Controllers
{
    public abstract partial class CurvyController
    {
        /// <summary>
        /// The play state of the controller
        /// </summary>
        public enum CurvyControllerState
        {
            Stopped,
            Playing,
            Paused
        }
    }
}