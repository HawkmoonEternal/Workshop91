using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    private static int cash=0;
    private static int xp = 0;
    private static int level = 1;
    public static int firstXPStep=250;
    public static MyGUI gui;

    
    public static int getCash() {
        return cash;
    }
    public static int getXP()
    {
        return xp;
    }
    public static int getLevel()
    {
        return level;
    }
    public static void updateCash(int amount) {
        cash += amount;
    }
    public static void updateXP(int amount)
    {
        xp += amount;
        if (xp >= Mathf.Pow(2, level)*firstXPStep)updateLevel(1);
        Debug.Log(xp + " " + Mathf.Pow(2, level) * firstXPStep + " " + level);
    }
    public static void updateLevel(int amount)
    {
        level += amount;
    }
}
