using UnityEngine;

public class TestMover : MonoBehaviour
{
    public float speed = 2;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + Vector3.forward * speed * Time.deltaTime;
    }
}
