using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public ActorController ac;
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;

    void Awake()
    {
        GameObject sensor = transform.Find("Sensor").gameObject;
        ac = GetComponent<ActorController>();
        GameObject model = ac.model;

        bm = Bind<BattleManager>(sensor);
        wm = Bind<WeaponManager>(model);
        sm = Bind<StateManager>(gameObject);
    }

    private T Bind<T>(GameObject go) where T : IActorManagerInterface  //����
    {
        T temp;
        temp = go.GetComponent<T>();
        if (temp == null)
        {
            temp = go.AddComponent<T>();
        }
        temp.am = this;
        return temp;
    }

    void Update()
    {
        
    }

    public void TryDoDamage(WeaponController targetWC)
    {
        if (sm.isCounterBack)  //����״̬
        {
            targetWC.wm.am.Stunned();  //�Է����뱻��������
        }
        else if (sm.isImmortal)
        {
            //do nothing
        }
        else if (sm.isDefense || sm.isImpact)
        {
            Impact();
        }
        else
        {
            if (sm.HP <= 0)
            {
                
            }
            else
            {
                sm.AddHP(-5);
                if (sm.HP > 0)
                {
                    Hit();
                }
                else
                {
                    Die();
                }
            }
        }
    }

    public void Stunned()  //������ʱ�Ķ���
    {
        ac.IssueTrigger("stunned");
    }

    public void Impact()
    {
        ac.IssueTrigger("impact");
    }

    public void Hit()
    {
        ac.IssueTrigger("hit");
    }
    public void Die()
    {
        ac.IssueTrigger("die");
        ac.pi.inputEnabled = false;
        //ac.planeVec = Vector3.zero;
        if (ac.camcon.lockState == true)
        {
            ac.camcon.LockUnlock();
        }
        ac.camcon.enabled = false;
    }
}
