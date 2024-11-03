using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodFillInfo : ScriptableObject
{
    public int player;
    public List<Diamond> previouslySearched;
    public List<Diamond> surrounding;
}

public class FloodFillEmptyResult
{
    public bool enclosed;
    public List<Diamond> emptyDiamonds;
    public List<Diamond> surroundingDiamonds;

    public FloodFillEmptyResult(bool enclosed)
    {
        this.enclosed = enclosed;
    }

    public FloodFillEmptyResult(bool enclosed, List<Diamond> emptyDiamonds, List<Diamond> surroundingDiamonds)
    {
        this.enclosed = enclosed;
        this.emptyDiamonds = emptyDiamonds;
        this.surroundingDiamonds = surroundingDiamonds;
    }

    public bool PlayerCanPlace(int player)
    {
        if (!enclosed)
            return true;

        for (int i = 0; i < surroundingDiamonds.Count; i++)
        {
            if (surroundingDiamonds[i].player == player)
                return true;
        }

        return false;
    }

    public int GetPlayerOwned()
    {
        Diamond newestDiamond = surroundingDiamonds[0];
        foreach (Diamond d in surroundingDiamonds)
        {
            if (d.turnPlaced > newestDiamond.turnPlaced)
                newestDiamond = d;
        }

        return newestDiamond.player;
    }

    public void SetVisuals()
    {
        int player = GetPlayerOwned();
        foreach (Diamond d in emptyDiamonds)
        {
            d.PlaceDiamond(0, player, d.turnPlaced);
        }
    }
}

public class FloodFillPlayerResult
{
    public bool enclosed;
    public List<Diamond> group; 
    
    public FloodFillPlayerResult(bool enclosed)
    {
        this.enclosed = enclosed;
    }

    public FloodFillPlayerResult(bool enclosed, List<Diamond> group)
    {
        this.enclosed = enclosed;
        this.group = group;
    }

    public void CaptureGroup(int player, int turn)
    {
        foreach (Diamond d in group)
        {
            d.PlaceDiamond(0, player, turn);
        }
    }
}