using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class Controller : MonoBehaviour
{
    RigidbodyFirstPersonController mainController;
    new ObjectSearcher camera;
    new Rigidbody rigidbody;
    new Collider collider;

    public GameObject selected;
    public Ship ship;
    public Animator eyesAnimator;

    Position pos;
    Panel panel;

    bool lockUseButton = false;
    Vector3 initialCameraPosition;

    public AudioClip startFix, hitSound;

    public float gravityMagnitude;

    int gravityType = 0;

    void Start() {
        mainController = GetComponent<RigidbodyFirstPersonController>();
        camera = GetComponentInChildren<ObjectSearcher>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        initialCameraPosition = camera.transform.localPosition;
        eyesAnimator.Play("EyesOpen");
    }

    void Update() {

        if (ship.Air < 0 || (transform.position - ship.transform.position).magnitude > 30f) {
            DieFromAirLoss();
        }

        if (selected == null && !lockUseButton && Input.GetButton("Use") && camera.selectedObject != null) {

            selected = camera.selectedObject;
            pos = selected.GetComponent<Position>();
            panel = selected.GetComponent<Panel>();
            selected.layer = LayerMask.NameToLayer("Selected");

            if (pos != null) {
                camera.transform.parent = pos.transform;
                camera.transform.position = pos.CameraPosition;
                camera.transform.forward = pos.transform.forward;

                rigidbody.isKinematic = true;
                mainController.enabled = collider.enabled = false;
                mainController.mouseLook.enableVertical = true;
                mainController.mouseLook.DropCameraTarget();

                if (pos.type == Position.Type.HELM) {
                    ship.InControl = true;
                }
            }

            if (panel != null) {
                AudioSource.PlayClipAtPoint(startFix, transform.position);
                panel.Use();
            }

            lockUseButton = true;
        }
        
        if (selected) {
            if (!lockUseButton && Input.GetButton("Use")) {
                selected.layer = LayerMask.NameToLayer("Selectable");
                selected = null;
                mainController.enabled = true;
                lockUseButton = true;

                if (pos != null) {
                    camera.transform.parent = this.transform;
                    camera.transform.localPosition = initialCameraPosition;
                    camera.transform.localRotation = Quaternion.identity;
                    
                    mainController.enabled = collider.enabled = true;
                    mainController.mouseLook.enableVertical = false;
                    mainController.mouseLook.DropCameraTarget();
                    rigidbody.isKinematic = false;

                    if (pos.type == Position.Type.HELM) {
                        ship.InControl = false;
                    }

                    pos = null;

                }

                if (panel != null) {
                    panel.CancelUse();
                    panel = null;
                }
            }
        } else {
            if (Input.GetKey(KeyCode.Alpha1)) {
                gravityType = 0;
                SetNewGravity(-ship.transform.forward, ship.transform.right);
            } else if (Input.GetKey(KeyCode.Alpha2)) {
                gravityType = 1;
                SetNewGravity(ship.transform.forward, ship.transform.right);
            } else if (Input.GetKey(KeyCode.Alpha3)) {
                gravityType = 2;
                SetNewGravity(-ship.transform.up, ship.transform.right);
            } else if (Input.GetKey(KeyCode.Alpha4)) {
                gravityType = 3;
                SetNewGravity(ship.transform.up, ship.transform.right);
            } else if (Input.GetKey(KeyCode.Alpha5)) {
                gravityType = 4;
                SetNewGravity(-ship.transform.right, ship.transform.up);
            } else if (Input.GetKey(KeyCode.Alpha6)) {
                gravityType = 5;
                SetNewGravity(ship.transform.right, ship.transform.up);
            }
        }

        if (lockUseButton && !Input.GetButton("Use")) lockUseButton = false;
        if (Input.GetButton("Exit"))
            SceneManager.LoadScene("MenuScene");
    }


    public void EndGameWin() {
        StartCoroutine(EndGameWinImpl());
    }

    IEnumerator EndGameWinImpl() {
        yield return new WaitForSeconds(5f);
        eyesAnimator.Play("WinGame");
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MenuScene");
    }

    void FixedUpdate() {
        Vector3 direction = Vector3.zero;
        switch (gravityType) {
            case 0: direction = -ship.transform.forward; break;
            case 1: direction =  ship.transform.forward; break;
            case 2: direction = -ship.transform.up; break;
            case 3: direction =  ship.transform.up; break;
            case 4: direction = -ship.transform.right; break;
            case 5: direction =  ship.transform.right; break;
        }
        mainController.CurrentGravityDirection = direction;
        rigidbody.velocity += direction * gravityMagnitude * Time.deltaTime;
    }
    void SetNewGravity(Vector3 direction, Vector3 forward) {
        Quaternion oldRotation = transform.rotation;
        transform.rotation = Quaternion.LookRotation(forward, -direction);
        Quaternion newRotation = transform.localRotation;
        transform.rotation = oldRotation;
        mainController.mouseLook.SetNewCharacterRotation(newRotation);
    }

    void LateUpdate() {
        if (selected) {
            if (pos != null) {
                UpdateCameraAtPosition();
            } else {
                UpdateObject();
            }
        }
    }

    void UpdateCameraAtPosition() {
        mainController.mouseLook.LookRotation(transform, camera.transform);
    }

    void UpdateObject() {
        selected.transform.position = camera.transform.position + camera.transform.forward * 0.5f;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.relativeVelocity.magnitude > 5f) {
            eyesAnimator.Play("Damage");
        }
    }

    void DieFromAirLoss() {
        StartCoroutine(DieFromAirLossImpl());
    }

    IEnumerator DieFromAirLossImpl() {
        yield return new WaitForSeconds(5);
        eyesAnimator.Play("EyesClosed");

        AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources) {
            audioS.Stop();
        }

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("MenuScene");
    }
    
}
