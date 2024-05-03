using HarmonyLib;
using Il2CppInterop.Runtime;
using TriggersAPI;
using static DataManager.GameData.BeatmapData.EventTriggers;

namespace TriggerAPI.Patchers
{

    [HarmonyPatch(typeof(TriggerDialog))]
    internal class SetupEventDataOnEditor
    {
        [HarmonyPatch(nameof(TriggerDialog.OnStart))]
        [HarmonyPostfix]
        public static void StartTriggersPost()
        {
            foreach (var keyValuePair in RegisterTriggerEvents.EventsData)
            {
                Il2CppSystem.Collections.Generic.List<string> newData =
                    new Il2CppSystem.Collections.Generic.List<string>();
                keyValuePair.Value.ForEach(x =>
                {
                    newData.Add(x);
                }); //is there a way to cast System List to Il2cppSystem List?

                DataManager.inst.gameData.beatmapData.eventTriggers.EventTypeData.TryAdd(
                    (EventType)Il2CppType.Of<EventType>().GetEnumNames().IndexOf(keyValuePair.Key), newData);
            }
        }
    }

    [HarmonyPatch(typeof(GameManager2))]
    internal class SetupEventTypes
    {
        [HarmonyPatch(nameof(GameManager2.PlayGame))]
        [HarmonyPrefix]
        public static void PrePlay()
        {
            foreach (var eventTriggersTrigger in DataManager.inst.gameData.beatmapData.eventTriggers.triggers)
            {
                if ((int)eventTriggersTrigger.EventType > 4)
                {
                    //if you download multiple events mods this ensures it has the right enum id.
                    if (eventTriggersTrigger.EventData.Count == 0)
                        return;

                    int index = Il2CppType.Of<EventType>().GetEnumNames()
                        .IndexOf(eventTriggersTrigger.EventData[0]);

                    //if custom trigger is not registered, default to LogEvent.
                    eventTriggersTrigger.EventType = index != -1 ? (EventType)index : (EventType)5;
 
                    eventTriggersTrigger.EventData.RemoveAt(0);
                }
            }
        }
    }
}