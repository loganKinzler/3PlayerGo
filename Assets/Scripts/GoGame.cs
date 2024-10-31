using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoGame : MonoBehaviour
{



    public Vector3[] FloodFill(Vector3 startingPosition, int[] whiteList) {
        return new Vector3[0];
    }

    public Vector3[] GetSurrounding(Vector3 startingPosition, int[] whiteList) {
        if (GameObject.Find(string.Format("Diamond:({0},{1},{2})",
            startingPosition.x, startingPosition.y, startingPosition.z))) return new Vector3[4];

        Vector3[] surrounding = new Vector3[4];
        switch (startingPosition.z) {
            case 0: surrounding = new Vector3[4]{
                new Vector3(startingPosition.x, startingPosition.y, 2),
                new Vector3(startingPosition.x+1, startingPosition.y, 2),
                new Vector3(startingPosition.x, startingPosition.y, 1),
                new Vector3(startingPosition.x+1, startingPosition.y-1, 1)
            }; break;

            case 1: surrounding = new Vector3[4]{
                new Vector3(startingPosition.x, startingPosition.y, 0),
                new Vector3(startingPosition.x, startingPosition.y, 2),
                new Vector3(startingPosition.x, startingPosition.y+1, 2),
                new Vector3(startingPosition.x+1, startingPosition.y+1, 1)
            }; break;

            case 2: surrounding = new Vector3[4]{
                new Vector3(startingPosition.x, startingPosition.y, 0),
                new Vector3(startingPosition.x, startingPosition.y, 1),
                new Vector3(startingPosition.x, startingPosition.y-1, 1),
                new Vector3(startingPosition.x-1, startingPosition.y, 1)
            }; break;
        }

        for (int s=0; s<4; s++) {
            GameObject surroundingDiamond = GameObject.Find(string.Format("Diamond:({0},{1},{2})",
                surrounding[s].x, surrounding[s].y, surrounding[s].z));

            if (surroundingDiamond == null) continue;
            if ( hasElement(whiteList, surroundingDiamond.GetComponent<Diamond>().player) ) continue;
            surrounding[s] = Vector3.back;
        }
        
        return surrounding;
    }

    private bool hasElement(int[] array, int elem) {
        foreach (object obj in array) if (obj.Equals(elem)) return true;
        return false;
    }
}
