using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]  //��Ҫһ����ײ������ײ���
public class BattleManager : IActorManagerInterface
{
    private CapsuleCollider defenseCol;

    void Start()
    {
        //��ʼ����ײ��
        defenseCol = GetComponent<CapsuleCollider>();
        defenseCol.center = Vector3.up * 1f;
        defenseCol.height = 2.0f;
        defenseCol.radius = 0.5f;
        defenseCol.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        WeaponController targetWC = col.GetComponentInParent<WeaponController>();  //��ȡ���ڵ�weapon handle��weapon controller
        if (col.tag == "Weapon")
        {
            am.TryDoDamage(targetWC);
        }
    }
}
