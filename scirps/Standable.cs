using UnityEngine;

public class Standable : MonoBehaviour
{
    private bool isStanding = true;  // �Ƿ�������״̬

    // �л�����״̬
    public void ToggleStand()
    {
        isStanding = !isStanding;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = isStanding;
            if (!isStanding)
            {
                rb.velocity = Vector3.zero;  // ֹͣ������ƶ�
            }
        }
    }
}
