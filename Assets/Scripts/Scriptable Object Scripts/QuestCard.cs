using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Card", menuName = "Cards/Quest Card")]
public class QuestCard : ScriptableObject
{
    public string cardName;
    public Sprite image;
    public int questStep;
    public string[] objectiveNames;
    public int[] objectiveXPRewards;
    public int rewardGold;
    public int rewardChaos;
}
