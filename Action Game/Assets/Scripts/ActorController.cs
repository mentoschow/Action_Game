using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;  //��ȡģ�����
    public CameraController camcon;
    public IUserInput pi;  //��ȡ���ƽű�
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
    private bool canAttack;  //�����Ծ�п��Թ�����bug
    private CapsuleCollider col;  //���Ħ��������
    //private float lerpTarget;  //��attackͼ�������Ȼ
    private Vector3 deltaPos;  //control root motion

    void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();  //����������������� /* �ص㣡�� */
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
        //if (pi.Jright == 1)  //�л�����Ŀ��
        //{
        //    camcon.SwitchLockon();
        //}

        if (camcon.lockState == false){
            anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), (pi.run) ? 2.0f : 1.0f, 0.1f));  //�ýű����ƶ�������lerp��ʹ�ܲ�����������Ȼ
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

        if (pi.jump == true)  //������Ծ
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        if (pi.roll || rb.velocity.magnitude > 6f)  //roll or jab?
        {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        if (pi.attack && (CheckState("ground") || CheckStateTag("attack")) && canAttack)  //���ƹ���
        {
            anim.SetBool("heavy", false);
            anim.SetTrigger("attack");
        }
        if (pi.heavyAttack && (CheckState("ground") || CheckStateTag("attack")) && canAttack)  //���ƹ���
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
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, turnSpeed);  //����������ƶ��������ת�ٶ� /*���裡��*/
            }
            if (lockPlane == false)
            {
                planeVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run) ? runSpeed : 1.0f);  //�����ƶ��ķ���
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
        //rb.position += planeVec * Time.fixedDeltaTime;  //���������λ��
        rb.position += deltaPos;  //����ײ����λ��root motion��ƫ����
        rb.velocity = new Vector3(planeVec.x, rb.velocity.y, planeVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    public bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    public bool CheckStateTag(string tagName, string layerName = "Base Layer")  //�ж϶�����Tag
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
        thrustVec = new Vector3(0, jumpVelocity, 0);  //������Ծ����������
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
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity") * jabVelocity;  //���Ӻ������ٶ�
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
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.3f));  //�ѹ���ͼ���Ȩ����Ϊ1.0f
    }

    public void OnAttackExit()  //�޸�������һ�뱻�����������Ч��bug
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnUpdateRootMotion(object _deltaPos)
    {
        if (CheckState("attackA") || CheckState("attackB") || CheckState("attackC"))  //ֻ�й���������ʱ��ż�¼ƫ����
        {
            deltaPos += 0.2f * deltaPos + 0.8f * (Vector3)_deltaPos;  //����������¾�ͷ�ζ�����
        }
    }

    public void OnHitEnter()
    {
        pi.inputEnabled = false;
        planeVec = Vector3.zero;  //����ʱҪͣ����
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
