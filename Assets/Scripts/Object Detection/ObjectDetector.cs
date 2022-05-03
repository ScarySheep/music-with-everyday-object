using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using UnityEngine.XR.ARFoundation;

public class ObjectDetector : MonoBehaviour
{
    public TextAsset labelsAsset;
    public NNModel srcModel;
    public Transform displayLocation;
    public Font font;
    public ARRaycastManager rayCastManager;
    public List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public Vector3 TO_PLACE_POS;

    private Model model;
    private IWorker engine;
    private string[] labels;
    private const int amountOfClasses = Config.ClassCount;
    private const int boxSectionSmall = Config.BoxSectionSmall;
    private const int boxSectionLarge = Config.BoxSectionLarge;
    private const int anchorBatchSize = Config.ClassCount + 5;
    private const int onScreenResolutionX = Config.CaptureSize;
    private const int onScreenResolutionY = Config.CaptureSize;
    private const float confidenceThreshold = Config.confidenceThreshold;
    private const float iouThreshold = Config.iouThreshold;
    //model output returns box scales relative to the anchor boxes, 3 are used for 40x40/26x26 outputs and other 3 for 20x20/13x13 outputs,
    //each cell has 3 boxes 3x85=255
    private readonly float[] anchors = { 10, 14, 23, 27, 37, 58, 81, 82, 135, 169, 344, 319 };

    // related to code flow
    GameObject placed;


    //box struct with the original output data
    public struct Box
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public string label;
        public int anchorIndex;
        public int cellIndexX;
        public int cellIndexY;
        public float value;
    }

    //restructured data with pixel units
    public struct PixelBox
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public string label;
        public float value;
    }

    void Start()
    {
        //parse neural net labels
        labels = labelsAsset.text.Split('\n');
        //load model
        model = ModelLoader.Load(srcModel);
    }

    public void ExecuteML(Texture texture)
    {
        ClearAnnotations();
        if (texture.width != Config.ImageSize || texture.height != Config.ImageSize)
        {
            //Debug.LogError("Image resolution must be 640x640. Make sure Texture Import Settings are similar to the example images");
            ARDebug.log("Wrong image size");
        }

        engine = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model);

        //preprocess image for input
        var input = new Tensor((texture), 3);
        engine.Execute(input);

        //read output tensors
        var outputSmall = engine.PeekOutput("016_convolutional"); //016_convolutional = original output tensor name for 20x20/13x13 boundingBoxes
        var outputLarge = engine.PeekOutput("023_convolutional"); //023_convolutional = original output tensor name for 40x40/26x26 boundingBoxes

        //this list is used to store the original model output data
        Box outputBox = new Box();

        //this list is used to store the values converted to intuitive pixel data
        PixelBox pixelBox = new PixelBox();

        //decode the output 
        outputBox = DecodeOutput(outputSmall, outputLarge);

        //convert output to intuitive pixel data (x,y coords from the center of the image; height and width in pixels)
        pixelBox = ConvertBoxToPixelData(outputBox);

        //non max suppression (remove overlapping objects)
        //pixelBox = NonMaxSuppression(pixelBox);

        //draw bounding boxes
        DrawBox(pixelBox);

        //try to raycast
        if (pixelBox.value != 0)
        {
            Vector3? hit = Raycast();
            if (hit != null)
            {
                //Athenaaaaaa add your code here :)
                //GameObject newObj = Instantiate(objectToPlace, (Vector3)hit, Quaternion.identity);
                TO_PLACE_POS = (Vector3)hit;
            }
        }

        //clean memory
        input.Dispose();
        engine.Dispose();
        Resources.UnloadUnusedAssets();
    }

    public Vector3 GetToPlacePos()
    {
        return TO_PLACE_POS;
    }

    private Vector3? Raycast()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        hits = new List<ARRaycastHit>();
        rayCastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0)
        {
            return hits[0].pose.position;
        }
        return null;
    }

    public void DrawBox(PixelBox box)
    {
        //add bounding box
        GameObject panel = new GameObject("ObjectBox");
        panel.AddComponent<CanvasRenderer>();
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 1, 1, 0.2f);
        panel.transform.SetParent(displayLocation, false);
        panel.transform.localPosition = new Vector3(box.x, -box.y);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(box.width, box.height);

        //add class label
        GameObject text = new GameObject("ObjectLabel");
        text.AddComponent<CanvasRenderer>();
        Text txt = text.AddComponent<Text>();
        text.transform.SetParent(panel.transform, false);
        txt.font = font;
        txt.text = box.label;
        txt.color = new Color(1, 0, 0, 1);
        txt.fontSize = 40;
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        RectTransform rt2 = text.GetComponent<RectTransform>();
        rt2.offsetMin = new Vector2(20, rt2.offsetMin.y);
        rt2.offsetMax = new Vector2(0, rt2.offsetMax.y);
        rt2.offsetMax = new Vector2(rt2.offsetMax.x, 0);
        rt2.offsetMin = new Vector2(rt2.offsetMin.x, 0);
        rt2.anchorMin = new Vector2(0, 0);
        rt2.anchorMax = new Vector2(1, 1);
    }

    public void EraseBoxAndLabel()
    {
        GameObject drawnBox = GameObject.Find("ObjectBox");
        Destroy(drawnBox);
        GameObject writtenLabel = GameObject.Find("ObjectLabel");
        Destroy(writtenLabel);
    }

    /*public List<PixelBox> NonMaxSuppression(List<PixelBox> boxList)
    {
        for (int i = 0; i < boxList.Count - 1; i++)
        {
            for (int j = i + 1; j < boxList.Count; j++)
            {
                if (IntersectionOverUnion(boxList[i], boxList[j]) > iouThreshold && boxList[i].label == boxList[j].label)
                {
                    boxList.RemoveAt(i);
                }
            }
        }
        return boxList;
    }*/

    public Box DecodeOutput(Tensor outputSmall, Tensor outputLarge)
    {
        Box outputBox = new Box();
        outputBox.value = 0;
        //decode results into a list for each output(20x20/13x13 and 40x40/26x26), anchor mask selects the output box presets (first 3 or the last 3 presets) 
        outputBox = DecodeYolo(outputBox, outputLarge, boxSectionLarge, 0);
        outputBox = DecodeYolo(outputBox, outputSmall, boxSectionSmall, 3);

        return outputBox;
    }

    public Box DecodeYolo(Box outputBox, Tensor output, int boxSections, int anchorMask)
    {
        for (int boundingBoxX = 0; boundingBoxX < boxSections; boundingBoxX++)
        {
            for (int boundingBoxY = 0; boundingBoxY < boxSections; boundingBoxY++)
            {
                for (int anchor = 0; anchor < 3; anchor++)
                {
                    if (output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 4] > confidenceThreshold)
                    {
                        //identify the best class
                        float bestValue = 0;
                        int bestIndex = 0;
                        for (int i = 0; i < amountOfClasses; i++)
                        {
                            float value = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 5 + i];
                            if (value > bestValue)
                            {
                                bestValue = value;
                                bestIndex = i;
                            }
                        }
                        //Debug.Log(labels[bestIndex]);
                        if (bestValue > outputBox.value)
                        {
                            Box tempBox;
                            tempBox.x = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize];
                            tempBox.y = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 1];
                            tempBox.width = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 2];
                            tempBox.height = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 3];
                            tempBox.label = labels[bestIndex];
                            tempBox.anchorIndex = anchor + anchorMask;
                            tempBox.cellIndexY = boundingBoxX;
                            tempBox.cellIndexX = boundingBoxY;
                            tempBox.value = bestValue;
                            outputBox = tempBox;
                        }
                    }
                }
            }
        }
        return outputBox;
    }



    public PixelBox ConvertBoxToPixelData(Box box)
    {
        PixelBox pixelBox = new PixelBox();

        //apply anchor mask, each output uses a different preset box
        var boxSections = box.anchorIndex > 2 ? boxSectionSmall : boxSectionLarge;

        //move marker to the edge of the picture -> move to the center of the cell -> add cell offset (cell size * amount of cells) -> add scale
        pixelBox.x = (float)(-onScreenResolutionX * 0.5) + onScreenResolutionX / boxSections * 0.5f +
                    onScreenResolutionX / boxSections * box.cellIndexX + Sigmoid(box.x);
        pixelBox.y = (float)(-onScreenResolutionY * 0.5) + onScreenResolutionX / boxSections * 0.5f +
                        onScreenResolutionX / boxSections * box.cellIndexY + Sigmoid(box.y);

        //select the anchor box and multiply it by scale
        pixelBox.width = anchors[box.anchorIndex * 2] * (float)Math.Pow(Math.E, box.width);
        pixelBox.height = anchors[box.anchorIndex * 2 + 1] * (float)Math.Pow(Math.E, box.height);
        pixelBox.label = box.label;
        pixelBox.value = box.value;

        return pixelBox;
    }


    public float IntersectionOverUnion(PixelBox box1, PixelBox box2)
    {
        //top left and bottom right corners of two rectangles
        float b1x1 = box1.x - 0.5f * box1.width;
        float b1x2 = box1.x + 0.5f * box1.width;
        float b1y1 = box1.y - 0.5f * box1.height;
        float b1y2 = box1.y + 0.5f * box1.height;
        float b2x1 = box2.x - 0.5f * box2.width;
        float b2x2 = box2.x + 0.5f * box2.width;
        float b2y1 = box2.y - 0.5f * box2.height;
        float b2y2 = box2.y + 0.5f * box2.height;

        //intersection rectangle
        float xLeft = Math.Max(b1x1, b2x1);
        float yTop = Math.Max(b1y1, b2y1);
        float xRight = Math.Max(b1x2, b2x2);
        float yBottom = Math.Max(b1y2, b2y2);

        //check if intersection rectangle exist
        if (xRight < xLeft || yBottom < yTop)
        {
            return 0.0f;
        }

        float intersectionArea = (xRight - xLeft) * (yBottom - yTop);
        float b1area = (b1x2 - b1x1) * (b1y2 - b1y1);
        float b2area = (b2x2 - b2x1) * (b2y2 - b2y1);
        return intersectionArea / (b1area + b2area - intersectionArea);
    }

    public float Sigmoid(float value)
    {
        return 1.0f / (1.0f + (float)Math.Exp(-value));
    }

    public void ClearAnnotations()
    {
        foreach (Transform child in displayLocation)
        {
            Destroy(child.gameObject);
        }
    }
}
