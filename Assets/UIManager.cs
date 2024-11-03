using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text startText;

    GoGame game;

    private void Start()
    {
        game = FindObjectOfType<GoGame>();
    }

    public void StartGame()
    {
        if (game.gameStarted)
            return;

        game.StartGame();
        startText.enabled = false;
    }
}
