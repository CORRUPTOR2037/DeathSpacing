using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Panel : MonoBehaviour
{
    Ship ship;
    SpriteRenderer problemSprite;
    public float useTime;
    public UnityEvent onUse;
    [TextArea]
    public string issueText;

    public bool damageFuel = false;
    public bool damageAir = false;
    public bool damageNavigation = false;
    public bool damageEnergy = false;

    // Start is called before the first frame update
    void Start()
    {
        ship = GetComponentInParent<Ship>();
        problemSprite = GetComponentInChildren<SpriteRenderer>();
    }

    bool inUsage = false;
    bool hasProblem = false;

    public void Use()
    {
        if (!hasProblem) return;
        inUsage = true;
        StartCoroutine(UseImpl());
    }

    public void CancelUse() {
        inUsage = false;
    }

    public void RaiseIssue() {
        hasProblem = true;
        problemSprite.color = Color.red;
        problemSprite.transform.localScale = Vector3.one;
    }

    IEnumerator UseImpl() {
        while (problemSprite.transform.localScale.x > 0) {
            if (!hasProblem || !inUsage) yield break;
            problemSprite.transform.localScale -= new Vector3(Time.deltaTime / useTime, 0, 0);
            yield return new WaitForEndOfFrame();
        }
        problemSprite.color = Color.green;
        problemSprite.transform.localScale = Vector3.one;
        hasProblem = false;
        onUse.Invoke();
    }
}
