using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Playing, InMenu }
    public GameState StateOfGame;

    public float Gravity = 9.81f;

    private void Awake ( )
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}