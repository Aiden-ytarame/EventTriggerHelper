using System;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TriggerAPI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BoostTrigger;

[HarmonyPatch(typeof(GameManager))]
public class GameManager_Patch
{
    [HarmonyPatch(nameof(GameManager.SetupPlayerEventTriggers))]
    [HarmonyPostfix]
    static void PostSetup(ref GameManager __instance)
    {
        VGPlayerManager.Inst.players.ForEach((System.Action<VGPlayerManager.VGPlayerData>)(player =>
        {
            player.PlayerObject.add_BoostEvent(new Action<Vector3>(_ =>
            {
                RegisterTriggerEvents.CallCustomTrigger("Player_Boost");
            }));
        }));
        
    }
}
[HarmonyPatch(typeof(VGPlayer))]
public static class HitPatch
{
    public static bool TakesDamage = true;
    public static bool gay = false;
    private static float timer = 1;
    private static Color lastColor = new();
    [HarmonyPatch(nameof(VGPlayer.PlayerHit))]
    [HarmonyPrefix]
    static bool PreHit()
    {
        return TakesDamage;
    }
    
     [HarmonyPatch(nameof(VGPlayer.SetColor))]
        [HarmonyPrefix]
        static void PreColor(ref Color _col)
        {
            if (gay && timer > 0.2f)
            {
                lastColor = new Color(Random.value, Random.value, Random.value);
                timer = 0;
            }

            _col = lastColor;
        }
        [HarmonyPatch(nameof(VGPlayer.Update))]
        [HarmonyPostfix]
        static void PostUpdate(ref VGPlayer __instance)
        {
            timer += Time.deltaTime;
            __instance.SetColor(Color.red, Color.white);
        }
}
public class NoDamage : CustomEvent
{
    public override void EventTriggered(List<string> data)
    {
        if (data[0] == "false")
            HitPatch.TakesDamage = false;
        else
            HitPatch.TakesDamage = true;
    }

    public override string EventName => "Takes_Damage";
}
public class IsGay : CustomEvent
{
    public override void EventTriggered(List<string> data)
    {
        if (data[0] == "false")
            HitPatch.gay = false;
        else
            HitPatch.gay = true;
    }

    public override string EventName => "IsGay";
}
