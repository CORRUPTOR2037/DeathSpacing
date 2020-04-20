using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UntilDestinationCounter : MonoBehaviour
{
    public float EndValue;
    public GameObject watch;
    TextMesh text;

    void Start() {
        text = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Mathf.Max(0, (EndValue - watch.transform.position.z)).ToString();
    }
}
