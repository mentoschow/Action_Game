using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelpers
{
    //this为扩展方法，即该类型的变量都可以调用这个方法，这里为Transform
    public static Transform DFS(this Transform parent, string targetName)  
    {
        Transform temp = null;

        foreach (Transform child in parent)
        {
            if (child.name == targetName)
            {
                return child;
            }
            else
            {
                //深度优先递归
                temp =  DFS(child, targetName);  
                if (temp != null)
                {
                    return temp;
                }
            }
        }
        return null;
    }
}
