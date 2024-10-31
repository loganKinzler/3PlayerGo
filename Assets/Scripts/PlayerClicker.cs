using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClicker : MonoBehaviour
{
    private float clickDelay = 0.1f;
    private bool clickDebounce = false;

    private int player = 1;


    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !clickDebounce) {
            clickDebounce = true;
            StartCoroutine(MouseCoroutine(0));
        }

        else if (Input.GetMouseButtonUp(1) && !clickDebounce) {
            clickDebounce = true;
            StartCoroutine(MouseCoroutine(1));
        }

        else if (Input.GetMouseButtonUp(2) && !clickDebounce) {
            clickDebounce = true;
            StartCoroutine(MouseCoroutine(2));
        }
    }


    // MOUSE METHODS
    private IEnumerator MouseCoroutine(int mouseButton) {
        MouseTesting(mouseButton);

        yield return new WaitForSeconds(clickDelay);
        clickDebounce = false;
    }

    private void MouseClick(int playerClick) {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if(hit.collider != null) {
            Vector3 clickedHexPosition = HexPositionFromEuclidean(hit.point);
            GameObject diamond = GameObject.Find(string.Format("Diamond:({0},{1},{2})",
                clickedHexPosition.x, clickedHexPosition.y, clickedHexPosition.z));
            
            if (diamond != null) diamond.GetComponent<Diamond>().PlaceDiamond(playerClick);
        }
    }

    private void MouseTesting(int mouseButton) {
        switch (mouseButton) {
            case 0:
                MouseClick(player);
            break;

            case 1:
                MouseClick(0);
            break;

            case 2:
                player++;
                player = (player>3)? 1:player;
            break;
        }
    }

    // TRANSFORMATION METHODS
    private Vector3 HexPositionFromEuclidean(Vector2 euclidPosition) {
        Vector2 hexXY = new Vector2(
            euclidPosition.x * 2.0f/3.0f,
            -euclidPosition.x/3.0f + euclidPosition.y * (float)Math.Pow(3, -0.5f)
        ) / GameObject.FindAnyObjectByType<DiamondGridGenerator>().GetDiamondSize();

        Vector2 hexCorner = new Vector2(
            (float)Math.Floor((double) hexXY.x),
            (float)Math.Floor((double) hexXY.y)
        );

        Vector2 hexCornerDiff = hexXY - hexCorner;

        // IN (1,1) HEXAGON
        if ((hexCornerDiff.y > -2*hexCornerDiff.x + 2) && (hexCornerDiff.y > -0.5*hexCornerDiff.x + 1)) {
            return new Vector3(hexCorner.x + 1, hexCorner.y + 1, 2);
        }

        // IN (0,0) HEXAGON
        if ((hexCornerDiff.y < -2*hexCornerDiff.x + 1) && (hexCornerDiff.y < -0.5*hexCornerDiff.x + 0.5)) {
            return new Vector3(hexCorner.x, hexCorner.y, (hexCornerDiff.y >= hexCornerDiff.x)? 1:0);
        }
        
        // IN (0,1) HEXAGON
        if (hexCornerDiff.y >= hexCornerDiff.x) {
            return new Vector3(hexCorner.x, hexCorner.y + 1, (hexCornerDiff.y <= -2*hexCornerDiff.x + 1)? 2:0);
        }

        // IN (1,0) HEXAGON
        return new Vector3(hexCorner.x + 1, hexCorner.y, (hexCornerDiff.y <= -0.5*hexCornerDiff.x + 0.5)? 2:1);
    }
}
