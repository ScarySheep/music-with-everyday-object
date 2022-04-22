using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;

public class ObjectDetector : MonoBehaviour
{
    public TextAsset labelsAsset;
    public NNModel srcModel;
    public Transform displayLocation;
    public Font font;

    private Model model;
    private IWorker engine;
    private string[] labels;
    private const int amountOfClasses = Config.ClassCount;
    private const int boxSectionSmall = Config.BoxSectionSmall;
    private const int boxSectionLarge = Config.BoxSectionLarge;
    private const int anchorBatchSize = Config.ClassCount + 5;
    private const int inputResolutionX = Config.ImageSize;
    private const int inputResolutionY = Config.ImageSize;
    private const float confidenceThreshold = Config.confidenceThreshold;
    private const float iouThreshold = Config.iouThreshold;
    //model output returns box scales relative to the anchor boxes, 3 are used for 40x40/26x26 outputs and other 3 for 20x20/13x13 outputs,
    //each cell has 3 boxes 3x85=255
    private readonly float[] anchors = { 10, 14, 23, 27, 37, 58, 81, 82, 135, 169, 344, 319 };

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
    }

    //restructured data with pixel units
    public struct PixelBox
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public string label;
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
        if (texture.width != inputResolutionX || texture.height != inputResolutionY)
        {
            Debug.LogError("Image resolution must be 640x640. Make sure Texture Import Settings are similar to the example images");
        }

        engine = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model);

        //preprocess image for input
        var input = new Tensor((texture), 3);
        engine.Execute(input);

        //read output tensors
        var outputSmall = engine.PeekOutput("016_convolutional"); //016_convolutional = original output tensor name for 20x20/13x13 boundingBoxes
        var outputLarge = engine.PeekOutput("023_convolutional"); //023_convolutional = original output tensor name for 40x40/26x26 boundingBoxes

        //this list is used to store the original model output data
        List<Box> outputBoxList = new List<Box>();

        //this list is used to store the values converted to intuitive pixel data
        List<PixelBox> pixelBoxList = new List<PixelBox>();

        //decode the output 
        outputBoxList = DecodeOutput(outputSmall, outputLarge);

        //convert output to intuitive pixel data (x,y coords from the center of the image; height and width in pixels)
        pixelBoxList = ConvertBoxToPixelData(outputBoxList);

        //non max suppression (remove overlapping objects)
        pixelBoxList = NonMaxSuppression(pixelBoxList);

        //draw bounding boxes
        for (int i = 0; i < pixelBoxList.Count; i++)
        {
            DrawBox(pixelBoxList[i]);
        }

        //clean memory
        input.Dispose();
        engine.Dispose();
        Resources.UnloadUnusedAssets();
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

    public List<PixelBox> NonMaxSuppression(List<PixelBox> boxList)
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
    }

    public List<Box> DecodeYolo(List<Box> outputBoxList, Tensor output, int boxSections, int anchorMask)
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
                        Box tempBox;
                        tempBox.x = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize];
                        tempBox.y = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 1];
                        tempBox.width = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 2];
                        tempBox.height = output[0, boundingBoxX, boundingBoxY, anchor * anchorBatchSize + 3];
                        tempBox.label = labels[bestIndex];
                        tempBox.anchorIndex = anchor + anchorMask;
                        tempBox.cellIndexY = boundingBoxX;
                        tempBox.cellIndexX = boundingBoxY;
                        outputBoxList.Add(tempBox);
                    }
                }
            }
        }
        return outputBoxList;
    }

    public List<Box> DecodeOutput(Tensor outputSmall, Tensor outputLarge)
    {
        List<Box> outputBoxList = new List<Box>();

        //decode results into a list for each output(20x20/13x13 and 40x40/26x26), anchor mask selects the output box presets (first 3 or the last 3 presets) 
        outputBoxList = DecodeYolo(outputBoxList, outputLarge, boxSectionLarge, 0);
        outputBoxList = DecodeYolo(outputBoxList, outputSmall, boxSectionSmall, 3);

        return outputBoxList;
    }

    public List<PixelBox> ConvertBoxToPixelData(List<Box> boxList)
    {
        List<PixelBox> pixelBoxList = new List<PixelBox>();
        for (int i = 0; i < boxList.Count; i++)
        {
            PixelBox tempBox;

            //apply anchor mask, each output uses a different preset box
            var boxSections = boxList[i].anchorIndex > 2 ? boxSectionSmall : boxSectionLarge;

            //move marker to the edge of the picture -> move to the center of the cell -> add cell offset (cell size * amount of cells) -> add scale
            tempBox.x = (float)(-inputResolutionX * 0.5) + inputResolutionX / boxSections * 0.5f +
                        inputResolutionX / boxSections * boxList[i].cellIndexX + Sigmoid(boxList[i].x);
            tempBox.y = (float)(-inputResolutionY * 0.5) + inputResolutionX / boxSections * 0.5f +
                          inputResolutionX / boxSections * boxList[i].cellIndexY + Sigmoid(boxList[i].y);

            //select the anchor box and multiply it by scale
            tempBox.width = anchors[boxList[i].anchorIndex * 2] * (float)Math.Pow(Math.E, boxList[i].width);
            tempBox.height = anchors[boxList[i].anchorIndex * 2 + 1] * (float)Math.Pow(Math.E, boxList[i].height);
            tempBox.label = boxList[i].label;
            pixelBoxList.Add(tempBox);
        }

        return pixelBoxList;
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
