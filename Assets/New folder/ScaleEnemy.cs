using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ScaleEnemy : MonoBehaviour
{
    public float targtScale, duration, delay,fightDelay;
   
    public Animator animator;
    public float ScaleUpDelay;
    public IEnumerator ScaleRoutine()
    {

        yield return new WaitForSeconds(0);
        transform.DOScale(targtScale, duration);
        yield return new WaitForSeconds(fightDelay);

        animator.Play("attack");
    
    }

    public IEnumerator ScaleDownRoutine()
    {

        yield return new WaitForSeconds(0.2f);
        transform.DOScale(Vector3.one * 1.5f, ScaleUpDelay).OnComplete(() =>
        {
         
        });


    }
    public SizeController sizeController;
    public void DiePlayer()
    {
        sizeController.Die();

    }
}
