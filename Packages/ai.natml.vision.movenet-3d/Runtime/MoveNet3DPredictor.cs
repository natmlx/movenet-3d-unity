/* 
*   MoveNet 3D
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Threading.Tasks;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// MoveNet 3D body pose predictor.
    /// This predictor uses environment depth to project the detected body pose into world space.
    /// </summary>
    public sealed partial class MoveNet3DPredictor : IMLPredictor<MoveNet3DPredictor.Pose> {

        #region --Client API--
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

        /// <summary>
        /// Dispose the predictor and release resources.
        /// </summary>
        public void Dispose () => predictor.Dispose();

        /// <summary>
        /// Create the MoveNet 3D predictor.
        /// </summary>
        /// <param name="smoothing">Apply smoothing filter to detected points.</param>
        /// <param name="configuration">Model configuration.</param>
        /// <param name="accessKey">NatML access key.</param>
        public static async Task<MoveNet3DPredictor> Create (
            bool smoothing = true,
            MLEdgeModel.Configuration configuration = null,
            string accessKey = null
        ) {
            var movenet = await MoveNetPredictor.Create(smoothing, configuration, accessKey);
            var predictor = new MoveNet3DPredictor(movenet);
            return predictor;
        }
        #endregion


        #region --Operations--
        private readonly MoveNetPredictor predictor;

        public MoveNet3DPredictor (MoveNetPredictor predictor) => this.predictor = predictor;
        #endregion
    }
}