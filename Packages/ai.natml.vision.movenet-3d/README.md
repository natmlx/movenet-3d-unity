# MoveNet 3D
Realtime 3D pose tracking with [MoveNet](https://blog.tensorflow.org/2021/05/next-generation-pose-detection-with-movenet-and-tensorflowjs.html) in augmented reality with [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/manual/index.html).

## Installing MoveNet 3D
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.fxn", "ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.movenet-3d": "1.0.2"
  }
}
```

## Predicting 3D Pose in Augmented Reality
These steps assume that you are starting with an AR scene in Unity with an `ARSession` and `ARSessionOrigin`. In your pose detection script, first create the MoveNet 3D predictor:
```csharp
MoveNet3DPredictor predictor;

async void Start () {
    // Create the MoveNet 3D predictor
    predictor = await MoveNet3DPredictor.Create();
}
```

Then in `Update`, acquire the latest CPU camera image and depth image from ARFoundation, then predict the pose:
```csharp
// Assign these in the inspector
public Camera arCamera;
public ARCameraManager cameraManager;
public AROcclusionManager occlusionManager;

void Update () {
    // Get the camera image
    if (cameraManager.TryAcquireLatestCpuImage(out var image))
        // Get the depth image
        if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out var depth)) {
            // Create an ML feature for the camera image
            var imageType = image.GetFeatureType();
            var imageFeature = new MLImageFeature(imageType.width, imageType.height);
            imageFeature.CopyFrom(image);
            // Create an ML feature for the depth image
            var depthFeature = new MLXRCpuDepthFeature(depth, arCamera);
            // Predict
            MoveNet3DPredictor.Pose pose = predictor.Predict(imageFeature, depthFeature);
        }
}
```

The pose contains 3D world positions for each detected keypoint.

> Note that on older iOS devices that don't support environment depth, you can use the human depth image instead which is supported by iPhone XS/XR or newer.

___

## Requirements
- Unity 2022.3+

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://natml.ai/community).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!