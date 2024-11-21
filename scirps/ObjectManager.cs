using JetBrains.Annotations;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public PoolManager poolManager;              // ���� PoolManager
    public GameObject[] prefabs;                 // Ԥ����������
    public int currentPrefabIndex = 0;          // ��ǰѡ���Ԥ������
    public GameObject currentPreview;           // ��ǰԤ������
    public Material transparentMaterial;        // ͸������
    public Material normalMaterial;             // ��������

    public LineRenderer previewLine;            // ���ڻ���Ԥ����
    public float previewDistance = 3.0f;        // Ԥ��������ƽ̨�ĸ߶Ȳ�
    public LayerMask platformLayer;             // ƽ̨�Ĳ㣬�������߼��

    private GameModeManager gameModeManager;    // ���� GameModeManager
    private Camera mainCamera;                  // �������
    public Vector3 mousePosition;
    // ����ģʽ���
    private GameObject selectedObject = null;   // ��ǰѡ�е�����
    private Rigidbody selectedRigidbody = null; // ѡ������ĸ���
    public float dragForce = 3f;                // ʩ�����Ĵ�С
    public LayerMask interactableLayer;             // ʩ�����Ĵ�С
    void Start()
    {
        gameModeManager = FindObjectOfType<GameModeManager>();
        mainCamera = Camera.main;
        if (gameModeManager.currentMode == GameModeManager.GameMode.CreateMode)
        {
            CreatePreviewObject(currentPrefabIndex);
        }
        // ��ʼ��Ԥ������
        CreatePreviewObject(currentPrefabIndex);

        // ��ʼ��Ԥ����
        if (previewLine != null)
        {
            previewLine.positionCount = 2;
            
            previewLine.material = new Material(Shader.Find("Sprites/Default"));
            previewLine.startColor = Color.red; // ������ɫ
            previewLine.endColor = Color.red;
            previewLine.widthMultiplier = 0.05f;
            previewLine.enabled = true;
        }
    }

    void Update()
    {
        if (gameModeManager == null)
        {
            gameModeManager = FindObjectOfType<GameModeManager>();
            if (gameModeManager == null) return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mousePosition = hit.point;
        }
        DeletePlacedObject();
        UpdatePreviewPosition();
        if (gameModeManager.currentMode == GameModeManager.GameMode.CreateMode)
        {
            HandleCreateMode();
        }
        else if (gameModeManager.currentMode == GameModeManager.GameMode.FreeMode)
        {
            HandleFreeMode();
        }
    }


    #region Create Mode Methods

    void HandleCreateMode()
    {
     
        // �����������������
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }

        //  �Ҽ�ɾ������
        if (Input.GetMouseButtonDown(1))
        {
            DeletePlacedObject();
        }

        // �������л�Ԥ������
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            SwitchToNextPreviewObject(Input.GetAxis("Mouse ScrollWheel") > 0 ? 1 : -1);
        }
    }

    public void SwitchToNextPreviewObject(int direction)
    {
        currentPrefabIndex = (currentPrefabIndex + direction + prefabs.Length) % prefabs.Length;
        CreatePreviewObject(currentPrefabIndex);
    }

    public void CreatePreviewObject(int prefabIndex)
    {
        // ���Ԥ�������Ѿ����ڣ�������
        if (currentPreview != null)
        {
            currentPreview.SetActive(false);
        }

        // �����Ҫ�µ�Ԥ����
        if (currentPreview == null || currentPreview.name != prefabs[prefabIndex].name)
        {
            currentPreview = Instantiate(prefabs[prefabIndex]);
            currentPreview.name = prefabs[prefabIndex].name;

            Renderer renderer = currentPreview.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = transparentMaterial;
            }

            Collider collider = currentPreview.GetComponent<Collider>();
            if (collider != null) collider.enabled = false; // ������ײ��
        }

        currentPreview.SetActive(true);
    }

    public void PlaceObject()
    {
        if (currentPreview == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, platformLayer))
        {
            Vector3 placementPosition = hit.point + Vector3.up * previewDistance;

            GameObject placedObject = poolManager.GetObjectFromPool(prefabs[currentPrefabIndex].name);
            if (placedObject != null)
            {
                placedObject.transform.position = placementPosition;
                placedObject.SetActive(true);

                Renderer renderer = placedObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = normalMaterial;
                }

                Collider collider = placedObject.GetComponent<Collider>();
                if (collider != null) collider.enabled = true;

                placedObject.tag = "PlacedObject";
            }
        }
    }

    public void DeletePlacedObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, platformLayer))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj.CompareTag("PlacedObject"))
            {
                poolManager.ReturnObjectToPool(obj, obj.name);
            }
        }
    }

    void UpdatePreviewPosition()//����Ԥ��λ��
    {
        if (currentPreview == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, platformLayer))
        {
            Vector3 newPosition = hit.point + Vector3.up * previewDistance;
            currentPreview.transform.position = newPosition;

            UpdatePreviewLine(hit.point);
        }
    }

    void UpdatePreviewLine(Vector3 hitPoint)//����Ԥ����λ��
    {
        if (previewLine == null || currentPreview == null) return;

       // Debug.Log("preview:" + currentPreview.transform.position);
        Vector3 startPoint = currentPreview.transform.position;
        Vector3 endPoint = hitPoint;
       // Debug.Log(startPoint + " " + endPoint);
       // Debug.DrawLine(startPoint, endPoint);
        previewLine.SetPosition(0, startPoint);
        previewLine.SetPosition(1, endPoint);
    }
    #endregion

    #region Free Mode Methods
    //����ģʽ
    void HandleFreeMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();//���ѡ��ʩ����
        }

        if (Input.GetMouseButton(0) && selectedRigidbody != null)
        {
            ApplyForceToSelectedObject();
        }

        if (Input.GetMouseButtonUp(0) && selectedRigidbody != null)
        {
            ReleaseObject();
        }


        void SelectObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
            {
                GameObject obj = hit.collider.gameObject;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    selectedObject = obj;
                    selectedRigidbody = rb;
                }
            }
        }

        void ApplyForceToSelectedObject()
        {
            if (selectedRigidbody != null)
            {
                selectedRigidbody.AddForce(mainCamera.transform.forward * dragForce, ForceMode.Acceleration);
            }
        }

        void ReleaseObject()
        {
            selectedObject = null;
            selectedRigidbody = null;
        }

        #endregion
    }
        #region Utility Methods

      public void ClearPreviewObject()//���Ԥ�����壬�л�����ģʽʱ����
        {
            if (currentPreview != null)
            {
                Destroy(currentPreview);
                currentPreview = null;
            }

            if (previewLine != null)
            {
                previewLine.enabled = false;
            }
        }
        #endregion
    }

