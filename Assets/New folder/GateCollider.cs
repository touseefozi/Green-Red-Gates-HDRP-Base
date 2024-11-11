using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateCollider : MonoBehaviour
{
    public static Action<float> SizeChanged;
    public float hideTime;
    public GameObject _parent;
    public float deltaSize;
    public enum Color
    {
        Green, Red
    }


    private void Start()
    {
        if (redVfx)
            redVfx.transform.parent = null;
    
         if(greenVfx)
            greenVfx.transform.parent = null;
    }

    float downPos = -5;
    public Ease ease;
    public Color color;
    public ParticleSystem greenVfx, redVfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SwerveController s))
        {
            SizeChanged?.Invoke(deltaSize);
            _parent.transform.DOMoveY(downPos, hideTime).SetEase(ease);
            if (color == Color.Green)
            {
                greenVfx.Play();
                SoundController.instance.GoodSound();
                return;
            }
            
                redVfx.Play();
            SoundController.instance.BadSound();

        }
    }


}
