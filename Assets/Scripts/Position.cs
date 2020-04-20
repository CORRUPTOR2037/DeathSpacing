using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public enum Type {
        WINDOW, HELM, PANEL
    }

    public Type type;

    [SerializeField]
    private Vector3 _CameraPosition;
    public Vector3 CameraPosition { get { return transform.rotation * _CameraPosition + transform.position;  } }
    public Vector2 Xclamp;
    public Vector2 Yclamp;

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(CameraPosition, 0.1f);
        Gizmos.DrawLine(CameraPosition, CameraPosition + transform.forward / 5);
    }

}
