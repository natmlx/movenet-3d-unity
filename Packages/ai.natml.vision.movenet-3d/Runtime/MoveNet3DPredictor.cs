/* 
*   MoveNet 3D
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// MoveNet 3D body pose predictor.
    /// This predictor uses environment depth to project the detected body pose into world space.
    /// </summary>
    public sealed partial class MoveNet3DPredictor {

        #region --Client API--
        /// <summary>
        /// Create a MoveNet 3D predictor.
        /// </summary>
        /// <param name="model">MoveNet ML model.</param>
        /// <param name="cameraManager">AR camera manager.</param>
        /// <param name="smoothing">Apply smoothing filter to detected points.</param>
        public MoveNet3DPredictor (MLModel model, bool smoothing = true) {
            this.model = model as MLEdgeModel;
            this.predictor = new MoveNetPredictor(model, smoothing);
        }

        /// <summary>
        /// Detect the body pose in an image.
        /// </summary>
        /// <param name="inputs">Input image and corresponding depth image.</param>
        /// <returns>Detected body pose.</returns>
        public Pose Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 2)
                throw new ArgumentException(@"MoveNet3D predictor expects an image feature and a depth feature", nameof(inputs));
            // Check image
            var imageFeature = inputs[0];
            var imageType = MLImageType.FromType(imageFeature.type);
            if (!imageType)
                throw new ArgumentException(@"MoveNet3D predictor expects first input feature to be an array or image feature", nameof(inputs));
            // Check depth
            if (!(inputs[1] is MLDepthFeature depthFeature))
                throw new ArgumentException(@"MoveNet3D predictor expects second input feature to be a depth feature", nameof(inputs));
            // Predict
            var pose = predictor.Predict(imageFeature);
            var result = new Pose(pose, imageType, depthFeature);
            return result;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly MoveNetPredictor predictor;
        #endregion
    }
}