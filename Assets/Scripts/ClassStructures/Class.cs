using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Class {
    private Stats stats;
    private string name;

    public Class(string name) {
        this.name = name;
    }
}