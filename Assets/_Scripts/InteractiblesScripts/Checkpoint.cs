using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Interactable
{
    private Animator _playerAC;

    public override void Start ( )
    {
        _playerAC = GameObject.Find ("Player").GetComponent<Animator> ();
    }
}
