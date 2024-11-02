using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        GameObject.Find("Background").GetComponent<PlayerClicker>().validationRequest += ValidatePlacement;
        GameObject.Find("Background").GetComponent<PlayerClicker>().clickProccesser += ClickProccesser;
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
            
            // ADJACENT TILE IS THE PLAYER'S (CHECK IF PLACING CURRENT TILE WILL CAUSE THE GROUP TO BE TAKEN)
            if (surround.player == diamondToPlace.player) {

                // FULL GROUP IS SURROUNDED (GROUP WILL BE TAKEN)
                FloodFillInfo floodFromSurround = FloodFill(surround);
                if (GroupIsCompletelySurrounded(floodFromSurround.surrounding)) {
                    diamondToPlace.player = prevDiamondPlayer;// RESET DtP
                    return false;
                }
                
                // AN EMPTY SPACE SOMEWHERE (GROUP IS FINE)
                diamondToPlace.player = prevDiamondPlayer;// RESET DtP
                return true;
            }
        }
        
        // CORNER CASE (PLACED IN CORNER ALONE, BUT SURROUNDED [WILL GET TAKEN])
        diamondToPlace.player = prevDiamondPlayer;// RESET DtP
        return false;
    }

    public void ValidationFailure(Diamond diamond) {
        print(string.Format("Failed to Place Diamond at:{0}", diamond.GetPosition()));
    }

    public void ClickProccesser(Diamond diamond) {
        diamond.PlaceDiamond(player);
        int scoreAddition = 1;

        List<Diamond> surrounding = GetSurrounding(diamond);
        foreach (Diamond surround in surrounding) {

            // TILE IS EMPTY OR PLAYER'S (DO NOTHING [VAIDATION ALREAYD RAN, SO NO ISSUES WITH CAPTURING SELF])
            if (surround.player == diamond.player || surround.player == 0) continue;
            FloodFillInfo floodInfo = FloodFill(surround);

            // GROUP IS NOT ENCLOSED (DO NOTHING)
            if (!GroupIsCompletelySurrounded(floodInfo.surrounding)) continue;

            // REMOVE THE ENCLOSED GROUP
            foreach (Diamond floodedDiamond in floodInfo.previouslySearched)
                floodedDiamond.PlaceDiamond(0);
            
            // ADD TAKEN TILES TO PLAYER'S SCORE
            scoreAddition += floodInfo.previouslySearched.Count;

            // REMOVE SCORE OF TAKEN TILES
            switch (surround.player) {
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
        player = (player>3)? 1:player;
    }

    public FloodFillInfo FloodFill(Diamond startingDiamond) {

        FloodFillInfo FFinfo = ScriptableObject.CreateInstance<FloodFillInfo>();
        FFinfo.player = startingDiamond.player;
        FFinfo.previouslySearched = new List<Diamond>(){startingDiamond};
        FFinfo.surrounding = GetSurrounding(startingDiamond);

        int limit = 100;
        int iter = 0;

        bool hasExpanded = true;
        while (hasExpanded && iter < limit) {
            hasExpanded = false;
            iter++;

            for (int sd=FFinfo.surrounding.Count-1; sd>=0; sd--) {
                
                // WALL IS FORMED
                if (FFinfo.surrounding[sd].player != FFinfo.player) continue;
                hasExpanded = true;

                // EXPAND WHERE IT CAN
                List<Diamond> expansion = GetSurrounding(FFinfo.surrounding[sd]);
                for (int ed=expansion.Count-1; ed>=0; ed--) {
                    if (!FFinfo.previouslySearched.Contains(expansion[ed]) && !FFinfo.surrounding.Contains(expansion[ed])) continue;
                    expansion.RemoveAt(ed);
                }

                // REMOVE THE TILE THAT WAS EXPANDED OFF OF & ADD NEW EXPANSION
                FFinfo.previouslySearched.Add(FFinfo.surrounding[sd]);
                FFinfo.surrounding.AddRange<Diamond>(expansion);
            }
        }

        return FFinfo;
    }

    public bool GroupIsCompletelySurrounded(List<Diamond> surroundsGroup) {
        foreach (Diamond diamond in surroundsGroup)
            if (diamond.player == 0) return false;
        return true;
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
                new Vector3(startingPosition.x-1, startingPosition.y+1, 0)
            }; break;

            case 2: surrounding = new Vector3[4]{
                new Vector3(startingPosition.x, startingPosition.y, 0),
                new Vector3(startingPosition.x, startingPosition.y, 1),
                new Vector3(startingPosition.x, startingPosition.y-1, 1),
                new Vector3(startingPosition.x-1, startingPosition.y, 0)
            }; break;
        }
        
        foreach (Vector3 surround in surrounding) {
            if (GameObject.Find(string.Format("Diamond:({0},{1},{2})",
                surround.x, surround.y, surround.z)).GetComponent<Diamond>() == null)
                    continue;
                    
            Diamond surroundingDiamond = GameObject.Find(string.Format("Diamond:({0},{1},{2})",
                surround.x, surround.y, surround.z)).GetComponent<Diamond>();
            
            surroundingDiamonds.Add(surroundingDiamond);
        }

        return surroundingDiamonds;
    }
}
