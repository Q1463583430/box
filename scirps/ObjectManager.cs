using JetBrains.Annotations;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public PoolManager poolManager;              // 引用 PoolManager
    public GameObject[] prefabs;                 // 预设物体数组
    public int currentPrefabIndex = 0;          // 当前选择的预设索引
    public GameObject currentPreview;           // 当前预览物体
    public Material transparentMaterial;        // 透明材质
    public Material normalMaterial;             // 正常材质

    public LineRenderer previewLine;            // 用于绘制预览线
    public float previewDistance = 3.0f;        // 预览物体与平台的高度差
    public LayerMask platformLayer;             // 平台的层，用于射线检测

    private GameModeManager gameModeManager;    // 引用 GameModeManager
    private Camera mainCamera;                  // 主摄像机
    public Vector3 mousePosition;
    // 自由模式相关
    private GameObject selectedObject = null;   // 当前选中的物体
    private Rigidbody selectedRigidbody = null; // 选中物体的刚体
    public float dragForce = 3f;                // 施加力的大小
    public LayerMask interactableLayer;             // 施加力的大小
    void Start()
    {
        gameModeManager = FindObjectOfType<GameModeManager>();
        mainCamera = Camera.main;
        if (gameModeManager.currentMode == GameModeManager.GameMode.CreateMode)
        {
            CreatePreviewObject(currentPrefabIndex);
        }
        // 初始化预览物体
        CreatePreviewObject(currentPrefabIndex);

        // 初始化预览线
        if (previewLine != null)
        {
            previewLine.positionCount = 2;
            
            previewLine.material = new Material(Shader.Find("Sprites/Default"));
            previewLine.startColor = Color.red; // 正常红色
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
     
        // 鼠标左键点击放置物体
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }

        //  右键删除物体
        if (Input.GetMouseButtonDown(1))
        {
            DeletePlacedObject();
        }

        // 鼠标滚轮切换预览物体
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
        // 如果预览物体已经存在，隐藏它
        if (currentPreview != null)
        {
            currentPreview.SetActive(false);
        }

        // 如果需要新的预览体
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
            if (collider != null) collider.enabled = false; // 禁用碰撞器
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

    void UpdatePreviewPosition()//更新预览位置
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

    void UpdatePreviewLine(Vector3 hitPoint)//更新预览线位置
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
    //自由模式
    void HandleFreeMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();//左键选择施加力
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

      public void ClearPreviewObject()//清楚预览物体，切换自由模式时启用
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

