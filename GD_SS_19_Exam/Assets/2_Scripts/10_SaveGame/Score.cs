using UnityEngine;

[CreateAssetMenu(fileName = "NewScore", menuName = "New Score")]
public class Score : ScriptableObject
{
    public float HighScore;
    public float lastScore;
}
