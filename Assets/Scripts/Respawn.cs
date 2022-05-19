using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    //You can drag a game object from your scene into this field in the inspector
    public GameObject myObject;

    private Vector3 savedPosition;

    void Start()
    {
        savedPosition = myObject.transform.position;
    }

    //You can now reset the position of the object to its starting position by calling this function
    private void OnCollisionEnter(Collision collision)
    {
        myObject.transform.position = savedPosition;
    }
   
}

