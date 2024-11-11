using System.Collections;
using UnityEngine;

public class InEnemy : MonoBehaviour
{




    [SerializeField] bool LooseFromPlayer;

    public Animator animator;
    public GameObject vfx
;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SwerveController sc))
        {
            if (LooseFromPlayer)
            {
                Instantiate(vfx,transform.position+Vector3.up,Quaternion.identity);
                gameObject.SetActive(false);

            }

            else
            {
                animator.Play("attack");
                sc.canWork = false;
                StartCoroutine(PlayerDie(sc));
                    }
        }


    }

    IEnumerator PlayerDie(SwerveController sc) 
    {

        yield return new WaitForSeconds(0f);
       
        sc.animator.Play("die");

    }

}