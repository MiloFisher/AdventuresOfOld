using UnityEngine;
using static UnityEngine.UI.Button;

public enum OptionRequirement {NONE, ANGELKIN_OR_HOLY, ANIMALKIN, BERSERK, CHAOS_TIER_1_TO_3, CHAOS_TIER_4_TO_6, DWARF_OR_HIGHBORN, ELVEN, FLEET_FOOTED, HAS_TORCH, LEONIN, LOOTER, MYSTICAL, POWERFUL };

[CreateAssetMenu(fileName = "New Event Card", menuName = "Cards/Event Card")]
public class EventCard : EncounterCard
{
    public int xp;
    public string[] optionNames;
    public string[] optionDescriptions;
    public OptionRequirement[] optionRequirements;
    public ButtonClickedEvent[] OptionEffects;
    //public void OptionEffect(int id) { EventOptionManager.Instance.CallOption(name,id); }
}
