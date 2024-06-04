using System;
using HarmonyLib;
using MovingTrigger;
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
        TriggerType CloseCallType = (TriggerType)TriggerAPI.RegisterTriggerEvents.GetTriggerEnumFromName("Close_Call");
        
        VGPlayerManager.Inst.players.ForEach((System.Action<VGPlayerManager.VGPlayerData>)(player =>
        {
            DataManager.inst.gameData.beatmapData.eventTriggers.triggers.ForEach( (Action<Trigger>)(trigger =>
            {
                if (trigger.EventTrigger == CloseCallType)
                {
                    player.PlayerObject.add_CloseCallEvent(new Action<Vector3>(_pos => { GameManager.Inst.CallEvent(trigger);}));
                }
            }));
        }));
        
    }
}

