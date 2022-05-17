using System;

[Serializable]
public class Effect
{
    public string name;
    public int potency;
    public int duration;
    public int counter;

    public Effect(string name, int duration, int potency = default, int counter = default)
    {
        this.name = name;
        this.duration = duration;
        this.potency = potency;
        this.counter = counter;
    }
}
