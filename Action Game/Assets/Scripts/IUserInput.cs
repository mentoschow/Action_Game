using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Header("===== Output Signals =====")]
    public float Dup;
    public float Dright;  //将输入信号转成数字信号
    public float Dmag;
    public Vector3 Dvec;

    public float Jup;
    public float Jright;

    public bool run;  //pressing type
    public bool defense;
    public bool jump;  //trigger once type
    public bool roll;
    protected bool lastjump = false;
    public bool attack;
    public bool heavyAttack;
    protected bool lastAttack = false;
    public bool lockon;
    public bool quit;

    //double trigger type

    [Header("===== Others =====")]
    public bool inputEnabled = true;  //开关

    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;  //应用smoothDamp

    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }

    protected void UpdateDmagDvec(float Dup2, float Dright2)
    {
        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));  //单位化
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;  //摇杆推的方向
    }
}
