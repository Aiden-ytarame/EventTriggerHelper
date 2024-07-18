using System;
using System.Collections.Generic;
using HarmonyLib;
using TriggerAPI;

namespace MovingTrigger;


[HarmonyPatch(typeof(VGPlayer))]
public class VgPlayerPatch
{
    static readonly Dictionary<int, bool> HasTriggeredMoved = new();
    static readonly Dictionary<int, bool> HasTriggeredStopped = new();

    [HarmonyPatch(nameof(VGPlayer.Init), new Type[]{})]
    [HarmonyPostfix]
    static void PostInit(ref VGPlayer __instance)
    {
        HasTriggeredMoved.TryAdd(__instance.PlayerID, false);
        HasTriggeredStopped.TryAdd(__instance.PlayerID, false);
    }
    [HarmonyPatch(nameof(VGPlayer.OnDestroy))]
    [HarmonyPostfix]
    static void PostDeath(ref VGPlayer __instance)
    {
        HasTriggeredMoved.Remove(__instance.PlayerID);
        HasTriggeredStopped.Remove(__instance.PlayerID);
    }
    
    [HarmonyPatch(nameof(VGPlayer.Update))]
    [HarmonyPostfix]
    static void PostUpdate(ref VGPlayer __instance)
    {
        HandleMovedTriggers(ref __instance);
        HandleStoppedTriggers(ref __instance);
    }

    //triggers event is player just started moving
    static void HandleMovedTriggers(ref VGPlayer inst)
    {
        if (HasTriggeredMoved[inst.PlayerID] || inst.internalVelocity.magnitude == 0)
            return;
        
        RegisterTriggerEvents.CallCustomTrigger("Player_Moved");
        HasTriggeredMoved[inst.PlayerID] = true;
        HasTriggeredStopped[inst.PlayerID] = false;
    }

    //triggers event is player just stopped moving
    static void HandleStoppedTriggers(ref VGPlayer inst)
    {
        if (HasTriggeredStopped[inst.PlayerID] || inst.internalVelocity.magnitude != 0)
            return;

        RegisterTriggerEvents.CallCustomTrigger("Player_Stopped");
        HasTriggeredStopped[inst.PlayerID] = true;
        HasTriggeredMoved[inst.PlayerID] = false;
    }
}

