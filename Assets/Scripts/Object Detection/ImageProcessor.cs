using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ImageProcessor : MonoBehaviour
{
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] Text debug;
    [SerializeField] RawImage placeHolder;
    ObjectDetector objectDetector;
    Texture2D texture = null;
    bool resolutionSetup = false;

    void Start()
    {
        objectDetector = GetComponent<ObjectDetector>();
    }

    void Update()
    {
        if (!resolutionSetup)
        {
            setupResolution();
        }
    }

    void setupResolution()
    {
        if ((cameraManager == null) || (cameraManager.subsystem == null) || !cameraManager.subsystem.running)
            return;
        //setup ar camera to a specific resolution
        using (var configurations = cameraManager.GetConfigurations(Allocator.Temp))
        {
            if (!configurations.IsCreated || (configurations.Length <= 0))
            {
                return;
            }

            debug.text = "camera config error";
            for (int i = 0; i < configurations.Length; ++i)
            {
                var config = configurations[i];
                if (config.width == 1920 && config.height == 1080)
                {
                    // Get that configuration by index
                    // Make it the active one
                    cameraManager.currentConfiguration = config;
                    debug.text = "camera config set";
                    break;
                }
            }
            resolutionSetup = true;
        }
    }
    public void GetImageAsync()
    {
        if (!resolutionSetup)
        {
            setupResolution();
        }
        // Get information about the device camera image.
        if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            // If successful, launch a coroutine that waits for the image
            // to be ready, then apply it to a texture.
            StartCoroutine(ProcessImage(image));

            // It's safe to dispose the image before the async operation completes.
            image.Dispose();
        }
    }

    IEnumerator ProcessImage(XRCpuImage image)
    {
        // Create the async conversion request.
        var request = image.ConvertAsync(new XRCpuImage.ConversionParams
        {
            // Use the full image.
            inputRect = new RectInt((int)((image.width - Config.CaptureSize) / 2), (int)((image.height - Config.CaptureSize) / 2), Config.CaptureSize, Config.CaptureSize),
            //inputRect = new RectInt(0, 0, 64, 64),

            // Downsample by 2.
            outputDimensions = new Vector2Int(Config.ImageSize, Config.ImageSize),

            // Color image format.
            outputFormat = TextureFormat.RGBA32,

            // Flip across the Y axis.
            transformation = XRCpuImage.Transformation.MirrorY
        });

        // Wait for the conversion to complete.
        while (!request.status.IsDone())
            yield return null;

        // Check status to see if the conversion completed successfully.
        if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
        {
            // Something went wrong.
            Debug.LogErrorFormat("Request failed with status {0}", request.status);

            // Dispose even if there is an error.
            request.Dispose();
            yield break;
        }

        // Image data is ready. Let's apply it to a Texture2D.
        var rawData = request.GetData<byte>();

        // Create a texture if necessary.
        if (texture == null)
        {
            texture = new Texture2D(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat,
                false);
        }

        // Copy the image data into the texture.
        texture.LoadRawTextureData(rawData);
        texture.Apply();
        placeHolder.texture = texture;
        //here is where I'd call the detectingObject function
        objectDetector.ExecuteML(texture);

        // Need to dispose the request to delete resources associated
        // with the request, including the raw data.
        request.Dispose();
    }
}
