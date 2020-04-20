using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorCloud : MonoBehaviour
{
    public GameObject[] assets;

    public float EmptyRadius;
    public int CloudRadius;
    public int DistanceBetween;
    public float RandomizePosition;
    public float ChanceToSpawn;
    public float MaxSpeed;
    public Vector2 RandomizeSize;

    public Vector3 velocity;
    public float drag;

    List<Culling> meteors;

    // Start is called before the first frame update
    void Start()
    {

        Culling.chunkSize = CloudRadius * DistanceBetween;
        Culling.doubleChunk = Culling.chunkSize * 2;

        meteors = new List<Culling>();

        for (int i = -CloudRadius; i < CloudRadius; i++) {
            for (int j = -CloudRadius; j < CloudRadius; j++) {
                for (int k = -CloudRadius; k < CloudRadius; k++) {

                    float radius = Mathf.Sqrt(i * i + k * k + j * j);
                    if (radius < EmptyRadius) continue;

                    if (Random.value > ChanceToSpawn) continue;

                    GameObject meteor = assets[Random.Range(0, assets.Length)];
                    meteor = Instantiate(meteor, transform);

                    meteor.transform.position = new Vector3(i, j, k) * DistanceBetween + new Vector3(Random.value, Random.value, Random.value) * RandomizePosition;

                    float scale = Mathf.Lerp(RandomizeSize.x, RandomizeSize.y, Random.value);
                    meteor.transform.localScale += Vector3.one * scale;
                    meteor.GetComponent<Rigidbody>().mass *= scale;

                    meteor.transform.rotation = Random.rotation;
                    Culling culling = meteor.GetComponent<Culling>();
                    culling.Start();
                    meteors.Add(culling);
                }
            }
        }

        StartCoroutine(MoveChildren());
    }

    void FixedUpdate() {
        transform.position += velocity * Time.deltaTime;

        velocity = Vector3.MoveTowards(velocity, Vector3.zero, velocity.magnitude * drag * Time.deltaTime);
    }

    IEnumerator MoveChildren() {
        while (true) {
            yield return new WaitForSeconds(0.1f);
            foreach (Culling child in meteors) {
                child.CullingUpdate();
            }
        }
    }

    public void AddForce(Vector3 force, ForceMode mode) {
        velocity -= force;
    }

}

