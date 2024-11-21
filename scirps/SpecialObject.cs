using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    public float forceFieldStrength = 10f;  // 力场强度，正值为排斥，负值为吸附
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);  // 检测范围5单位内的物体
        foreach (Collider collider in colliders)
        {  if (collider.attachedRigidbody != this)
            {
                if (collider.attachedRigidbody != null && collider.gameObject != gameObject)
                {
                    Vector3 direction = transform.position - collider.transform.position;
                    if (forceFieldStrength > 0)
                    {
                        rb.AddForce(direction.normalized * forceFieldStrength);  // 排斥
                    }
                    else
                    {
                        rb.AddForce(-direction.normalized * Mathf.Abs(forceFieldStrength));  // 吸附
                    }
                }
            }
        }
    }
}

