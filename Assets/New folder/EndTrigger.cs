using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class EndTrigger : MonoBehaviour
{

    bool triggered;
    public Transform playerArenaPoint;
    public float playerMoveTime;
    public GameObject fightCam;
    public ScaleEnemy scaleEnemy;
    public GameObject playerlevel,EnemyLevel;
    public Trigger trigger;

    private void OnTriggerEnter(Collider other)
    {

        if (triggered)
            return;
        StartCoroutine(scaleEnemy.ScaleRoutine());
        fightCam.SetActive(true);
        if (other.TryGetComponent(out SwerveController controller))
        {
           // playerlevel.SetActive(true);
            //EnemyLevel.SetActive(true);
            triggered = true;
            controller.canWork = false;
            controller.gameObject.transform.DOMove(playerArenaPoint.position, playerMoveTime).OnComplete(() =>
            {
                controller.animator.Play("idle");
           trigger.Endreached = true;

            });

        }
    }
}
