using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public enum GameMode { CreateMode, FreeMode }
    public GameMode currentMode = GameMode.CreateMode;

    public ObjectManager objectManager;          // ���� ObjectManager
    public CameraController cameraController;    // ���� CameraController

    void Update()
    {
        // ���� M ���л�ģʽ
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleGameMode();
        }

        // ������ģʽ�£����� B ���л���������
        if (currentMode == GameMode.FreeMode && Input.GetKeyDown(KeyCode.B))
        {
            cameraController.ToggleGravityDirection();
        }
    }

    // �л���Ϸģʽ
    void ToggleGameMode()
    {
        if (currentMode == GameMode.CreateMode)
        {
            currentMode = GameMode.FreeMode;
            objectManager.ClearPreviewObject();  // ��������ģʽʱ���Ԥ��
            Debug.Log("�л�������ģʽ");//����̨��ӡ
        }
        else
        {
            currentMode = GameMode.CreateMode;
            objectManager.CreatePreviewObject(objectManager.currentPrefabIndex);  // ���봴��ģʽʱ�ָ�Ԥ��
            Debug.Log("�л�������ģʽ");
        }
    }
}
