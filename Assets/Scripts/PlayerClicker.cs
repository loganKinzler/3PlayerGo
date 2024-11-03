using System;
using System.Collections;
using UnityEngine;

public class PlayerClicker : MonoBehaviour
{
    private float clickDelay = 0.1f;
    private bool clickDebounce = false;
    private int testPlayer = 1;


    private delegate void ClickDelegate(int mouseButton);
    private ClickDelegate clickMethod;

    DiamondGridGenerator diamondGrid;
    GoGame game;

    void Start()
    {
        diamondGrid = FindObjectOfType<DiamondGridGenerator>();
        game = FindObjectOfType<GoGame>();

        clickMethod = GameActual;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !clickDebounce) {
            clickDebounce = true;
            StartCoroutine(MouseCoroutine(0, clickMethod));
        }

        else if (Input.GetMouseButtonUp(1) && !clickDebounce) {
            clickDebounce = true;
            StartCoroutine(MouseCoroutine(1, clickMethod));
        }

        else if (Input.GetMouseButtonUp(2) && !clickDebounce) {
            clickDebounce = true;
            StartCoroutine(MouseCoroutine(2, clickMethod));
        }
    }


    // MOUSE METHODS
    private IEnumerator MouseCoroutine(int mouseButton, ClickDelegate clickDelegate) {
        clickDelegate(mouseButton);

        yield return new WaitForSeconds(clickDelay);
        clickDebounce = false;
    }

    private Diamond GetClickedDiamond() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if(hit.collider != null) {
            Vector3 clickedHexPosition = HexPositionFromEuclidean(hit.point);
            
            if (!diamondGrid.diamonds.TryGetValue(new Vector3Int(
                (int)clickedHexPosition.x,
                (int)clickedHexPosition.y,
                (int)clickedHexPosition.z),
                out Diamond diamondObject)) return null;
            return diamondObject.GetComponent<Diamond>();
        }

        return null;
    }

    private void GameActual(int mouseButton) {
        if (mouseButton != 0) return;
        Diamond clickedDiamond = GetClickedDiamond();

        game.TryPlaceDiamond(clickedDiamond);

        /*if (validationRequest == null || clickedDiamond == null) return;

        if (!validationRequest(clickedDiamond)) {
            if (validationFail == null) return;
            validationFail(clickedDiamond);
        }

        if (clickProccesser == null) return;
        clickProccesser(clickedDiamond);*/
    }

    // TRANSFORMATION METHODS
    private Vector3 HexPositionFromEuclidean(Vector2 euclidPosition) {
        Vector2 hexXY = new Vector2(
            euclidPosition.x * 2.0f/3.0f,
            -euclidPosition.x/3.0f + euclidPosition.y * (float)Math.Pow(3, -0.5f)
        ) / diamondGrid.GetDiamondSize();

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
