public static class Config
{
    public const int ImageSize = 416;
    public const int CellsInRow = 13;
    public const int AnchorCount = 5;
    public const int ClassCount = 20;

    public const int InputSize = ImageSize * ImageSize * 3;
    public const int TotalCells = CellsInRow * CellsInRow;
    public const int OutputPerCell = AnchorCount * (5 + ClassCount);
    public const int MaxDetection = TotalCells * AnchorCount;

    public static string[] _labels = new[]
    {
        "Plane", "Bicycle", "Bird", "Boat",
        "Bottle", "Bus", "Car", "Cat",
        "Chair", "Cow", "Table", "Dog",
        "Horse", "Motorbike", "Person", "Plant",
        "Sheep", "Sofa", "Train", "TV"
    };

    //coco dataset
    /*{
        "person","bicycle","car","motorcycle","airplane","bus","train","truck",
        "boat","traffic light","fire hydrant","stop sign","parking meter","bench","bird","cat",
        "dog","horse","sheep","cow","elephant","bear","zebra","giraffe",
        "backpack","umbrella","handbag","tie","suitcase","frisbee","skis","snowboard",
        "sports ball","kite","baseball bat","baseball glove","skateboard","surfboard","tennis racket","bottle",
        "wine glass","cup","fork","knife","spoon","bowl","banana","apple",
        "sandwich","orange","broccoli","carrot","hot dog","pizza","donut","cake",
        "chair","couch","potted plant","bed","dining table","toilet","tv","laptop",
        "mouse","remote","keyboard","cell phone","microwave","oven","toaster","sink",
        "refrigerator","book","clock","vase","scissors","teddy bear","hair drier","toothbrush"
    };*/

    public static string GetLabel(int index)
      => _labels[index];
}