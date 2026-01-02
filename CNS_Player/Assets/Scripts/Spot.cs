using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    public int id;
    public Transform thisSpot;

    private void Start()
    {
        thisSpot = this.transform;
    }
}
