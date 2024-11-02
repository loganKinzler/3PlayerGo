using System;
using System.Collections;
using UnityEngine;

public class PlayerClicker : MonoBehaviour
{
    public GoGame GameScript;
    private float clickDelay = 0.1f;
    private bool clickDebounce = false;
    private int testPlayer = 1;// TESTING VALUE


    private delegate void ClickDelegate(int mouseButton);
    private ClickDelegate clickMethod;

    public delegate bool ClickValidationRequest(Diamond diamond);
    public event ClickValidationRequest validationRequest;

    public delegate void ValidationFalure(Diamond diamond);
    public event ValidationFalure validationFail;

    public delegate void ClickProccesser(Diamond diamond);
    public event ClickProccesser clickProccesser;

    void Start()
    {
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
            GameObject diamondObject = GameObject.Find(string.Format("Diamond:({0},{1},{2})",
                clickedHexPosition.x, clickedHexPosition.y, clickedHexPosition.z));
            
            return diamondObject.GetComponent<Diamond>();
        }

        return null;
    }

    private void GameActual(int mouseButton) {
        if (mouseButton != 0) return;
        Diamond clickedDiamond = GetClickedDiamond();

        if (validationRequest == null) return;

        if (!validationRequest(clickedDiamond)) {
            if (validationFail == null) return;
            validationFail(clickedDiamond);
        }

        if (clickProccesser == null) return;
        clickProccesser(clickedDiamond);
    }

    private void GameTesting(int mouseButton) {
        Diamond clickedDiamond = GetClickedDiamond();

        switch (mouseButton) {
            case 0:
                clickedDiamond.PlaceDiamond(testPlayer);
            break;

            case 1:
                clickedDiamond.PlaceDiamond(0);
            break;

            case 2:                
                testPlayer++;
                testPlayer = (testPlayer>3)? 1:testPlayer;
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
