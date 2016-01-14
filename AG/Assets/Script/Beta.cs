using UnityEngine;
using System.Collections;
using Assets.Script;

public class Beta : MonoBehaviour {
    public float gravity = 20.0F;
    public float walkSpeed = 2.0F;
    public float runSpeed = 6.0F;

    // 摄像机
    public Transform camera;
    public float cameraDistanceAwary = 5;
    public float cameraSmooth = 2.0F;
    // 武器
    public GameObject weapon;

    // Beta 的速度（矢量）
    private Vector3 curSpeedV = Vector3.zero;
    private float curSpeed = 0.0F;
    // 状态机
    private Animator myAnimator;
    // 角色控制器
    private CharacterController controller;
    // Beta 的默认朝向（只能通过鼠标按住右键旋转）
    private Vector3 curForward;

    private ArmPose armPose = ArmPose.None;
    private bool hasWeapon = false;
    public bool isAttack = false;

    private float cameraRotationX = 30.0F;
    private float cameraRotationY = 0.0F;

    private

	void Start () {
        myAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        curForward = Vector3.forward;

        weapon.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        updateForWard();
        updateModeRotLoc();
        updateModeAction();
	}

    void LateUpdate() {
        // 调整镜头的坐标
        Vector3 cameraPosition = Quaternion.Euler(new Vector3(cameraRotationX,cameraRotationY,0)) * Vector3.back * cameraDistanceAwary + transform.position;
        camera.position = Vector3.Lerp(camera.position , cameraPosition,0.8F);
        // 调整镜头的朝向
        camera.LookAt(transform.position);
    }

    private void updateForWard() {
        if(Input.GetButton("Fire2")) {
            float mouseX = Input.GetAxis("Mouse X");
            // 轴的改变方向
            float mouseY = Input.GetAxis("Mouse Y");

            // 鼠标 Y 轴移动改变的是相机的 X 轴旋转
            cameraRotationX -= mouseY * cameraSmooth;
            cameraRotationX = Mathf.Min(cameraRotationX, 80);
            cameraRotationX = Mathf.Max(cameraRotationX, 10);
            // 鼠标 x 轴移动改变的是相机的 Y 轴旋转
            cameraRotationY += mouseX * cameraSmooth;

            curForward = Quaternion.Euler(new Vector3(0,mouseX * cameraSmooth, 0)) * curForward;
        }
    }

    private void updateModeAction() {
        if (Input.GetButtonUp("Weapon")) {
            hasWeapon = !hasWeapon;
        }

        isAttack = hasWeapon && Input.GetButton("Fire1");

        if (hasWeapon) {
            if (isAttack) {
                armPose = ArmPose.None;
            }
            else {
                armPose = ArmPose.HandleWeapon;
            }
        }
        else {
            armPose = ArmPose.None;
        }

        // 设置攻击状态
        myAnimator.SetBool("isAttack",isAttack);
        // 设置武器的显示
        weapon.SetActive(hasWeapon);
        // 设置手臂姿态
        myAnimator.SetInteger("ArmPose",(int)armPose);
    }

    private void updateModeRotLoc() {
        // 水平轴的改变方向
        float horizontal = Input.GetAxis("Horizontal");
        // 纵轴的改变方向
        float vertical = Input.GetAxis("Vertical");

        curSpeedV.x = horizontal * curSpeed;
        curSpeedV.z = vertical * curSpeed;
        curSpeedV.y = 0;
        bool isMove = horizontal != 0 || vertical != 0;
        myAnimator.SetBool("isMove",isMove);

        // 人物是接触地面的
        if (controller.isGrounded) {
            bool isRun = isMove && Input.GetButton("Run");
            if(isMove) {
                if(isRun){
                    curSpeed = Mathf.Lerp(curSpeed,runSpeed,0.3F);
                }
                else {
                    curSpeed = Mathf.Lerp(curSpeed,walkSpeed,0.3F);
                }
            }
            else{
                curSpeed = 0;
            }
            // 设置移动的速度
            myAnimator.SetFloat("Speed",curSpeed);
        }
        else {
            curSpeedV.y -= gravity;
        }

        controller.Move(Quaternion.LookRotation(curForward) * curSpeedV * Time.deltaTime);

        // 设置模型的 Rotation
        if (!myAnimator.applyRootMotion) {
            if(isMove) {
                Vector3 nowForward = Quaternion.LookRotation(new Vector3(curSpeedV.x,0,curSpeedV.z)) * curForward;
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(nowForward),0.2F);
            }
            else {
                if(!curForward.Equals(Vector3.zero)) {
                    transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(curForward),0.2F);
                }
            }
        }
    }
}
