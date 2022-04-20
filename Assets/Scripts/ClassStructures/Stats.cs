using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdventuresOfOldMultiplayer;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Stats {
    private int str = 0;
    private int dex = 0;
    private int inte = 0;
    private int spd = 0;
    private int con = 0;
    private int eng = 0;

    public Stats(int str, int dex, int inte, int spd, int con, int eng) {
        this.str = str;
        this.dex = dex;
        this.inte = inte;
        this.spd = spd;
        this.con = con;
        this.eng =  eng;
    }

    public int get_str() {return str;}
    public int get_dex() {return dex;}
    public int get_inte() {return inte;}
    public int get_spd() {return spd;}
    public int get_con() {return con;}
    public int get_eng() {return eng;}
    public override string ToString() {
        return ("Strength: " + str
            + " Dexterity: " + dex
            + " Intelligence: " + inte
            + " Speed: " + spd
            + " Constitution: " + con
            + " Energy: " + eng);
    }
    public void set_str(int str) {this.str = str;}
    public void set_dex(int dex) {this.dex = dex;}
    public void set_inte(int inte) {this.inte = inte;}
    public void set_spd(int spd) {this.spd = spd;}
    public void set_con(int con) {this.con = con;}
    public void set_eng(int eng) {this.eng = eng;}
}