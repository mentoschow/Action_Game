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
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > unlockPos)  //Զ��Ŀ���Զ�����
            {
                LockProcess(null, false, false, isAI);
            }

            if (lockTarget.am != null && lockTarget.am.sm.isDie)  //��������ʱ�Զ�����
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

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);  //������ת

            tempEulerX -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 40);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;  //������ת
        }
        else  //lockon
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;  //�µ�cameraǰ����
            tempForward.y = 0;  //��ֱ���򲻱�
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform.position + Vector3.up * 0.4f);
        }

        if (!isAI)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, transform.position, ref camDampVelocity, camDampSpeed);
            //cam.transform.position = transform.position;
            //cam.transform.eulerAngles = transform.eulerAngles;  //�ᵼ�¾�ͷҡ�Σ�ͷ��
            cam.transform.LookAt(cameraHandle.transform);  //���ͷ��  /* �ص㣡�� */ ���ֵ��·���ʱ��ͷ�ζ�����Ϊ����ʱ������λ��
        }      
    }

    public void LockUnlock()
    {
        Vector3 modelOrigin = model.transform.position;  //camera������
        Vector3 viewOrigin = modelOrigin + new Vector3(0, 1, 0);  //�ӽǵ����꣬�ڱ�camera��1�׵ĵط�
        Vector3 boxCenter = viewOrigin + model.transform.forward * 5f;  //��ײbox���������꣬��cameraǰ����5�׵ĵط�
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask(isAI ? "Player" : "Enemy"));  //������box������ײ�����
        if (cols.Length == 0)
        {
            LockProcess(null, false, false, isAI);
        }
        else
        {
            foreach (var col in cols)
            {
                if (lockTarget != null && lockTarget.obj == col.gameObject)  //����ͬһ������ʱ����
                {
                    LockProcess(null, false, false, isAI);
                    break;
                }
                LockProcess(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }
    }

    public void SwitchLockon()  //�л�����Ŀ��
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
