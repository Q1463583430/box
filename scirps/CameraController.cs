using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 10f;          // �����ٶ�
    public float rotationSpeed = 50f;      // ��ת�ٶ�
    public Vector3 fixedPosition = new Vector3(0, 10, -10);  // �̶������λ��
    public float minZoom = 5f;             // ��С����
    public float maxZoom = 50f;            // �������

    public Camera mainCamera;               // �������

    private Vector3 currentGravityDirection = Vector3.down;  // ��ǰ��������

    void Start()
    {
        mainCamera.transform.position = fixedPosition;
        mainCamera.transform.LookAt(Vector3.zero);
        Physics.gravity = currentGravityDirection;  // ���ó�ʼ����
    }

    void Update()
    {
        // ͨ�����¼�ͷ�����������������
        if (Input.GetKey(KeyCode.UpArrow))
        {
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }

        // ͨ�� X ���� Y ����ת�����
        if (Input.GetKey(KeyCode.X))
        {
            mainCamera.transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    // �л���������
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
