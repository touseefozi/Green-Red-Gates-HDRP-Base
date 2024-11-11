using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineCam : MonoBehaviour
{
    public GameObject cam1, cam2;
    public Animator enemyAnimator;
    public SwerveController controller;

    public bool switchCam; // The flag for switching cameras
    private bool isTransitioning = false; // To prevent multiple transitions
    bool completed;
    public ScaleEnemy scaleEnemy;
    void Update()
    {
        if (switchCam && !isTransitioning && !completed)
        {
            StartCoroutine(SwitchToCam2());
        }
       
    }

    IEnumerator SwitchToCam2()
    {
    
        isTransitioning = true;

        // Start transition by enabling cam2 and disabling cam1 after a delay
        cam2.SetActive(true);

        // Optionally, play transition animation or wait for a short duration
        yield return new WaitForSeconds(0.5f);
        cam1.SetActive(false);
        yield return new WaitForSeconds(0.8f); // Adjust duration as needed for transition
        StartCoroutine(scaleEnemy.ScaleDownRoutine());

       
        completed = true;
        isTransitioning = false;

        yield return new WaitForSeconds(0.9f);
        controller.canWork = true; // Enable controller after transition completes
       
    }


}
