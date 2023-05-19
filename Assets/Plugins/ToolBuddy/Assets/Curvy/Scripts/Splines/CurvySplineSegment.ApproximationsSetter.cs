// =====================================================================
// Copyright 2013-2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using FluffyUnderware.Curvy.Pools;
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools.Extensions;
using JetBrains.Annotations;
using ToolBuddy.Pooling.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy
{
    partial class CurvySplineSegment
    {
        private static class ApproximationsSetter
        {
            #region Positions

            public static void SetPositionsToPoint([NotNull] Approximations approximations, Vector3 currentPosition)
            {
                approximations.ResizePositions(1);

                approximations.Positions.Array[0] = currentPosition;
            }

            public static void SetPositionsToLinear([NotNull] Approximations approximations,
                int elementCount,
                Vector3 startPosition,
                Vector3 endPosition)
            {
                approximations.ResizePositions(elementCount);

                //The following code is the inlined version of this code:
                //        for (int i = 1; i < CacheSize; i++)
                //        {
                //            Approximation[i] = interpolateLinear(i * mStepSize);
                //            t = (Approximation[i] - Approximation[i - 1]);
                //            Length += t.magnitude;
                //            ApproximationDistances[i] = Length;
                //            ApproximationT[i - 1] = t.normalized;
                //        }

                float mStepSize = 1f / (elementCount - 1);

                Vector3[] positions = approximations.Positions.Array;
                positions[0] = startPosition;
                for (int i = 1; i < elementCount - 1; i++)
                    positions[i] = OptimizedOperators.LerpUnclamped(
                        startPosition,
                        endPosition,
                        i * mStepSize
                    );

                positions[elementCount - 1] = endPosition;
            }

            public static void SetPositionsToCatmullRom([NotNull] Approximations approximations,
                int elementCount,
                Vector3 startPosition,
                Vector3 endPosition,
                Vector3 preSegmentPosition,
                Vector3 postSegmentPosition)
            {
                approximations.ResizePositions(elementCount);

                //The following code is the inlined version of this code:
                //        for (int i = 1; i < CacheSize; i++)
                //        {
                //            Approximation[i] = interpolateTCB(i * mStepSize);
                //            t = (Approximation[i] - Approximation[i - 1]);
                //            Length += t.magnitude;
                //            ApproximationDistances[i] = Length;
                //            ApproximationT[i - 1] = t.normalized;
                //        }

                float mStepSize = 1f / (elementCount - 1);

                const double Ft1 = -0.5;
                const double Ft2 = 1.5;
                const double Ft3 = -1.5;
                const double Ft4 = 0.5;
                const double Fu2 = -2.5;
                const double Fu3 = 2;
                const double Fu4 = -0.5;
                const double Fv1 = -0.5;
                const double Fv3 = 0.5;

                double FAX = (Ft1 * preSegmentPosition.x)
                             + (Ft2 * startPosition.x)
                             + (Ft3 * endPosition.x)
                             + (Ft4 * postSegmentPosition.x);
                double FBX = preSegmentPosition.x
                             + (Fu2 * startPosition.x)
                             + (Fu3 * endPosition.x)
                             + (Fu4 * postSegmentPosition.x);
                double FCX = (Fv1 * preSegmentPosition.x) + (Fv3 * endPosition.x);
                double FDX = startPosition.x;

                double FAY = (Ft1 * preSegmentPosition.y)
                             + (Ft2 * startPosition.y)
                             + (Ft3 * endPosition.y)
                             + (Ft4 * postSegmentPosition.y);
                double FBY = preSegmentPosition.y
                             + (Fu2 * startPosition.y)
                             + (Fu3 * endPosition.y)
                             + (Fu4 * postSegmentPosition.y);
                double FCY = (Fv1 * preSegmentPosition.y) + (Fv3 * endPosition.y);
                double FDY = startPosition.y;

                double FAZ = (Ft1 * preSegmentPosition.z)
                             + (Ft2 * startPosition.z)
                             + (Ft3 * endPosition.z)
                             + (Ft4 * postSegmentPosition.z);
                double FBZ = preSegmentPosition.z
                             + (Fu2 * startPosition.z)
                             + (Fu3 * endPosition.z)
                             + (Fu4 * postSegmentPosition.z);
                double FCZ = (Fv1 * preSegmentPosition.z) + (Fv3 * endPosition.z);
                double FDZ = startPosition.z;
                Vector3[] positions = approximations.Positions.Array;
                positions[0] = startPosition;
                for (int i = 1; i < elementCount - 1; i++)
                {
                    float localF = i * mStepSize;

                    positions[i].x = (float)((((((FAX * localF) + FBX) * localF) + FCX) * localF) + FDX);
                    positions[i].y = (float)((((((FAY * localF) + FBY) * localF) + FCY) * localF) + FDY);
                    positions[i].z = (float)((((((FAZ * localF) + FBZ) * localF) + FCZ) * localF) + FDZ);
                }

                positions[elementCount - 1] = endPosition;
            }


            public static void SetPositionsToTCB([NotNull] Approximations approximations,
                int elementCount,
                TcbParameters tcbParameters,
                Vector3 startPosition,
                Vector3 endPosition,
                Vector3 preSegmentPosition,
                Vector3 postSegmentPosition)
            {
                approximations.ResizePositions(elementCount);

                //The following code is the inlined version of this code:
                //        for (int i = 1; i < CacheSize; i++)
                //        {
                //            Approximation[i] = interpolateCatmull(i * mStepSize);
                //            t = (Approximation[i] - Approximation[i - 1]);
                //            Length += t.magnitude;
                //            ApproximationDistances[i] = Length;
                //            ApproximationT[i - 1] = t.normalized;
                //        }

                float mStepSize = 1f / (elementCount - 1);

                float ft0 = tcbParameters.StartTension;
                float ft1 = tcbParameters.EndTension;
                float fc0 = tcbParameters.StartContinuity;
                float fc1 = tcbParameters.EndContinuity;
                float fb0 = tcbParameters.StartBias;
                float fb1 = tcbParameters.EndBias;

                double FFA = (1 - ft0) * (1 + fc0) * (1 + fb0);
                double FFB = (1 - ft0) * (1 - fc0) * (1 - fb0);
                double FFC = (1 - ft1) * (1 - fc1) * (1 + fb1);
                double FFD = (1 - ft1) * (1 + fc1) * (1 - fb1);

                double DD = 2;
                double Ft1 = -FFA / DD;
                double Ft2 = ((+4 + FFA) - FFB - FFC) / DD;
                double Ft3 = ((-4 + FFB + FFC) - FFD) / DD;
                double Ft4 = FFD / DD;
                double Fu1 = (+2 * FFA) / DD;
                double Fu2 = ((-6 - (2 * FFA)) + (2 * FFB) + FFC) / DD;
                double Fu3 = ((+6 - (2 * FFB) - FFC) + FFD) / DD;
                double Fu4 = -FFD / DD;
                double Fv1 = -FFA / DD;
                double Fv2 = (FFA - FFB) / DD;
                double Fv3 = FFB / DD;
                double Fw2 = +2 / DD;

                double FAX = (Ft1 * preSegmentPosition.x)
                             + (Ft2 * startPosition.x)
                             + (Ft3 * endPosition.x)
                             + (Ft4 * postSegmentPosition.x);
                double FBX = (Fu1 * preSegmentPosition.x)
                             + (Fu2 * startPosition.x)
                             + (Fu3 * endPosition.x)
                             + (Fu4 * postSegmentPosition.x);
                double FCX = (Fv1 * preSegmentPosition.x) + (Fv2 * startPosition.x) + (Fv3 * endPosition.x);
                double FDX = Fw2 * startPosition.x;

                double FAY = (Ft1 * preSegmentPosition.y)
                             + (Ft2 * startPosition.y)
                             + (Ft3 * endPosition.y)
                             + (Ft4 * postSegmentPosition.y);
                double FBY = (Fu1 * preSegmentPosition.y)
                             + (Fu2 * startPosition.y)
                             + (Fu3 * endPosition.y)
                             + (Fu4 * postSegmentPosition.y);
                double FCY = (Fv1 * preSegmentPosition.y) + (Fv2 * startPosition.y) + (Fv3 * endPosition.y);
                double FDY = Fw2 * startPosition.y;

                double FAZ = (Ft1 * preSegmentPosition.z)
                             + (Ft2 * startPosition.z)
                             + (Ft3 * endPosition.z)
                             + (Ft4 * postSegmentPosition.z);
                double FBZ = (Fu1 * preSegmentPosition.z)
                             + (Fu2 * startPosition.z)
                             + (Fu3 * endPosition.z)
                             + (Fu4 * postSegmentPosition.z);
                double FCZ = (Fv1 * preSegmentPosition.z) + (Fv2 * startPosition.z) + (Fv3 * endPosition.z);
                double FDZ = Fw2 * startPosition.z;

                Vector3[] positions = approximations.Positions.Array;
                positions[0] = startPosition;
                for (int i = 1; i < elementCount - 1; i++)
                {
                    float localF = i * mStepSize;
                    positions[i].x = (float)((((((FAX * localF) + FBX) * localF) + FCX) * localF) + FDX);
                    positions[i].y = (float)((((((FAY * localF) + FBY) * localF) + FCY) * localF) + FDY);
                    positions[i].z = (float)((((((FAZ * localF) + FBZ) * localF) + FCZ) * localF) + FDZ);
                }

                positions[elementCount - 1] = endPosition;
            }


            public static void SetPositionsToBezier([NotNull] Approximations approximations,
                int elementCount,
                Vector3 startPosition,
                Vector3 startTangent,
                Vector3 endPosition,
                Vector3 endTangent)
            {
                approximations.ResizePositions(elementCount);

                //The following code is the inlined version of this code:
                //        for (int i = 1; i < CacheSize; i++)
                //        {
                //            Approximation[i] = interpolateBezier(i * mStepSize);
                //            t = (Approximation[i] - Approximation[i - 1]);
                //            Length += t.magnitude;
                //            ApproximationDistances[i] = Length;
                //            ApproximationT[i - 1] = t.normalized;
                //        }

                float mStepSize = 1f / (elementCount - 1);

                const double Ft2 = 3;
                const double Ft3 = -3;
                const double Fu1 = 3;
                const double Fu2 = -6;
                const double Fu3 = 3;
                const double Fv1 = -3;
                const double Fv2 = 3;

                double FAX = -startPosition.x + (Ft2 * startTangent.x) + (Ft3 * endTangent.x) + endPosition.x;
                double FBX = (Fu1 * startPosition.x) + (Fu2 * startTangent.x) + (Fu3 * endTangent.x);
                double FCX = (Fv1 * startPosition.x) + (Fv2 * startTangent.x);
                double FDX = startPosition.x;

                double FAY = -startPosition.y + (Ft2 * startTangent.y) + (Ft3 * endTangent.y) + endPosition.y;
                double FBY = (Fu1 * startPosition.y) + (Fu2 * startTangent.y) + (Fu3 * endTangent.y);
                double FCY = (Fv1 * startPosition.y) + (Fv2 * startTangent.y);
                double FDY = startPosition.y;

                double FAZ = -startPosition.z + (Ft2 * startTangent.z) + (Ft3 * endTangent.z) + endPosition.z;
                double FBZ = (Fu1 * startPosition.z) + (Fu2 * startTangent.z) + (Fu3 * endTangent.z);
                double FCZ = (Fv1 * startPosition.z) + (Fv2 * startTangent.z);
                double FDZ = startPosition.z;
                Vector3[] positions = approximations.Positions.Array;
                positions[0] = startPosition;
                for (int i = 1; i < elementCount - 1; i++)
                {
                    float localF = i * mStepSize;

                    positions[i].x = (float)((((((FAX * localF) + FBX) * localF) + FCX) * localF) + FDX);
                    positions[i].y = (float)((((((FAY * localF) + FBY) * localF) + FCY) * localF) + FDY);
                    positions[i].z = (float)((((((FAZ * localF) + FBZ) * localF) + FCZ) * localF) + FDZ);
                }

                positions[elementCount - 1] = endPosition;
            }

            public static void SetPositionsToBSpline([NotNull] Approximations approximations,
                int elementCount,
                SubArray<Vector3> splineP0Array,
                BSplineApproximationParameters bSplineParameters)
            {
#if CURVY_SANITY_CHECKS
                Assert.IsTrue(splineP0Array.Array.Length >= bSplineParameters.Degree + 1);
                Assert.IsTrue(bSplineParameters.StartTf.IsBetween0And1());
                Assert.IsTrue(bSplineParameters.EndTf.IsBetween0And1());
#endif

                approximations.ResizePositions(elementCount);

                float mStepSize = 1f / (elementCount - 1);
                float tfIncrement = mStepSize / bSplineParameters.SegmentsCount;

                int controlPointsCount = bSplineParameters.ControlPoints.Count;
                int n = BSplineHelper.GetBSplineN(
                    controlPointsCount,
                    bSplineParameters.Degree,
                    bSplineParameters.IsClosed
                );

                int previousK = int.MinValue;

                int nPlus1 = n + 1;
                Vector3[] positions = approximations.Positions.Array;

                SubArray<Vector3> splinePsVector = splineP0Array;
                Vector3[] ps = splinePsVector.Array;
                int psCount = splinePsVector.Count;

                //positions[0]
                {
                    BSplineHelper.GetBSplineUAndK(
                        bSplineParameters.StartTf,
                        bSplineParameters.IsClamped,
                        bSplineParameters.Degree,
                        n,
                        out float u,
                        out int k
                    );
                    GetBSplineP0s(
                        bSplineParameters.ControlPoints,
                        controlPointsCount,
                        bSplineParameters.Degree,
                        k,
                        ps
                    );
                    positions[0] = bSplineParameters.IsClamped
                        ? BSplineHelper.DeBoorClamped(
                            bSplineParameters.Degree,
                            k,
                            u,
                            nPlus1,
                            ps
                        )
                        : BSplineHelper.DeBoorUnclamped(
                            bSplineParameters.Degree,
                            k,
                            u,
                            ps
                        );
                }

                SubArray<Vector3> psCopySubArray = ArrayPools.Vector3.Allocate(psCount);
                Vector3[] psCopy = psCopySubArray.Array;

                for (int i = 1; i < elementCount - 1; i++)
                {
                    float tf = bSplineParameters.StartTf + (tfIncrement * i);
                    BSplineHelper.GetBSplineUAndK(
                        tf,
                        bSplineParameters.IsClamped,
                        bSplineParameters.Degree,
                        n,
                        out float u,
                        out int k
                    );

                    if (k != previousK)
                    {
                        GetBSplineP0s(
                            bSplineParameters.ControlPoints,
                            controlPointsCount,
                            bSplineParameters.Degree,
                            k,
                            ps
                        );
                        previousK = k;
                    }

                    Array.Copy(
                        ps,
                        0,
                        psCopy,
                        0,
                        psCount
                    );

                    positions[i] = bSplineParameters.IsClamped
                        ? BSplineHelper.DeBoorClamped(
                            bSplineParameters.Degree,
                            k,
                            u,
                            nPlus1,
                            psCopy
                        )
                        : BSplineHelper.DeBoorUnclamped(
                            bSplineParameters.Degree,
                            k,
                            u,
                            psCopy
                        );
                }

                ArrayPools.Vector3.Free(psCopySubArray);

                //positions[cacheSize]
                {
                    BSplineHelper.GetBSplineUAndK(
                        bSplineParameters.EndTf,
                        bSplineParameters.IsClamped,
                        bSplineParameters.Degree,
                        n,
                        out float u,
                        out int k
                    );
                    GetBSplineP0s(
                        bSplineParameters.ControlPoints,
                        controlPointsCount,
                        bSplineParameters.Degree,
                        k,
                        ps
                    );
                    positions[elementCount - 1] = bSplineParameters.IsClamped
                        ? BSplineHelper.DeBoorClamped(
                            bSplineParameters.Degree,
                            k,
                            u,
                            nPlus1,
                            ps
                        )
                        : BSplineHelper.DeBoorUnclamped(
                            bSplineParameters.Degree,
                            k,
                            u,
                            ps
                        );
                }
            }

            #endregion


            #region Orientations

            public static void SetOrientationToNone([NotNull] Approximations approximations,
                int elementCount)
            {
                approximations.ResizeUps(elementCount);
                Array.Clear(
                    approximations.Ups.Array,
                    0,
                    approximations.Ups.Count
                );
            }

            public static void SetOrientationToStatic([NotNull] Approximations approximations,
                int elementCount,
                Vector3 startUp,
                Vector3 endUp)
            {
                approximations.ResizeUps(elementCount);

                Vector3[] upsArray = approximations.Ups.Array;
                upsArray[0] = startUp;
                if (approximations.Ups.Count > 1)
                {
                    float oneOnCacheSize = 1f / (elementCount - 1);
                    for (int i = 1; i < elementCount - 1; i++)
                        upsArray[i] = Vector3.SlerpUnclamped(
                            startUp,
                            endUp,
                            i * oneOnCacheSize
                        );
                    upsArray[elementCount - 1] = endUp;
                }
            }

            public static void SetOrientationToDynamic([NotNull] Approximations approximations,
                int elementCount,
                Vector3 startUp)
            {
                approximations.ResizeUps(elementCount);

#if CURVY_SANITY_CHECKS
                Assert.IsTrue(approximations.Ups.Count == approximations.Tangents.Count);
#endif

                Vector3[] upsArray = approximations.Ups.Array;
                Vector3[] tangentsArray = approximations.Tangents.Array;

                int upsLength = approximations.Ups.Count;
                upsArray[0] = startUp;

                for (int i = 1; i < upsLength; i++)
                {
                    //Inlined version of ups[i] = DTMath.ParallelTransportFrame(ups[i-1], tangents[i - 1], tangents[i]) and with less checks for performance reasons
                    Vector3 tan0 = tangentsArray[i - 1];
                    Vector3 tan1 = tangentsArray[i];
                    //Inlined version of Vector3 A = Vector3.Cross(tan0, tan1);
                    Vector3 A;
                    {
                        A.x = (tan0.y * tan1.z) - (tan0.z * tan1.y);
                        A.y = (tan0.z * tan1.x) - (tan0.x * tan1.z);
                        A.z = (tan0.x * tan1.y) - (tan0.y * tan1.x);
                    }
                    //Inlined version of float a = (float)Math.Atan2(A.magnitude, Vector3.Dot(tan0, tan1));
                    float a = (float)Math.Atan2(
                        Math.Sqrt((A.x * A.x) + (A.y * A.y) + (A.z * A.z)),
                        (tan0.x * tan1.x) + (tan0.y * tan1.y) + (tan0.z * tan1.z)
                    );
                    upsArray[i] = Quaternion.AngleAxis(
                                      Mathf.Rad2Deg * a,
                                      A
                                  )
                                  * upsArray[i - 1];
                }
            }

            #endregion

            #region Tangents and distances

            public static float SetPointTangentsAndDistances([NotNull] Approximations approximations,
                Vector3 previousPosition,
                Vector3 currentPosition,
                Vector3 nextPosition,
                Quaternion currentRotation)
            {
                approximations.ResizeTangents(1);
                approximations.ResizeDistances(1);

#if CURVY_SANITY_CHECKS
                Assert.IsTrue(approximations.Positions.Count == approximations.Tangents.Count);
                Assert.IsTrue(approximations.Positions.Count == approximations.Distances.Count);
#endif

                approximations.Distances.Array[0] = 0;

                if (currentPosition != nextPosition)
                    approximations.Tangents.Array[0] = nextPosition.Subtraction(currentPosition).normalized;
                else if (currentPosition != previousPosition)
                    approximations.Tangents.Array[0] = currentPosition.Subtraction(previousPosition).normalized;
                else
                    approximations.Tangents.Array[0] = currentRotation * Vector3.forward;

                //Last visible control point gets the last tangent from the previous segment. This is done in CurvySpline.EnforceTangentContinuity
                return 0;
            }

            public static float SetSegmentTangentsAnDistances([NotNull] Approximations approximations, int elementCount)
            {
                approximations.ResizeTangents(elementCount);
                approximations.ResizeDistances(elementCount);

#if CURVY_SANITY_CHECKS
                Assert.IsTrue(approximations.Positions.Count == approximations.Tangents.Count);
                Assert.IsTrue(approximations.Positions.Count == approximations.Distances.Count);
#endif

                Vector3[] positions = approximations.Positions.Array;
                float lengthAccumulator = 0;
                Vector3[] tangents = approximations.Tangents.Array;
                float[] distances = approximations.Distances.Array;
                distances[0] = 0;
                Vector3 delta;
                for (int i = 1; i < elementCount; i++)
                {
                    delta.x = positions[i].x - positions[i - 1].x;
                    delta.y = positions[i].y - positions[i - 1].y;
                    delta.z = positions[i].z - positions[i - 1].z;

                    float deltaMagnitude = Mathf.Sqrt((delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z));

                    lengthAccumulator += deltaMagnitude;
                    distances[i] = lengthAccumulator;

                    if (deltaMagnitude > 9.99999974737875E-06)
                    {
                        float oneOnMagnitude = 1 / deltaMagnitude;
                        tangents[i - 1].x = delta.x * oneOnMagnitude;
                        tangents[i - 1].y = delta.y * oneOnMagnitude;
                        tangents[i - 1].z = delta.z * oneOnMagnitude;
                    }
                    else
                    {
                        tangents[i - 1].x = 0;
                        tangents[i - 1].y = 0;
                        tangents[i - 1].z = 0;
                    }
                }

                //is overriden in CurvySpline.EnforceTangentContinuity
                tangents[elementCount - 1] = tangents[elementCount - 2];
                return lengthAccumulator;
            }

            #endregion
        }
    }
}