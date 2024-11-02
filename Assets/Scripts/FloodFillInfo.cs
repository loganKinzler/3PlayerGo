using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodFillInfo : ScriptableObject
{
    public int player;
    public List<Diamond> previouslySearched;
    public List<Diamond> surrounding;
}
