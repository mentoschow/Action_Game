using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]  //需要一个碰撞器做碰撞检测
public class BattleManager : IActorManagerInterface
{
    private CapsuleCollider defenseCol;

    void Start()
    {
        //初始化碰撞器
        defenseCol = GetComponent<CapsuleCollider>();
        defenseCol.center = Vector3.up * 1f;
        defenseCol.height = 2.0f;
        defenseCol.radius = 0.5f;
        defenseCol.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        WeaponController targetWC = col.GetComponentInParent<WeaponController>();  //获取父节点weapon handle的weapon controller
        if (col.tag == "Weapon")
        {
            am.TryDoDamage(targetWC);
        }
    }
}
