using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;  //获取模型物件
    public CameraController camcon;
    public IUserInput pi;  //获取控制脚本
    //public JoystickInput ji;
    public float walkSpeed = 2.0f;
    public float runSpeed = 2.0f;
    public float turnSpeed = 0.5f;
    public float jumpVelocity = 3.0f;
    public float rollVelocity = 1.0f;
    public float jabVelocity = 10.0f;

    [Header("===== Friction Settings =====")]
    public PhysicMaterial f0;
    public PhysicMaterial f1;

    private Animator anim;
    private Rigidbody rb;
    public Vector3 planeVec;
    private bool lockPlane = false;
    private bool trackDir = false;
    private Vector3 thrustVec;
    private bool canAttack;  //解决跳跃中可以攻击的bug
    private CapsuleCollider col;  //解决摩擦力问题
    //private float lerpTarget;  //让attack图层过渡自然
    private Vector3 deltaPos;  //control root motion

    void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();  //解决多输入适配问题 /* 重点！！ */
        foreach (var input in inputs)
        {
            if (input.enabled == true)
            {
                pi = input;
                break;
            }
        }

        anim = model.GetComponent<Animator>();
        //ji = GetComponent<JoystickInput>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()  //60 times per second
    {
        if (pi.quit)
        {
            //Debug.Log("quit");
            Application.Quit();
        }

        if (pi.lockon)
        {
            camcon.LockUnlock();
        }
        //if (pi.Jright == 1)  //切换锁定目标
        //{
        //    camcon.SwitchLockon();
        //}

        if (camcon.lockState == false){
            anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), (pi.run) ? 2.0f : 1.0f, 0.1f));  //用脚本控制动画，用lerp来使跑步动画过渡自然
            anim.SetFloat("right", 0);
        }
        else
        {
            anim.SetFloat("forward", pi.Dvec.z*((pi.run) ? 2.0f : 1.0f));
            anim.SetFloat("right", pi.Dvec.x*((pi.run) ? 2.0f : 1.0f));
            if(pi.Jright != 0)
            {
                
            }
        }

        if (pi.jump == true)  //控制跳跃
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        if (pi.roll || rb.velocity.magnitude > 6f)  //roll or jab?
        {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        if (pi.attack && (CheckState("ground") || CheckStateTag("attack")) && canAttack)  //控制攻击
        {
            anim.SetBool("heavy", false);
            anim.SetTrigger("attack");
        }
        if (pi.heavyAttack && (CheckState("ground") || CheckStateTag("attack")) && canAttack)  //控制攻击
        {
            anim.SetBool("heavy", true);
            anim.SetTrigger("attack");
        }

        if (CheckState("ground") || CheckState("impact", "Defense"))
        {
            if (pi.defense){
                anim.SetBool("defense", true);
                anim.SetLayerWeight(anim.GetLayerIndex("Defense"), 1);
                canAttack = false;
            }
            else
            {
                anim.SetBool("defense", false);
                anim.SetLayerWeight(anim.GetLayerIndex("Defense"), 0);
                canAttack = true;
            }
        }
        

        if (camcon.lockState == false)
        {
            if (pi.Dmag > 0.01f)
            {
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, turnSpeed);  //控制人物的移动方向和旋转速度 /*精髓！！*/
            }
            if (lockPlane == false)
            {
                planeVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run) ? runSpeed : 1.0f);  //人物移动的方向
            }
        }
        else
        {
            if (trackDir == false)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planeVec.normalized;
            }

            if (lockPlane == false)
            {
                planeVec = pi.Dvec * walkSpeed * ((pi.run) ? runSpeed : 1.0f);
            }
        }        
    }

    void FixedUpdate()  //50 times per second
    {
        //rb.position += planeVec * Time.fixedDeltaTime;  //让物体进行位移
        rb.position += deltaPos;  //让碰撞体先位移root motion的偏移量
        rb.velocity = new Vector3(planeVec.x, rb.velocity.y, planeVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    public bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    public bool CheckStateTag(string tagName, string layerName = "Base Layer")  //判断动画的Tag
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsTag(tagName);
    }

    /// <summary>
    /// Message processing block
    /// </summary>
    public void OnJumpEnter()
    {
        pi.inputEnabled = false;
        lockPlane = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);  //增加跳跃的向上向量
        trackDir = true;
    }

    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }

    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlane = false;
        canAttack = true;
        col.material = f1;
        trackDir = false;
    }

    public void OnGroundExit()
    {
        col.material = f0;
    }

    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlane = true;
    }

    public void OnRollEnter()
    {
        pi.inputEnabled = false;
        lockPlane = true;
        thrustVec = new Vector3(0, rollVelocity, 0);
        trackDir = true;
    }

    public void OnJabEnter()
    {
        pi.inputEnabled = false;
        lockPlane = true;
    }

    public void OnJabUpdate() 
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity") * jabVelocity;  //增加后跳的速度
    }

    public void OnAttackAEnter()
    {
        pi.inputEnabled = false;
    }

    public void OnAttackAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attackAVelocity");
        //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("Attack"));
        //currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.1f);
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack"), currentWeight);
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.3f));  //把攻击图层的权重设为1.0f
    }

    public void OnAttackExit()  //修复攻击到一半被打后武器仍有效的bug
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnUpdateRootMotion(object _deltaPos)
    {
        if (CheckState("attackA") || CheckState("attackB") || CheckState("attackC"))  //只有攻击动画的时候才记录偏移量
        {
            deltaPos += 0.2f * deltaPos + 0.8f * (Vector3)_deltaPos;  //解决动画导致镜头晃动问题
        }
    }

    public void OnHitEnter()
    {
        pi.inputEnabled = false;
        planeVec = Vector3.zero;  //挨打时要停下来
        model.SendMessage("WeaponDisable");
    }

    public void OnDieEnter()
    {
        pi.inputEnabled = false;
        planeVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }

    public void OnImpactEnter()
    {
        pi.inputEnabled = false;
    }

    public void OnImpactExit()
    {
        pi.inputEnabled = true;
    }

    public void IssueTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
}
