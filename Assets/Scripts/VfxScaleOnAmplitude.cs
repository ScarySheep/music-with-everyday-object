using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxScaleOnAmplitude : MonoBehaviour
{
    public float _startScale, _maxScale;
    public bool _useBuffer;

    //Material _material;
    public SpriteRenderer _spriteColor;
    //public float _red, _green, _blue;
    //public float _green;
    public float _transparency;

    public VfxSoundData VFXDATA;

    // Start is called before the first frame update
    void Start()
    {
        //_material = GetComponent<SpriteRenderer> ().materials [0];
        _spriteColor = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_useBuffer){
            //transform.localScale = new Vector3 ((VFXDATA._amplitude * _maxScale) + _startScale, (VFXDATA._amplitude * _maxScale) + _startScale, (VFXDATA._amplitude * _maxScale) + _startScale);
            transform.localScale = new Vector2 ((VFXDATA._amplitude * _maxScale) + _startScale, (VFXDATA._amplitude * _maxScale) + _startScale);        
            //Color _color = new Color (_red * VFXDATA._amplitude, _green * VFXDATA._amplitude, _blue * VFXDATA._amplitude);
            Color _color = new Color (70, 70, 255, _transparency * VFXDATA._amplitude);
            _spriteColor.color = _color;
        }

        if(_useBuffer){
            //transform.localScale = new Vector3 ((VFXDATA._amplitude * _maxScale) + _startScale, (VFXDATA._amplitude * _maxScale) + _startScale, (VFXDATA._amplitude * _maxScale) + _startScale);
            transform.localScale = new Vector2 ((VFXDATA._amplitude * _maxScale) + _startScale, (VFXDATA._amplitude * _maxScale) + _startScale);
            //Color _color = new Color (_red * VFXDATA._amplitude, _green * VFXDATA._amplitude, _blue * VFXDATA._amplitude);
            Color _color = new Color (70, 70, 255, _transparency * VFXDATA._amplitude);
           _spriteColor.color = _color;
        }
        
    }
}
