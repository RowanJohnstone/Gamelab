using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    [SerializeField]
    private Transform follow;
    public Animator animator;
    public GameObject gameobject;
    Quaternion one;
    public Transform forward;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Hard Landing"))
            {
            transform.position = follow.transform.position;
            transform.rotation = follow.transform.rotation;
            
            
        }

        else if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Falling to Roll"))
        {
            transform.position = follow.transform.position;
            transform.rotation = follow.transform.rotation;
            
        }

        else
        {
            transform.position = follow.transform.position;
       
            transform.rotation = Quaternion.Lerp(forward.rotation, follow.transform.rotation, 0.5f);
        }
    }
}
