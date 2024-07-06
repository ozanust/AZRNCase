using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfigSO", order = 1)]
public class GameConfigSO : ScriptableObject
{
    public int PointsPerGoal;
    public int ExtraPointsPerTargetHit;
    public float ShootSpeed;
    public string GameDifficulty;
    public int PlayerHealth;
    public float ShootingTime;
}