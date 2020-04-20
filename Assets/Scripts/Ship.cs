using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    public float forceMultiplier = 10;
    public float rotateMultiplier = 10;
    public float rateMoveSpeed = 1;
    public float damageMultiplier;

    public float Air { get; set; }
    public float MaxAir;
    public float Hull { get; set; }
    public float MaxHull;
    public float Energy { get; set; }
    public float MaxEnergy;
    public float Fuel { get; set; }
    public float MaxFuel;

    public new MeteorCloud cloud;
    public Rigidbody rigidbody;

    public Panel[] panels;
    public Light issueLight;
    public Light[] lights;
    public TextMesh issueText;

    public AudioClip fixSound, brokeSound, hitSound, explosionSound;
    public AudioSource sirenSource, engineSource;

    public bool InControl = false, DamagedControl = false, AirDamage = false, FuelDamage = false, EnergyDamage = false;

    // Start is called before the first frame update
    void Start() {
        Air = MaxAir;
        Hull = MaxHull;
        Energy = MaxEnergy;
        Fuel = MaxFuel;
        rigidbody = GetComponent<Rigidbody>();
        issueLight.gameObject.SetActive(false);
    }

    float hRate, vRate, aRate, sRate;

    public void Update() {

        if (inCollision.Count > 0 && Hull > 0) {
            Hull -= inCollision.Count * cloud.velocity.magnitude * damageMultiplier * Time.deltaTime;
        }

        if (AirDamage) Air -= Time.deltaTime;
        if (FuelDamage) Fuel -= Time.deltaTime * 10f;

        if (Hull < 0) {
            Explode();
        }
    }


    public void FixedUpdate() {

        if (InControl && !DamagedControl && Fuel > 0)
            Move();
        else
            engineSource.Stop();

        if (cloud.transform.position.z > 100000) {
            GetComponentInChildren<Controller>().EndGameWin();
        }
    }

    public void Explode() {
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        rigidbody.AddExplosionForce(800000000, transform.position - Vector3.one, 1000);
    }


    public void Move() {

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float a = Input.GetAxis("Ascending");
            float s = Input.GetAxis("Siding");

            hRate = Mathf.MoveTowards(hRate, h, Time.deltaTime * rateMoveSpeed);
            vRate = Mathf.MoveTowards(vRate, v, Time.deltaTime * rateMoveSpeed);
            aRate = Mathf.MoveTowards(aRate, a, Time.deltaTime * rateMoveSpeed);
            sRate = Mathf.MoveTowards(sRate, s, Time.deltaTime * rateMoveSpeed);

            Fuel -= Mathf.Abs(hRate) + Mathf.Abs(vRate) + Mathf.Abs(aRate) + Mathf.Abs(sRate);

            bool play = false;

            if (Mathf.Abs(vRate) > 0.01) {
                play = true;
                cloud.AddForce(transform.up * forceMultiplier * vRate * Time.deltaTime, ForceMode.Acceleration);
            }
            if (Mathf.Abs(hRate) > 0.01) {
                play = true;
                cloud.AddForce(transform.right * forceMultiplier * -hRate * Time.deltaTime, ForceMode.Acceleration);
            }
            if (Mathf.Abs(sRate) > 0.01) {
                play = true;
                rigidbody.AddTorque(transform.forward * rotateMultiplier * sRate * Time.deltaTime, ForceMode.Acceleration);
            }
            if (Mathf.Abs(aRate) > 0.01) {
                play = true;
                rigidbody.AddTorque(transform.right * rotateMultiplier * -aRate * Time.deltaTime, ForceMode.Acceleration);
            }

            if (play && !engineSource.isPlaying)
                engineSource.Play();
            else
                engineSource.Stop();
    }

    HashSet<GameObject> inCollision = new HashSet<GameObject>();

    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == 11) {// LayerMask.NameToLayer("SpaceObject")) ;
            Hull -= collision.relativeVelocity.magnitude * damageMultiplier;
            inCollision.Add(collision.gameObject);

            RandomDamage();
        }
    }

    HashSet<Panel> brokenPanels = new HashSet<Panel>();

    public void RandomDamage() {
        AudioSource.PlayClipAtPoint(hitSound, issueText.transform.position);
        //int var = Random.Range(-1, panels.Length);
        int var = Random.Range(0, panels.Length);
        if (var >= 0) {
            Panel panel = panels[var];
            issueText.text = panel.issueText;
            issueLight.gameObject.SetActive(true);
            panel.RaiseIssue();

            if (panel.damageAir) AirDamage = true;
            if (panel.damageFuel) FuelDamage = true;
            if (panel.damageNavigation) DamagedControl = true;
            if (panel.damageEnergy) {
                EnergyDamage = true;
                DamagedControl = true;
                SetLightsActive(false);
            }

            AudioSource.PlayClipAtPoint(brokeSound, panel.transform.position);
            brokenPanels.Add(panel);
            if (!sirenSource.isPlaying)
                sirenSource.Play();
        }
    }

    public void Fix(Panel panel) {
        AudioSource.PlayClipAtPoint(fixSound, issueText.transform.position);
        if (panel.damageAir) AirDamage = false;
        if (panel.damageFuel) FuelDamage = false;
        if (panel.damageNavigation) DamagedControl = false;
        if (panel.damageEnergy) {
            EnergyDamage = false;
            DamagedControl = false;
            SetLightsActive(true);
        }

        brokenPanels.Remove(panel);
        if (brokenPanels.Count == 0) {
            issueText.text = "";
            issueLight.gameObject.SetActive(false);
            sirenSource.Stop();
        } else {
            IEnumerator<Panel> i = brokenPanels.GetEnumerator();
            i.MoveNext();
            issueText.text = i.Current.issueText;
        }
    }

    public void OnCollisionExit(Collision collision) {
        inCollision.Remove(collision.gameObject);
    }

    public void SetLightsActive(bool value) {
        foreach (Light light in lights) {
            light.enabled = value;
        }
    }
}
