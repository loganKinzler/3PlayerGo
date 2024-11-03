using System;
using System.Collections.Generic;
using UnityEngine;

public class DiamondGridGenerator : MonoBehaviour
{
    public Diamond diamond;
    public int gridSize = 5;
    public Dictionary<Vector3Int, Diamond> diamonds = new Dictionary<Vector3Int, Diamond>();

    void Start()
    {
        CreateGrid(gridSize - 1);
    }

    void CreateGrid(int size) {
        for (int y=0; y<=2*size; y++)
                for (int x=-Math.Min(y, size); x<Math.Min(2*size - y + 1, size + 1); x++)
                    CreateHexagon(x, y - size);
    }


    void CreateHexagon(int x, int y) {
        for (int z=0; z<3; z++)
            PlaceDiamond(x,y,z);
    }

    void PlaceDiamond(int x, int y, int z) {
        Diamond diamondClone = Instantiate(diamond);
        diamondClone.name = string.Format("Diamond:({0},{1},{2})", x, y, z);
        diamondClone.transform.SetParent(gameObject.transform);
        diamondClone.transform.localScale = Vector3.one * GetDiamondSize();
        diamondClone.InstantiateDiamond(x, y, z);
        diamonds[new Vector3Int(x, y, z)] = diamondClone;
    }

    public float GetDiamondSize() {
        return 0.5f;// SCALE DIAMONDS S THEY FIT ON SCREEN (LATER)
    }
}
