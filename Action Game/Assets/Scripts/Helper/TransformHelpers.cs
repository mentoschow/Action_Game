using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelpers
{
    //thisΪ��չ�������������͵ı��������Ե����������������ΪTransform
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
                //������ȵݹ�
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
