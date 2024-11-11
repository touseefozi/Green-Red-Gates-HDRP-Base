using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool Endreached;

    public SizeController sizeController;
    public void DiePlayer()
    {
    if(Endreached)
        sizeController.Die();

    }

}
