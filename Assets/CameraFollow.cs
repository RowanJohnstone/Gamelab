using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// You need to create an empty, attach this script to it, then parent your camera to that empty. After that just drag your char object to Public Transform player

public class TestCamRotatanator5000 : MonoBehaviour
{




    //   | CAMERA WILL MATCH ROTATION OF ANY OBJECT YOUR STICK IN HERE|
    //   v                                                            v
    public GameObject player;
    //   ^                                                            ^
    //   |                                                            |

  

    

    void Start()
    {
        
    }

    void Update()
    {
        
        transform.position = player.transform.position;
       

    }

}