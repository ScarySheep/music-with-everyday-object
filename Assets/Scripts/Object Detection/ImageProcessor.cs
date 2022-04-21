using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ImageProcessor : MonoBehaviour
{
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] RawImage test;
    ObjectDetector objectDetector;
    Texture2D texture = null;


    void Start()
    {
        objectDetector = GetComponent<ObjectDetector>();
    }

    public void GetImageAsync()
    {
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
            inputRect = new RectInt((int)((image.width - 640) / 2), (int)((image.height - 640) / 2), 640, 640),
            //inputRect = new RectInt(0, 0, 64, 64),

            // Downsample by 2.
            outputDimensions = new Vector2Int(640, 640),

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

        //here is where I'd call the detectingObject function
        objectDetector.ExecuteML(texture);

        // Need to dispose the request to delete resources associated
        // with the request, including the raw data.
        request.Dispose();
    }
}
