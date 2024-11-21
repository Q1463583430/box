using UnityEngine;

public class Standable : MonoBehaviour
{
    private bool isStanding = true;  // 是否处于立场状态

    // 切换立场状态
    public void ToggleStand()
    {
        isStanding = !isStanding;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = isStanding;
            if (!isStanding)
            {
                rb.velocity = Vector3.zero;  // 停止物体的移动
            }
        }
    }
}
