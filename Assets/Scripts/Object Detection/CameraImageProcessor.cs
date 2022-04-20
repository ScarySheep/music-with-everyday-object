using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Barracuda;
using UI = UnityEngine.UI;


public class CameraImageProcessor : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField, Range(0, 1)] float _scoreThreshold = 0.1f;
    [SerializeField, Range(0, 1)] float _overlapThreshold = 0.5f;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Marker _markerPrefab = null;
    [SerializeField] UI.RawImage _placeHolder = null;

    // Thresholds are exposed to the runtime UI.
    public float scoreThreshold { set => _scoreThreshold = value; }
    public float overlapThreshold { set => _overlapThreshold = value; }

    #endregion

    #region Internal objects

    WebCamTexture _webcamRaw;
    RenderTexture _webcamBuffer;
    ObjectDetector _detector;
    Marker[] _markers = new Marker[50];
    Texture2D m_Texture;

    #endregion

    void Start()
    {
        // Object detector initialization
        _detector = new ObjectDetector(_resources);

        // Marker populating
        for (var i = 0; i < _markers.Length; i++)
        {
            _markers[i] = Instantiate(_markerPrefab, _placeHolder.transform);
        }
    }

    void OnDisable()
    {
        _detector?.Dispose();
        _detector = null;
        //cameraManager.frameReceived -= GetImageAsync;
    }

    void OnDestroy()
    {
        for (var i = 0; i < _markers.Length; i++) Destroy(_markers[i]);
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
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2.
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

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
        if (m_Texture == null)
        {
            m_Texture = new Texture2D(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat,
                false);
        }

        // Copy the image data into the texture.
        m_Texture.LoadRawTextureData(rawData);
        m_Texture.Apply();

        //here is where I'd call the detectingObject function
        detectingObject();
        // Need to dispose the request to delete resources associated
        // with the request, including the raw data.
        request.Dispose();
    }

    void detectingObject()
    {
        // Run the object detector with the webcam input.
        _detector.ProcessImage
          (m_Texture, _scoreThreshold, _overlapThreshold);
        // Marker update
        var i = 0;
        foreach (var box in _detector.DetectedObjects)
        {
            if (i == _markers.Length) break;
            _markers[i++].SetAttributes(box);
        }

        for (; i < _markers.Length; i++) _markers[i].Hide();
    }
}
