using System;
using System.Collections.Generic;
using UnityEngine;

public class GoGame : MonoBehaviour
{
    public FloodFillInfo FloodFill(Diamond startingDiamond) {

        FloodFillInfo FFinfo = ScriptableObject.CreateInstance<FloodFillInfo>();
        FFinfo.player = startingDiamond.player;
        FFinfo.previouslySearched = new List<Diamond>();
        FFinfo.surrounding = GetSurrounding(startingDiamond);
        FFinfo.completelySurrounded = false;


        while (!FFinfo.completelySurrounded) {
            CullSurrounding(FFinfo.player, FFinfo.previouslySearched, FFinfo.surrounding);
            
            for (int ds=FFinfo.surrounding.Count-1; ds>=0; ds--) {

                // WITH AN EMPTY SPACE, THERE'S NOTHING THAT NEEDS TO BE DONE (NO POSSIBLE WAY TO TAKE THE GROUP)
                if (FFinfo.surrounding[ds].player == 0) return FFinfo;
                FFinfo.completelySurrounded = true;

                // THERE STILL IS MORE TO EXPAND TO
                if (FFinfo.surrounding[ds].player == FFinfo.player) {
                    FFinfo.completelySurrounded = false;
                    continue;
                }

                // REMOVE THE NON-SIMILAR PLAYER DIAMONDS SO THEY DON'T GET EXPANDED INTO    
                FFinfo.surrounding.RemoveAt(ds);
            }

            // SET UP FOR NEXT ITERATION
            FFinfo.previouslySearched.AddRange(FFinfo.surrounding);

            List<Diamond> newSurrounding = new List<Diamond>();
            foreach (Diamond surround in FFinfo.surrounding)
                newSurrounding.AddRange(GetSurrounding(surround));
        }

        return FFinfo;
    }

    public void CullSurrounding(int player, List<Diamond> previouslySearched, List<Diamond> surroundingList) {
        for (int sd=surroundingList.Count-1; sd>=0; sd--) {
            if (!previouslySearched.Contains(surroundingList[sd])) {surroundingList.RemoveAt(sd); continue;}
        }
    }

    public List<Diamond> GetSurrounding(Diamond diamond) {
        List<Diamond> surroundingDiamonds = new List<Diamond>();
        Vector3[] surrounding = new Vector3[4];

        Vector3 startingPosition = diamond.GetPosition();
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
        
        foreach (Vector3 surround in surrounding) {
            Diamond surroundingDiamond = GameObject.Find(string.Format("Diamond:({0},{1},{2})",
                surround.x, surround.y, surround.z)).GetComponent<Diamond>();
            
            if (surroundingDiamond == null) continue;
            surroundingDiamonds.Add(surroundingDiamond);
        }

        return surroundingDiamonds;
    }
}
