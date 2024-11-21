using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    public float forceFieldStrength = 10f;  // ����ǿ�ȣ���ֵΪ�ų⣬��ֵΪ����
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);  // ��ⷶΧ5��λ�ڵ�����
        foreach (Collider collider in colliders)
        {  if (collider.attachedRigidbody != this)
            {
                if (collider.attachedRigidbody != null && collider.gameObject != gameObject)
                {
                    Vector3 direction = transform.position - collider.transform.position;
                    if (forceFieldStrength > 0)
                    {
                        rb.AddForce(direction.normalized * forceFieldStrength);  // �ų�
                    }
                    else
                    {
                        rb.AddForce(-direction.normalized * Mathf.Abs(forceFieldStrength));  // ����
                    }
                }
            }
        }
    }
}

