/* 
*   MoveNet 3D
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using UnityEngine.XR.ARFoundation;
    using NatML.Features;
    using NatML.Vision;
    using Visualizers;

    public sealed class MoveNet3DSample : MonoBehaviour {

        [Header(@"AR")]
        public Camera arCamera;
        public ARCameraManager cameraManager;
        public AROcclusionManager occlusionManager;

        [Header(@"Visualizer")]
        public MoveNet3DVisualizer visualizer;

        private MLImageFeature imageFeature;
        private MoveNet3DPredictor predictor;

        async void Start () {
            Debug.Log("Fetching model data from NatML...");
            // Create the MoveNet 3D predictor
            predictor = await MoveNet3DPredictor.Create();
        }

        void Update () {
            // Check that the predictor has been created
            if (predictor == null)
                return;
            // Get the camera image
            if (cameraManager.TryAcquireLatestCpuImage(out var image)) using (image)
                // Get depth image
                if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out var depth)) using (depth) {
                    // Create image feature
                    var imageType = image.GetFeatureType();
                    imageFeature ??= new MLImageFeature(imageType.width, imageType.height);
                    imageFeature.CopyFrom(image);
                    // Create depth feature
                    var depthFeature = new MLXRCpuDepthFeature(depth, arCamera);
                    // Predict
                    var pose = predictor.Predict(imageFeature, depthFeature);
                    // Visualize
                    visualizer.Render(pose);
                }
        }

        void OnDisable () {
            // Dispose the predictor
            predictor?.Dispose();
        }
    }
}