using System;
using System.Collections.Generic;
using UnityEngine;

public class GoGame : MonoBehaviour
{

    // add scores up here and add functionality for them
    public int player = 1;
    public int player1score = 0;
    public int player2score = 0;
    public int player3score = 0;

    void Start()
    {
        GameObject.Find("Square").GetComponent<PlayerClicker>().validationRequest += ValidatePlacement;
        GameObject.Find("Square").GetComponent<PlayerClicker>().clickProccesser += ClickProccesser;
    }

    public bool ValidatePlacement(Diamond diamondToPlace) {
        List<Diamond> surroundingDiamonds = GetSurrounding(diamondToPlace);
        
        // CANNOT PLACE TILE ON ANOTHER
        if (diamondToPlace.player != 0) return false;

        // CHECK IF PREVIOUS MOVES WERE IDENTICLE (LATER)

        // TEMPORARILY SET DtP AS IF IT HAD ALREADY BEEN PLACED (RESET BEFORE RETURNING)
        int prevDiamondPlayer = diamondToPlace.player;
        diamondToPlace.player = player;

        foreach (Diamond surround in surroundingDiamonds) {

            // ADJCENT TILE IS BLANK (ALWAYS VALID)
            if (surround.player == 0) {
                diamondToPlace.player = prevDiamondPlayer;// RESET DtP
                return true;
            }
            
            // ADJACENT TILE IS ANOTHER PLAYER'S (NOT ENOUGH INFORMATION TO WORK WITH)
            if (surround.player != diamondToPlace.player) continue;
            
            // ADJACENT TILE IS THE PLAYER'S (NEED TO CHECK IF PLACING CURRENT TILE WILL CAUSE THE GROUP TO BE TAKEN)
            if (surround.player == diamondToPlace.player) {

                // FULL GROUP IS SURROUNDED (GROUP WILL BE TAKEN)
                FloodFillInfo floodFromSurround = FloodFill(surround);
                if (floodFromSurround.completelySurrounded) {
                    diamondToPlace.player = prevDiamondPlayer;// RESET DtP
                    return true;
                }
            }
        }
        
        diamondToPlace.player = prevDiamondPlayer;// RESET DtP

        // CORNER CASE (PLACED IN CORNER ALONE, BUT SURROUNDED [WILL GET TAKEN])
        return false;
    }

    public void ValidationFailure(Diamond diamond) {
        print(string.Format("Failed to Place Diamond at:{0}", diamond.GetPosition()));
    }

    public void ClickProccesser(Diamond diamond) {
        diamond.PlaceDiamond(player);
        int scoreAddition = 1;

        List<Diamond> surrounding = GetSurrounding(diamond);
        for (int s=0; s<surrounding.Count; s++) {
            
            // TILE IS EMPTY OR PLAYER'S (DO NOTHING [VAIDATION ALREAYD RAN, SO NO ISSUES WITH CAPTURING])
            if (surrounding[s].player == diamond.player || surrounding[s].player == 0) continue;
            FloodFillInfo floodInfo = FloodFill(surrounding[s]);

            if (floodInfo.previouslySearched.Count != 0) print("THANK GOD");

            // GROUP IS ENCLOSED (REMOVE IT)
            if (floodInfo.completelySurrounded)
                for (int f=0; f<floodInfo.previouslySearched.Count; f++)
                    floodInfo.previouslySearched[f].PlaceDiamond(0);
            
            // ADD TAKEN TILES TO PLAYER'S SCORE
            scoreAddition += floodInfo.previouslySearched.Count;

            // REMOVE SCORE OF TAKEN TILES
            switch (surrounding[s].player) {
                case 1:
                    player1score -= scoreAddition;
                break;

                case 2:
                    player2score -= scoreAddition;
                break;

                case 3:
                    player3score -= scoreAddition;
                break;
            }   
        }

        // ADD SCORE TO CORRECT PLAYER
        switch (player) {
            case 1:
                player1score += scoreAddition;
            break;

            case 2:
                player2score += scoreAddition;
            break;

            case 3:
                player3score += scoreAddition;
            break;
        }    

        IteratePlayer();
    }

    public void IteratePlayer() {
        player++;
        player = player%3 + 1;
    }

    public FloodFillInfo FloodFill(Diamond startingDiamond) {

        FloodFillInfo FFinfo = ScriptableObject.CreateInstance<FloodFillInfo>();
        FFinfo.player = startingDiamond.player;
        FFinfo.previouslySearched = new List<Diamond>();
        FFinfo.surrounding = GetSurrounding(startingDiamond);
        FFinfo.completelySurrounded = false;

        int iter = 0;
        int MAX_ITERS = 10_000;
        while (!FFinfo.completelySurrounded && iter < MAX_ITERS) {
            RemovePreviouslySearched(FFinfo.previouslySearched, FFinfo.surrounding);
            iter++;

            for (int ds=FFinfo.surrounding.Count-1; ds>=0; ds--) {

                // EMPTY SPACE (THERE'S NOTHING THAT NEEDS TO BE DONE [NO POSSIBLE WAY TO TAKE THE GROUP] )
                if (FFinfo.surrounding[ds].player == 0) return FFinfo;
                FFinfo.completelySurrounded = true;// IF THERE IS NO SURROUNDING PLAYER TILES, IT'S FINISHED

                // SAME PLAYER (THERE STILL IS MORE TO EXPAND TO)
                if (FFinfo.surrounding[ds].player == FFinfo.player) {
                    FFinfo.completelySurrounded = false;// RESET SO ALG DOESN'T END EARLY
                    print("Same player is adjacent");
                    continue;
                }

                // NON-PLAYER (REMOVE THEM, SO THEY DON'T GET EXPANDED INTO)    
                FFinfo.surrounding.RemoveAt(ds);

                // STOP POTENTAIL INFINITE LOOPS
                if (FFinfo.surrounding.Count == 0) return FFinfo;
            }

            // SET UP FOR NEXT ITERATION:
            // ADD SURROUNDING
            FFinfo.previouslySearched.AddRange(FFinfo.surrounding);

            // CREATE NEW SURROUNDING
            List<Diamond> newSurrounding = new List<Diamond>();
            foreach (Diamond surround in FFinfo.surrounding)
                newSurrounding.AddRange(GetSurrounding(surround));
        }

        return FFinfo;
    }

    public void RemovePreviouslySearched(List<Diamond> previouslySearched, List<Diamond> surroundingList) {
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
            
            // DIAMOND IS OUT OF BOUNDS
            if (surroundingDiamond == null) continue;
            surroundingDiamonds.Add(surroundingDiamond);
        }

        return surroundingDiamonds;
    }
}
