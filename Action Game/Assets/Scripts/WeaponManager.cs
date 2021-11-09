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
        whR = transform.DFS("WeaponHandle").gameObject;  //�����������ڵ�

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

    public void WeaponEnable()  //�����Ķ����¼�
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

    public void CounterBackEnable()  //�����Ķ����¼�
    {

    }

    public void CounterBackDisable()
    {

    }
}
