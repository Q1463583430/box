using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 10f;          // 缩放速度
    public float rotationSpeed = 50f;      // 旋转速度
    public Vector3 fixedPosition = new Vector3(0, 10, -10);  // 固定摄像机位置
    public float minZoom = 5f;             // 最小缩放
    public float maxZoom = 50f;            // 最大缩放

    public Camera mainCamera;               // 主摄像机

    private Vector3 currentGravityDirection = Vector3.down;  // 当前重力方向

    void Start()
    {
        mainCamera.transform.position = fixedPosition;
        mainCamera.transform.LookAt(Vector3.zero);
        Physics.gravity = currentGravityDirection;  // 设置初始重力
    }

    void Update()
    {
        // 通过上下箭头键控制摄像机的缩放
        if (Input.GetKey(KeyCode.UpArrow))
        {
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }

        // 通过 X 键绕 Y 轴旋转摄像机
        if (Input.GetKey(KeyCode.X))
        {
            mainCamera.transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    // 切换重力方向
    public void ToggleGravityDirection()
    {
        if (currentGravityDirection == Vector3.down)
        {
            currentGravityDirection = Vector3.up;
        }
        else
        {
            currentGravityDirection = Vector3.down;
        }

        Physics.gravity = currentGravityDirection;
    }
}
