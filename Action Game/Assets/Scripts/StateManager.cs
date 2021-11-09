using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : IActorManagerInterface
{
    public float HP = 15f;
    public float HPMax = 100f;
    public float HPMin = 0f;

    [Header("1st order state flags")]
    public bool isGround;
    public bool isJump;
    public bool isRoll;
    public bool isFall;
    public bool isJab;
    public bool isAttack;
    public bool isDefense;
    public bool isHit;
    public bool isDie;
    public bool isImpact;
    public bool isCounterBack = false;

    [Header("2nd order state flags")]
    public bool canDefense;
    public bool isImmortal;  //нч╣п

    void Start()
    {
        HP = HPMax;
    }

    void Update()
    {
        isGround = am.ac.CheckState("ground");
        isJump = am.ac.CheckState("jump");
        isRoll = am.ac.CheckState("roll");
        isFall = am.ac.CheckState("fall");
        isJab = am.ac.CheckState("jab");
        isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");
        isHit = am.ac.CheckState("hit");
        isDie = am.ac.CheckState("die");
        isImpact = am.ac.CheckState("impact", "Defense");
        isCounterBack = am.ac.CheckState("counterback", "Defense");

        canDefense = isGround || isImpact;
        isDefense = canDefense && am.ac.CheckState("defense", "Defense");
        isImmortal = isRoll || isJab;
    }

    public void AddHP(float _HP)
    {
        HP += _HP;
        HP = Mathf.Clamp(HP, HPMin, HPMax);
        
    }
}
