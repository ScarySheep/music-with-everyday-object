using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxCubeLine : MonoBehaviour
{
    public int _band;
    public float _startScale, _scaleMultiplier;
    public bool _useBuffer;

    public VfxSoundData VFXDATA;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer){
        transform.localScale = new Vector3(transform.localScale.x, (VFXDATA._audioBandBuffer[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
        }

        if (!_useBuffer){
        transform.localScale = new Vector3(transform.localScale.x, (VFXDATA._audioBand[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
        }
    }
}