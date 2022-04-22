using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class VfxSoundData : MonoBehaviour
{
    AudioSource _audioSource;
    public static float[] _samples = new float[512];
    float[] _freqBand = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];

    float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
    }

    void CreateAudioBands()
    {
        for(int i = 0; i < 8; i++){
            if (_freqBand[i] > _freqBandHighest[i]){
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; g++){
            if(_freqBand [g] > _bandBuffer [g]){
                _bandBuffer [g] = _freqBand [g];
                _bufferDecrease [g] = 0.005f;
            }

            if(_freqBand [g] < _bandBuffer [g]){
                _bandBuffer [g] -= _bufferDecrease [g];
                _bufferDecrease [g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {

    // 22050 / 512 = 43 hertz per sample

    // frequency bands
    // [0] - 0 - 86 hertz (2)
    // [1] - 87 - 258 hertz (4)
    // [2] - 259 - 602 hertz (8)
    // [3] - 603 - 1290 hertz (16)
    // [4] - 1291 - 2666 hertz (32)
    // [5] - 2667 - 5418 hertz (64)
    // [6] - 5419 - 10922 hertz (128)
    // [7] - 10923 - 21930 hertz (256)

    int count = 0;

    for (int i = 0; i < 8; i++){

        float average = 0;
        int sampleCount = (int)Mathf.Pow (2, i) * 2;

        if (i == 7){
            sampleCount += 2;
        }
        for(int j = 0; j < sampleCount; j++){
            average += _samples[count] * (count + 1);
            count++;
        }

        average /= count;

        _freqBand[i] = average * 10;
    }

    }
}
