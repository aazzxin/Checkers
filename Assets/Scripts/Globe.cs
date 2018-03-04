using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    OnePerson,
    TwoPerson
}

public class Globe : MonoBehaviour {

    public static GameMode gameMode = GameMode.TwoPerson;
}
