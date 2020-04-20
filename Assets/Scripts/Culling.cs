using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culling : MonoBehaviour
{
    public static int chunkSize = 3000;
    public static int doubleChunk = chunkSize * 2;

    SphereCollider collider;
    Rigidbody body;
    GameObject child;
    static Camera cam;

    // Start is called before the first frame update
    public void Start()
    {
        child = transform.GetChild(0).gameObject;
        body = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
        cam = Camera.main;
    }

    bool atBack, farAway;
    Vector3 pos, camPos;

    bool a, b, c, d, e, f;

    // Update is called once per frame
    public void CullingUpdate()
    {
        pos = transform.position;
        camPos = cam.transform.position;

        while (pos.x - chunkSize > camPos.x)
            pos.x -= doubleChunk;
        while (pos.x + chunkSize < camPos.x)
            pos.x += doubleChunk;

        while (pos.y - chunkSize > camPos.y)
            pos.y -= doubleChunk;
        while (pos.y + chunkSize < camPos.y)
            pos.y += doubleChunk;

        while (pos.z - chunkSize > camPos.z && transform.localPosition.z - chunkSize > -105000)
            pos.z -= doubleChunk;
        while (pos.z + chunkSize < camPos.z)
            pos.z += doubleChunk;

        transform.position = pos;

        atBack = Vector3.Dot(cam.transform.forward, pos - camPos) > 0.1f;
        farAway = Vector3.Distance(camPos, pos) > 300 + collider.radius;

        body.isKinematic = farAway || atBack;
        collider.enabled = !farAway;
        if (atBack != child.gameObject.activeSelf)
            child.gameObject.SetActive(atBack);
    }
}
