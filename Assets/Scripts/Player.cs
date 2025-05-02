using UnityEngine;

public class Player
{
    public int ID;
    public string Name;
    public readonly float Weight;
    public bool PlayerTeam;
    public int HR;
    public int[] ECG;
    public int CaloriesBurnt;
    public float DirectionInAngles;

    private float force;
    private float acceleration;

    public Player(int iD, string name, bool team)
    {
        System.Random rand = new System.Random();
        
        ID = iD;
        Name = name;
        Weight = (float)(rand.NextDouble() * (105 - 80) + 80);
        PlayerTeam = team;
        HR = 75;
        CaloriesBurnt = 0;
        DirectionInAngles = 45.0f;
        acceleration = 0.0f;
        force = Weight * acceleration;
    }

    public void UpdatePlayer(int _hr, int[] _ecg, float _acc)
    {
        HR = _hr;
        ECG = _ecg;
        acceleration = _acc;
        force = acceleration * Weight;
        Debug.Log(force);
    }
}
