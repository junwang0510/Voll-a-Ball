using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMotion : MonoBehaviour
{
    private Vector3 normal;
    private Vector3 rot;
    private float speed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        rot = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        // update 5 degrees per frame
        rot += new Vector3(5 * speed * Mathf.Sin(Time.time),
                           5 * speed * Mathf.Cos(Time.time),
                           5 * speed * -Mathf.Sin(Time.time));
        transform.rotation = Quaternion.Euler(rot);
    }

    public void SetNormal(Vector3 normal)
    {
        this.normal = normal;
    }
}
