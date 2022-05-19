using System;

[Serializable]
public class Effect
{
    public string name;
    public int potency;
    public int duration;
    public bool canStack;
    public int counter;

    public Effect(string name, int duration, int potency = default, bool canStack = default, int counter = default)
    {
        this.name = name;
        this.duration = duration;
        this.potency = potency;
        this.canStack = canStack;
        this.counter = counter;
    }
}
