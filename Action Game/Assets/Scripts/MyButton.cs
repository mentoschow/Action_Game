using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton  //封装硬件输入的类
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;
    public bool IsDelaying = false;

    public float extDuration = 0.15f;  //两次输入的间隔时间

    public float delayDuration = 0.15f;  //延迟判断的时间

    private bool currentState = false;
    private bool lastState = false;

    private MyTimer extTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();

    public void Tick(bool input)  //按下按键时执行
    {
        extTimer.Tick();  //判断状态
        delayTimer.Tick();

        currentState = input;

        IsPressing = currentState;

        OnPressed = false;
        OnReleased = false;
        IsExtending = false;
        IsDelaying = false;

        if (currentState!=lastState)
        {
            if (currentState == true)
            {
                OnPressed = true;
                StartTimer(delayTimer, delayDuration);
            }
            else
            {
                OnReleased = true;
                StartTimer(extTimer, extDuration);
            }
        }
        lastState = currentState;

        if(extTimer.state == MyTimer.STATE.RUN)
        {
            IsExtending = true;
        }

        if (delayTimer.state == MyTimer.STATE.RUN)
        {
            IsDelaying = true;
        }
    }

    private void StartTimer(MyTimer timer, float _duration)
    {
        timer.duration = _duration;
        timer.Go();
    }
}
