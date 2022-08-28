/* 
*   MoveNet 3D
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
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

        MLModelData modelData;
        MLModel model;
        MoveNet3DPredictor predictor;

        async void Start () {
            Debug.Log("Fetching model data from NatML...");
            // Fetch model data from NatML Hub
            modelData = await MLModelData.FromHub("@natsuite/movenet-3d");
            // Deserialize the model
            model = modelData.Deserialize();
            // Create the MoveNet 3D predictor
            predictor = new MoveNet3DPredictor(model);
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
                    var imageFeature = new MLXRCpuImageFeature(image);
                    (imageFeature.mean, imageFeature.std) = modelData.normalization;
                    imageFeature.aspectMode = modelData.aspectMode;
                    // Create depth feature
                    var depthFeature = new MLXRCpuDepthFeature(depth, arCamera);
                    // Predict
                    var pose = predictor.Predict(imageFeature, depthFeature);
                    // Visualize
                    visualizer.Render(pose);
                }
        }

        void OnDisable () {
            // Dispose the model
            model?.Dispose();
        }
    }
}