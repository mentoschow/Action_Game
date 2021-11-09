using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//动画事件控制脚本
public class TriggerController : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void ResetTrigger(string triggerName)
    {
        anim.ResetTrigger(triggerName);
    }
}
