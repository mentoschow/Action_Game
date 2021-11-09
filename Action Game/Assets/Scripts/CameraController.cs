using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private IUserInput pi;
    public float horizontalSpeed = 50.0f;
    public float verticalSpeed = 40.0f;
    public float camDampSpeed = 0.5f;
    public Image lockDot;
    public bool lockState;
    public float unlockPos;
    public bool isAI = false;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerX;
    private GameObject model;
    private GameObject cam;
    private Vector3 camDampVelocity;
    private LockTarget lockTarget;

    void Start()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        pi = ac.pi;
        model = ac.model;
        tempEulerX = 20f;
        if (!isAI)
        {
            cam = Camera.main.gameObject;
            lockDot.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        lockState = false;
        unlockPos = 8f;       
    }

    void Update()
    {
        if(lockTarget != null)
        {
            if (!isAI)
            {
                lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            }        
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > unlockPos)  //远离目标自动解锁
            {
                LockProcess(null, false, false, isAI);
            }

            if (lockTarget.am != null && lockTarget.am.sm.isDie)  //敌人死亡时自动解锁
            {
                LockProcess(null, false, false, isAI);
            }
        }
    }

    void FixedUpdate()
    {
        if (lockTarget == null)  //unlockon
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);  //横轴旋转

            tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 40);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;  //纵轴旋转
        }
        else  //lockon
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;  //新的camera前向量
            tempForward.y = 0;  //垂直方向不变
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform.position + Vector3.up * 0.4f);
        }

        if (!isAI)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, transform.position, ref camDampVelocity, camDampSpeed);
            //cam.transform.position = transform.position;
            //cam.transform.eulerAngles = transform.eulerAngles;  //会导致镜头摇晃，头晕
            cam.transform.LookAt(cameraHandle.transform);  //解决头晕  /* 重点！！ */ 但又导致翻滚时镜头晃动，因为翻滚时会向上位移
        }      
    }

    public void LockUnlock()
    {
        Vector3 modelOrigin = model.transform.position;  //camera的坐标
        Vector3 viewOrigin = modelOrigin + new Vector3(0, 1, 0);  //视角的坐标，在比camera高1米的地方
        Vector3 boxCenter = viewOrigin + model.transform.forward * 5f;  //碰撞box的中心坐标，在camera前方向5米的地方
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask(isAI ? "Player" : "Enemy"));  //计算与box发生碰撞的物件
        if (cols.Length == 0)
        {
            LockProcess(null, false, false, isAI);
        }
        else
        {
            foreach (var col in cols)
            {
                if (lockTarget != null && lockTarget.obj == col.gameObject)  //锁到同一个物体时解锁
                {
                    LockProcess(null, false, false, isAI);
                    break;
                }
                LockProcess(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }
    }

    public void SwitchLockon()  //切换锁定目标
    {
        GameObject nowLock = lockTarget.obj;
        Collider[] cols = Physics.OverlapBox(nowLock.transform.position, new Vector3(5, 5, 5), nowLock.transform.rotation, LayerMask.GetMask("Enemy"));
        if (cols.Length != 0)
        {
            foreach(var col in cols)
            {
                if (col.gameObject != nowLock)
                {
                    lockTarget = new LockTarget(col.gameObject, col.bounds.extents.y);
                }
            }
        }
    }

    private void LockProcess(LockTarget _lockTarget, bool _lockDotEnable, bool _lockState, bool _isAI)
    {
        if (!_isAI)
        {
            lockDot.enabled = _lockDotEnable;
        }
        lockTarget = _lockTarget;      
        lockState = _lockState;
    }

    private class LockTarget
    {
        public GameObject obj;
        public float halfHeight;
        public ActorManager am;

        public LockTarget(GameObject _obj, float _halfHeight)
        {
            obj = _obj;
            halfHeight = _halfHeight;
            am = _obj.GetComponent<ActorManager>();
        }
    }
}
