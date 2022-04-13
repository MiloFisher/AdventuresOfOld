using UnityEngine;

[CreateAssetMenu(fileName = "New Event Card", menuName = "Cards/Event Card")]
public class EventCard : EncounterCard
{
    public int xp;
    public string[] optionNames;
    public string[] optionDescriptions;
    public void OptionEffect(int id) { EventOptionManager.Instance.CallOption(name,id); }
}
