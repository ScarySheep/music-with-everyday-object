using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System.Runtime.InteropServices;

public class Config : MonoBehaviour
{
    public const int CaptureSize = 720;
    public const int ImageSize = 416;
    public const int ClassCount = 80;
    public const int BoxSectionSmall = 13;
    public const int BoxSectionLarge = 26;
    public const float confidenceThreshold = 0.0f;
    public const float iouThreshold = 0.45f;
}
