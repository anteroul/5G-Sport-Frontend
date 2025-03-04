using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HockeyAction
{
    public string actionType; // E.g., "Skating", "Shooting", "Passing", "Defending"
    public float duration;    // How long to perform this action
}

[Serializable]
public class HockeyActionList
{
    public List<HockeyAction> actions;
}
