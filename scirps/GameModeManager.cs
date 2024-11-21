using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode { CreateMode, FreeMode }
    public GameMode currentMode = GameMode.CreateMode;

    public ObjectManager objectManager;          // 引用 ObjectManager
    public CameraController cameraController;    // 引用 CameraController

    void Update()
    {
        // 按下 M 键切换模式
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleGameMode();
        }

        // 在自由模式下，按下 B 键切换重力方向
        if (currentMode == GameMode.FreeMode && Input.GetKeyDown(KeyCode.B))
        {
            cameraController.ToggleGravityDirection();
        }
    }

    // 切换游戏模式
    void ToggleGameMode()
    {
        if (currentMode == GameMode.CreateMode)
        {
            currentMode = GameMode.FreeMode;
            objectManager.ClearPreviewObject();  // 进入自由模式时清除预览
            Debug.Log("切换到自由模式");//控制台打印
        }
        else
        {
            currentMode = GameMode.CreateMode;
            objectManager.CreatePreviewObject(objectManager.currentPrefabIndex);  // 进入创造模式时恢复预览
            Debug.Log("切换到创造模式");
        }
    }
}
