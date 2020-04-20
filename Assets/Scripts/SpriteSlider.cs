using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSlider : MonoBehaviour
{
    SpriteRenderer sprite;
    float Value;
    float MaxValue;

    Ship ship;

    public enum Type {
        HULL, AIR, ENERGY, FUEL
    }
    public Type type;

    void Start() {
        ship = GetComponentInParent<Ship>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        switch (type) {
            case Type.AIR: {
                Value = ship.Air;
                MaxValue = ship.MaxAir;
            } break;
            case Type.HULL: {
                Value = ship.Hull;
                MaxValue = ship.MaxHull;
            }
            break;
            case Type.ENERGY: {
                if (ship.EnergyDamage) {
                    sprite.color = Color.red;
                } else {
                    sprite.color = Color.blue;
                }
                return;
            }
            break;
            case Type.FUEL: {
                Value = ship.Fuel;
                MaxValue = ship.MaxFuel;
            }
            break;
        }
        if (Value <= 0) {
            gameObject.SetActive(false);
        } else {
            transform.localScale = new Vector3(0.18f * Value / MaxValue, 0.1f, 0.1f);
        }
    }
}
