using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text startText;
    [SerializeField] GameObject passPanel;
    [SerializeField] Image[] passStatusImages;
    [SerializeField] Image passButtonImage;
    [SerializeField] Color[] playerColors;
    [SerializeField] float passButtonAnimationTime = .2f;

    GoGame game;

    float passButtonAnimationFrame = 0;
    Color passButtonAnimationStart, passButtonAnimationGoal;

    void Start()
    {
        game = FindObjectOfType<GoGame>();

        passPanel.SetActive(false);
    }

    void Update()
    {
        if (passButtonAnimationFrame < passButtonAnimationTime)
        {
            passButtonAnimationFrame += Time.deltaTime;
            passButtonImage.color = Color.Lerp(passButtonAnimationStart, passButtonAnimationGoal, passButtonAnimationFrame / passButtonAnimationTime);
            if (passButtonAnimationFrame >= passButtonAnimationTime)
                passButtonImage.color = passButtonAnimationGoal;
        }
    }

    public void NewTurn(int player)
    {
        // Update pass status
        {
            passButtonAnimationFrame = 0;
            passButtonAnimationStart = passButtonImage.color;
            passButtonAnimationGoal = playerColors[player - 1];

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
    }

    public void StartGame()
    {
        if (game.gameStarted)
            return;

        game.StartGame();

        startText.enabled = false;
        passPanel.SetActive(true);
        NewTurn(1);
    }

    public void PlayerPassed()
    {
        game.PassPlayer();
    }
}
