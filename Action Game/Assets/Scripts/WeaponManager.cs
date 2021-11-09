using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    //private Collider weaponColL;
    private Collider weaponColR;
    //public GameObject whL;  //weapon handle left
    public GameObject whR;  //weapon handle right

    public WeaponController wcR;

    void Start()
    {
        whR = transform.DFS("WeaponHandle").gameObject;  //搜索到武器节点

        wcR = BindWeaponController(whR);  //bind weapon controller to weapon handle

        weaponColR = whR.GetComponentInChildren<Collider>();
    }

    public WeaponController BindWeaponController(GameObject target)
    {
        WeaponController tempWC;
        tempWC = target.GetComponent<WeaponController>();
        if (tempWC == null)
        {
            tempWC = target.AddComponent<WeaponController>();
        }
        tempWC.wm = this;
        return tempWC;
    }

    public void WeaponEnable()  //攻击的动画事件
    {
        if (am.ac.CheckStateTag("attack"))
        {
            weaponColR.enabled = true;
        }
        else
        {
            weaponColR.enabled = false;
        }
    }
    public void WeaponDisable()
    {
        weaponColR.enabled = false;
    }

    public void CounterBackEnable()  //弹反的动画事件
    {

    }

    public void CounterBackDisable()
    {

    }
}
