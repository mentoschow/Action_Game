using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnAnimatorMove()  //handle root motion
    {
        SendMessageUpwards("OnUpdateRootMotion", (object)anim.deltaPosition);
    }
}
