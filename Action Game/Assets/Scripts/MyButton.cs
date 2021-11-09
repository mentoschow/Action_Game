using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton  //��װӲ���������
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;
    public bool IsDelaying = false;

    public float extDuration = 0.15f;  //��������ļ��ʱ��

    public float delayDuration = 0.15f;  //�ӳ��жϵ�ʱ��

    private bool currentState = false;
    private bool lastState = false;

    private MyTimer extTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();

    public void Tick(bool input)  //���°���ʱִ��
    {
        extTimer.Tick();  //�ж�״̬
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
