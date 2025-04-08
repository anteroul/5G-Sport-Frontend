using System;
using UnityEngine;

[Serializable]
public class Player : MonoBehaviour
{
    public int ID;
    public string Name;
    public readonly float Weight;
    public Team PlayerTeam;
    public int HR;
    public float ECG;
    public int CaloriesBurnt;
    public float DirectionInAngles;

    private float force;
    private float acceleration;

    
    Player(int iD, string name, Team team)
    {
        System.Random rand = new System.Random();
        
        ID = iD;
        Name = name;
        Weight = (float)(rand.NextDouble() * (105 - 80) + 80);
        PlayerTeam = team;
        HR = 75;
        ECG = 0.0f;
        CaloriesBurnt = 0;
        DirectionInAngles = 45.0f;
        acceleration = 0.0f;
        force = Weight * acceleration;
    }

    public void UpdatePlayer(int _hr, float _ecg, float _acc)
    {
        HR = _hr;
        ECG = _ecg;
        acceleration = _acc;
        force = acceleration * Weight;
        Debug.Log(force);
    }
}
