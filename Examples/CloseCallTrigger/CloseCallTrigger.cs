using System;
using HarmonyLib;
using TriggerAPI;
using UnityEngine;
using static DataManager.GameData.BeatmapData.EventTriggers;

namespace CloseCallTrigger;



[HarmonyPatch(typeof(GameManager))]
public class GameManager_Patch
{
    [HarmonyPatch(nameof(GameManager.SetupPlayerEventTriggers))]
    [HarmonyPostfix]
    static void PostSetup(ref GameManager __instance)
    {
        VGPlayerManager.Inst.players.ForEach((System.Action<VGPlayerManager.VGPlayerData>)(player =>
        {
            player.PlayerObject.add_CloseCallEvent(new Action<Vector3>(_ =>
            {
                RegisterTriggerEvents.CallCustomTrigger("Close_Call");
            }));
        }));
        
    }
}

