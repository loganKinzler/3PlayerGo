using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoGame : MonoBehaviour
{
    [SerializeField] PieChart scoreChart;

    // add scores up here and add functionality for them
    public int player = 1;
    public int player1score = 0;
    public int player2score = 0;
    public int player3score = 0;

    public int turnCount = 0;

    DiamondGridGenerator diamondGrid;

    void Start()
    {
        diamondGrid = FindObjectOfType<DiamondGridGenerator>();
    }

    public bool ValidatePlacement(Diamond diamondToPlace) 
    {
        if (diamondToPlace == null)
            return false;

        // CANNOT PLACE TILE ON ANOTHER
        if (diamondToPlace.player != 0)
            return false;

        // If player has no pieces, allow them to place anywhere (punishment for the other players ganging up on them
        if (player == 1 && player1score == 0 ||
            player == 2 && player2score == 0 ||
            player == 3 && player3score == 0)
            return true;

        // TILE GROUP CAPTURED
        FloodFillEmptyResult result = FloodFillEmpty(diamondToPlace);
        if (result != null && !result.PlayerCanPlace(player))
            return false;

        // WOULD BE CAPTURED
        // Set diamond to player temporarily
        diamondToPlace.player = player;
        FloodFillPlayerResult resultPlayer = FloodFillPlayer(diamondToPlace);
        if (resultPlayer != null && resultPlayer.enclosed)
        {
            diamondToPlace.player = 0;
            return false;
        }
        diamondToPlace.player = 0;

        return true;
    }

    // Run flood fill on a group of empty diamonds and get the surrounding diamonds
    public FloodFillEmptyResult FloodFillEmpty(Diamond startingDiamond)
    {
        if (turnCount < 4) // Earliest turn a piece can be captured
            return null;

        if (startingDiamond.player != 0)
            return null;

        List<Diamond> group = new List<Diamond>();
        List<Diamond> surroundingDiamonds = new List<Diamond>();
        List<int> playersInBorder = new List<int>();
        List<Diamond> diamondsChecked = new List<Diamond>();

        Queue<Diamond> checkQueue = new Queue<Diamond>();
        checkQueue.Enqueue(startingDiamond);

        while (checkQueue.Count > 0)
        {
            // Get next diamond in queue
            Diamond diamond = checkQueue.Dequeue();

            if (diamond == null)
                continue;

            // Check if solid
            if (diamond.player != 0) // Solid
            {
                // Add player to playersInBorder
                if (!playersInBorder.Contains(diamond.player))
                    playersInBorder.Add(diamond.player);

                // Check if group contains all 3 players. If so, it is not enclosed -> return
                if (playersInBorder.Count >= 3)
                    return new FloodFillEmptyResult(false);

                // Add to surrounding
                surroundingDiamonds.Add(diamond);
            }
            else // Not Solid
            {
                group.Add(diamond);

                // Add surrounding diamonds to queue
                List<Diamond> surrounding = GetSurrounding(diamond);
                foreach (Diamond d in surrounding)
                {
                    if (!diamondsChecked.Contains(d))
                    {
                        checkQueue.Enqueue(d);
                        diamondsChecked.Add(d);
                    }
                }
            }
        }

        return new FloodFillEmptyResult(true, group, surroundingDiamonds);
    }

    // Run flood fill on a group of a player's diamonds to see if it's captuerd
    public FloodFillPlayerResult FloodFillPlayer(Diamond startingDiamond)
    {
        if (turnCount < 4) // Earliest turn a capture can happen on is 5
            return null;

        int player = startingDiamond.player;
        if (player == 0)
            return null;

        List<Diamond> group = new List<Diamond>();
        List<Diamond> diamondsChecked = new List<Diamond>();

        Queue<Diamond> checkQueue = new Queue<Diamond>();
        checkQueue.Enqueue(startingDiamond);

        while (checkQueue.Count > 0)
        {
            // Get next diamond in queue
            Diamond diamond = checkQueue.Dequeue();

            if (diamond == null)
                continue;

            // Check if empty
            if (diamond.player == 0) // Empty
            {
                // Player is not enclosed, return
                return new FloodFillPlayerResult(false);
            }

            // Check if player's
            if (diamond.player == player) // Player's piece
            {
                group.Add(diamond);

                // Add surrounding diamonds to queue
                List<Diamond> surrounding = GetSurrounding(diamond);
                foreach (Diamond d in surrounding)
                {
                    if (!diamondsChecked.Contains(d))
                    {
                        checkQueue.Enqueue(d);
                        diamondsChecked.Add(d);
                    }
                }
            }
        }

        return new FloodFillPlayerResult(true, group);
    }

    public void TryPlaceDiamond(Diamond diamond)
    {
        // Check if piece can be placed
        if (!ValidatePlacement(diamond))
            return;

        // Set piece
        diamond.PlaceDiamond(player, 0, turnCount);

        // Capture groups if possible
        List<Diamond> surroundingDiamonds = GetSurrounding(diamond);
        foreach (Diamond d in surroundingDiamonds)
        {
            if (d == null)
                continue;

            if (d.player == 0)
                continue;

            FloodFillPlayerResult result = FloodFillPlayer(d);
            if (result != null && result.enclosed)
                result.CaptureGroup(player, turnCount);
        }

        // End turn
        UpdateScores();
        IteratePlayer();
    }

    public void UpdateScores()
    {
        player1score = 0;
        player2score = 0;
        player3score = 0;

        List<Diamond> diamondsChecked = new List<Diamond>();

        foreach (KeyValuePair<Vector3Int, Diamond> entry in diamondGrid.diamonds)
        {
            Diamond diamond = entry.Value;

            if (diamondsChecked.Contains(diamond))
                continue;

            diamondsChecked.Add(diamond);

            switch (diamond.player)
            {
                // Add score to player who's tile it is
                case 1:
                    player1score++;
                    break;
                case 2:
                    player2score++;
                    break;
                case 3:
                    player3score++;
                    break;

                // Empty spaces scored differently
                case 0:
                    // Add score based on empty pieces owned by player
                    FloodFillEmptyResult result = FloodFillEmpty(diamond);
                    if (result == null || !result.enclosed)
                    {
                        if (diamond.playerOwned != 0)
                            diamond.PlaceDiamond(0, 0, turnCount);
                        continue;
                    }

                    result.SetVisuals();
                    switch (result.GetPlayerOwned())
                    {
                        case 1:
                            player1score += result.emptyDiamonds.Count;
                            break;
                        case 2:
                            player2score += result.emptyDiamonds.Count;
                            break;
                        case 3:
                            player3score += result.emptyDiamonds.Count;
                            break;

                    }

                    // Add all empty pieces to checked list
                    foreach (Diamond d in result.emptyDiamonds)
                        diamondsChecked.Add(d);
                    break;
            }
        }
    }

    public void IteratePlayer() {
        player++;
        turnCount++;
        player = (player>3)? 1:player;

        scoreChart.CreatePieChart(player1score, player2score, player3score);
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
            if (!diamondGrid.diamonds.TryGetValue(new Vector3Int((int)surround.x, (int)surround.y, (int)surround.z), out Diamond surroundingDiamond))
            {
                surroundingDiamonds.Add(null);
                continue;
            }
            
            surroundingDiamonds.Add(surroundingDiamond);
        }

        return surroundingDiamonds;
    }
}
