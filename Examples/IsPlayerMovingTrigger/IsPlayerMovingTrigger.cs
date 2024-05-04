using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static DataManager.GameData.BeatmapData.EventTriggers;
namespace InvisblePlayerEvent;


[HarmonyPatch(typeof(VGPlayer))]
public class VGPlayer_Patch
{
    public static List<Trigger> _movedTriggers = new();
    public static List<Trigger> _stoppedTriggers = new();
    
    static readonly Dictionary<int, bool> HasTriggeredMoved = new();
    static readonly Dictionary<int, bool> HasTriggeredStopped = new();

    [HarmonyPatch(nameof(VGPlayer.Init), new Type[]{})]
    [HarmonyPostfix]
    static void PostInit(ref VGPlayer __instance)
    {
        HasTriggeredMoved.TryAdd(__instance.PlayerID, false);
        HasTriggeredStopped.TryAdd(__instance.PlayerID, false);
        
        //so this is never cleared, but just a few entries shouldn't matter
    }
    
    [HarmonyPatch(nameof(VGPlayer.Update))]
    [HarmonyPostfix]
    static void PostUpdate(ref VGPlayer __instance)
    {
        if (_movedTriggers.Count != 0)
            HandleMovedTriggers(ref __instance);

        if (_stoppedTriggers.Count != 0)
            HandleStoppedTriggers(ref __instance);
    }

    //triggers event is player just started moving
    static void HandleMovedTriggers(ref VGPlayer inst)
    {
        if (HasTriggeredMoved[inst.PlayerID] || inst.internalVelocity.magnitude == 0)
            return;
        
        foreach (var trigger in _movedTriggers)
        {
            GameManager2.inst.CallEvent(trigger);
            HasTriggeredMoved[inst.PlayerID] = true;
            HasTriggeredStopped[inst.PlayerID] = false;
        }
    }
    //triggers event is player just stopped moving
    static void HandleStoppedTriggers(ref VGPlayer inst)
    {
        if (HasTriggeredStopped[inst.PlayerID] || inst.internalVelocity.magnitude != 0)
            return;

        foreach (var trigger in _stoppedTriggers)
        {
            GameManager2.inst.CallEvent(trigger);
            HasTriggeredStopped[inst.PlayerID] = true;
            HasTriggeredMoved[inst.PlayerID] = false;
        }
    }
}


[HarmonyPatch(typeof(GameManager2))]
public class GameManager2_Patch
{
    [HarmonyPatch(nameof(GameManager2.SetupPlayerEventTriggers))]
    [HarmonyPostfix]
    static void PostStart(ref GameManager2 __instance)
    {
        VGPlayer_Patch._movedTriggers.Clear();
        VGPlayer_Patch._stoppedTriggers.Clear();
        
        TriggerType playerMovingType = (TriggerType)TriggerAPI.RegisterTriggerEvents.GetTriggerEnumFromName("Player_Moved");
        TriggerType playerStandingType = (TriggerType)TriggerAPI.RegisterTriggerEvents.GetTriggerEnumFromName("Player_Stopped");
        var Enu = DataManager.inst.gameData.beatmapData.eventTriggers.triggers.GetEnumerator();
        while (Enu.MoveNext())
        {
            if (Enu.Current.EventTrigger == playerMovingType)
            {
                VGPlayer_Patch._movedTriggers.Add(Enu.Current);
            }
        }
        Enu.Dispose();
        
        Enu = DataManager.inst.gameData.beatmapData.eventTriggers.triggers.GetEnumerator();
        while (Enu.MoveNext())
        {
            if (Enu.Current.EventTrigger == playerStandingType)
            {
                VGPlayer_Patch._stoppedTriggers.Add(Enu.Current);
            }
        }
        Enu.Dispose();
    }
}