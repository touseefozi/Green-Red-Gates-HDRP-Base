using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEditor;
using UnityEngine;

public class CameraSplineFollower : MonoBehaviour
{
    public SplineFollower follower;
    [SerializeField] private CinemachineVirtualCamera trackCam, idleCam;
    public float time;

    private void Start()
    {
        //  follower.onEndReached += OnCompolete;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            follower.follow = true;
            DOVirtual.DelayedCall(time, OnCompolete).SetUpdate(true);
        }
    }

    void OnCompolete()
    {
        trackCam.Priority = 0;
        idleCam.Priority = 1;
    }
}