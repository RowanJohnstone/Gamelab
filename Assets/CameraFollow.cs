using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform follow;
    //in blender remove head bobbing, enable x rotation in one object and y/z rotation in another
    // Start is called before the first frame update
  

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = follow.position;
    }
}
