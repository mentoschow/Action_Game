using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : IUserInput
{
    [Header("===== Joystick Settings =====")]
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJright = "axis4";
    public string axisJup = "axis5";
    public float buttonTrigger;

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonX = new MyButton();
    public MyButton buttonY = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonRB = new MyButton();
    public MyButton buttonRS = new MyButton();
    public MyButton buttonLT = new MyButton();
    public MyButton buttonRT = new MyButton();
    public MyButton buttonQuit = new MyButton();

    void Update()
    {
        buttonA.Tick(Input.GetButton("Button0"));
        buttonB.Tick(Input.GetButton("Button1"));
        buttonX.Tick(Input.GetButton("Button2"));
        buttonY.Tick(Input.GetButton("Button3"));
        buttonLB.Tick(Input.GetButton("Button4"));
        buttonRB.Tick(Input.GetButton("Button5"));
        buttonRS.Tick(Input.GetButton("Button9"));
        buttonLT.Tick(Input.GetAxis("axis3") == -1);
        buttonRT.Tick(Input.GetAxis("axis3") == 1);
        buttonQuit.Tick(Input.GetButton("Button7"));

        Jup = -Input.GetAxis(axisJup);  //如果发生上下颠倒时改这里的正负符号
        Jright = Input.GetAxis(axisJright);

        targetDup = Input.GetAxis(axisY);
        targetDright = Input.GetAxis(axisX);

        /* 按键功能的核心，要吃透！！ */
        run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;
        jump = buttonA.OnPressed && buttonA.IsExtending;
        roll = buttonA.OnReleased && buttonA.IsDelaying;
        /* ************************** */

        attack = buttonX.OnPressed;
        heavyAttack = buttonY.OnPressed;
        defense = buttonLB.IsPressing;
        lockon = buttonRS.OnPressed;
        quit = buttonQuit.OnPressed;

        if (inputEnabled == false)
        {
            targetDup = 0f;
            targetDright = 0f;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dup2 = tempDAxis.y;
        float Dright2 = tempDAxis.x;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));  //单位化
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;  //摇杆推的方向
    }
}
