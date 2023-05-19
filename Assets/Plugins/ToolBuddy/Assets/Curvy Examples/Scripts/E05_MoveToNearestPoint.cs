// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using FluffyUnderware.DevTools;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace FluffyUnderware.Curvy.Examples
{
    [ExecuteAlways]
    public class E05_MoveToNearestPoint : MonoBehaviour
    {
        public Transform Lookup;
        public CurvySpline Spline;
        public Text StatisticsText;
        public Slider Density;
        private readonly TimeMeasure Timer = new TimeMeasure(30);

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            if (Spline && Spline.IsInitialized && Lookup && Spline.Dirty == false)
            {
                // get the nearest point's TF on spline
                Timer.Start();
                transform.position = Spline.GetNearestPoint(
                    Lookup.position,
                    Space.World
                );
                Timer.Stop();
                // set the corresponding position to nearestTF
                StatisticsText.text =
                    string.Format(
                        "Blue Curve Cache Points: {0} \nAverage Lookup (ms): {1:0.000}",
                        Spline.CacheSize,
                        Timer.AverageMS
                    );
            }
        }

        public void OnSliderChange() =>
            Spline.CacheDensity = (int)Density.value;
    }
}