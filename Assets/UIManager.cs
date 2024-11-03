using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Start")]
    [SerializeField] TMP_Text startText;

    [Header("Pass Panel")]
    [SerializeField] GameObject passPanel;
    [SerializeField] Image[] passStatusImages;
    [SerializeField] Image passButtonImage;
    [SerializeField] Color[] playerColors;
    [SerializeField] float passButtonAnimationTime = .2f;

    [Header("Turn Info")]
    [SerializeField] GameObject turnStatusPanel;
    [SerializeField] TMP_Text[] turnStatusText;

    GoGame game;

    float playerColorAnimationFrame = 0;
    Color playerColorAnimationStart, playerColorAnimationGoal;

    void Start()
    {
        game = FindObjectOfType<GoGame>();

        passPanel.SetActive(false);
        turnStatusPanel.SetActive(false);
    }

    void Update()
    {
        if (playerColorAnimationFrame < passButtonAnimationTime)
        {
            playerColorAnimationFrame += Time.deltaTime;

            Color c = playerColorAnimationFrame >= passButtonAnimationTime ? playerColorAnimationGoal : 
                Color.Lerp(playerColorAnimationStart, playerColorAnimationGoal, playerColorAnimationFrame / passButtonAnimationTime);

            // Pass button
            passButtonImage.color = c;

            // Turn status
            turnStatusText[1].color = c;
        }
    }

    public void NewTurn(int player)
    {
        // Player color animation parameters
        playerColorAnimationFrame = 0;
        playerColorAnimationStart = passButtonImage.color;
        playerColorAnimationGoal = playerColors[player - 1];

        // Update pass status
        {
            for (int i = 0; i < passStatusImages.Length; i++)
            {
                Color c = passStatusImages[i].color;
                bool passed;
                switch (i + 1)
                {
                    case 1:
                        passed = game.player1Passed;
                        break;
                    case 2:
                        passed = game.player2Passed;
                        break;
                    default:
                        passed = game.player3Passed;
                        break;
                }
                c.a = passed ? 1f : 0.5f;
                passStatusImages[i].color = c;
            }
        }

        // Update turn status
        {
            foreach (TMP_Text t in turnStatusText)
                t.text = $"Player {player}'s Turn";
        }
    }

    public void StartGame()
    {
        if (game.gameStarted)
            return;

        game.StartGame();

        startText.enabled = false;
        passPanel.SetActive(true);
        turnStatusPanel.SetActive(true);
        NewTurn(1);
    }

    public void PlayerPassed()
    {
        game.PassPlayer();
    }
}
