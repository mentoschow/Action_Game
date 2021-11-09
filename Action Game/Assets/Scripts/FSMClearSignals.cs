using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMClearSignals : StateMachineBehaviour
{
    public string[] clearAtEnter;
    public string[] clearAtExit;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var signal in clearAtEnter)  //遍历数组，但次数未知，所以使用foreach
        {
            animator.ResetTrigger(signal);
        }
    }


    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var signal in clearAtExit)  //遍历数组，但次数未知，所以使用foreach
        {
            animator.ResetTrigger(signal);
        }
    }
}
