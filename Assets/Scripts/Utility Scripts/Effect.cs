using System;

[Serializable]
public class Effect
{
    public string name;
    public int potency;
    public int duration;

    public Effect(string name, int duration, int potency = default)
    {
        this.name = name;
        this.duration = duration;
        this.potency = potency;
    }
}
