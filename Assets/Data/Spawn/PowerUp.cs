using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PowerUpType
{
    None,
    HeightJump,
    bonusMana
}
public class PowerUp : MonoBehaviour
{
    public PowerUpType PowerType;
}
