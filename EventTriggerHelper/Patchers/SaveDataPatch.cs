using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using static DataManager.GameData.BeatmapData.EventTriggers;

namespace TriggerAPI.Patchers
{
    [HarmonyPatch(typeof(DataManager))]
    internal class SaveEventType
    {
        [HarmonyPatch(nameof(DataManager.SaveData))]
        [HarmonyPrefix]
        //this saves the event type on a EventData, done so multiple mods don't overlap the enum ids of each other
        public static void PreSaveData()
        {
            foreach (var dataEntry in DataManager.inst.gameData.beatmapData.eventTriggers.triggers)
            {
                if ((int)dataEntry.EventType >= Plugin.Inst.DefaultEventsCount)
                {
                    dataEntry.EventData.Insert(0, $"(CustomEvent){Il2CppType.Of<EventType>().GetEnumNames()[(int)dataEntry.EventType]}");
                }
                if ((int)dataEntry.EventTrigger >= Plugin.Inst.DefaultTriggersCount)
                {
                    dataEntry.EventData.Insert(0, $"(CustomTrigger){Il2CppType.Of<TriggerType>().GetEnumNames()[(int)dataEntry.EventTrigger]}");
                }
            }
        }
}
    [HarmonyPatch(typeof(DataManager._SaveDataNew_d__113))]
    internal class SaveEventTypeCoroutine
    {
        [HarmonyPatch(nameof(DataManager._SaveDataNew_d__113.MoveNext))]
        [HarmonyPostfix]
        //removes the event type from event data, so it doesn't show up on the level editor
        public static void PostSaveData()
        {
            foreach (var dataEntry in  DataManager.inst.gameData.beatmapData.eventTriggers.triggers)
            {
                if ((int)dataEntry.EventTrigger >= Plugin.Inst.DefaultTriggersCount)
                {
                    dataEntry.EventData.RemoveAt(0);
                }
   
                if ((int)dataEntry.EventType >= Plugin.Inst.DefaultEventsCount)
                {
                    dataEntry.EventData.RemoveAt(0);
                }
            }
        }
    }
}