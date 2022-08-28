/*
*   MoveNet 3D
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples.Visualizers {

    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using NatML.Vision;

    /// <summary>
    /// Lightweight 3D body pose skeleton visualizer.
    /// </summary>
    public sealed class MoveNet3DVisualizer : MonoBehaviour {

        #region --Client API--
        /// <summary>
        /// Render a body pose.
        /// </summary>
        /// <param name="pose"></param>
        public void Render (MoveNet3DPredictor.Pose pose) {
            // Delete current
            foreach (var point in currentSkeleton)
                GameObject.Destroy(point.gameObject);
            currentSkeleton.Clear();
            // Instantiate keypoints
            for (var i = 5; i < 17; ++i) {
                var point = Instantiate(keypointPrefab, (Vector3)pose[i], Quaternion.identity, transform);
                point.gameObject.SetActive(true);
                currentSkeleton.Add(point);
            }
            // Instantiate bones
            foreach (var positions in new [] {
                new [] { pose.leftShoulder, pose.rightShoulder },
                new [] { pose.leftShoulder, pose.leftElbow, pose.leftWrist },
                new [] { pose.rightShoulder, pose.rightElbow, pose.rightWrist },
                new [] { pose.leftShoulder, pose.leftHip },
                new [] { pose.rightShoulder, pose.rightHip },
                new [] { pose.leftHip, pose.rightHip },
                new [] { pose.leftHip, pose.leftKnee, pose.leftAnkle },
                new [] { pose.rightHip, pose.rightKnee, pose.rightAnkle }
            }) {
                var bone = Instantiate(bonePrefab, transform.position, Quaternion.identity, transform);
                bone.gameObject.SetActive(true);
                bone.positionCount = positions.Length;
                bone.SetPositions(positions.Select(v => (Vector3)v).ToArray());
                currentSkeleton.Add(bone.transform);
            };
        }
        #endregion


        #region --Operations--
        [SerializeField] Transform keypointPrefab;
        [SerializeField] LineRenderer bonePrefab;
        readonly List<Transform> currentSkeleton = new List<Transform>();
        #endregion
    }
}