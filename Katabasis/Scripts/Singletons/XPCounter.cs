using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class XPCounter
{
    public static int CurrentLevel = 1;
    public static int CurrentXP;
    public static int XPpoints;
    private const int baseXPLevelCost = 100;
    public static int nextLevelCost => (int)(baseXPLevelCost + baseXPLevelCost * 1.5f * (CurrentLevel-1));

    public static Action OnExpChanged;
    public static void AddXP(int xp)
    {
        CurrentXP += xp;
        while(CurrentXP > nextLevelCost)
        {
            CurrentXP -= nextLevelCost;
            CurrentLevel++;
            XPpoints += 2;
            Announcer.Announce("You are lvl " + CurrentLevel + " now!");
        }
        OnExpChanged?.Invoke();
    }


}
