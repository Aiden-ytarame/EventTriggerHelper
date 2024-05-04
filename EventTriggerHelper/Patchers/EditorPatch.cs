using HarmonyLib;
using Il2CppInterop.Runtime;
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
            foreach (var trigger in DataManager.inst.gameData.beatmapData.eventTriggers.triggers)
            {
                if ((int)trigger.EventType >= Plugin.Inst.DefaultEventsCount)
                {
                    //if you download multiple events mods this ensures it has the right enum id.
                    if (trigger.EventData.Count < 2)
                        return;

                    int triggerIndex = RegisterTriggerEvents.GetTriggerEnumFromName(trigger.EventData[0]);
                    
                    int eventIndex = RegisterTriggerEvents.GetEventEnumFromName(trigger.EventData[1]);
                   
                    trigger.EventTrigger = triggerIndex != -1 ? (TriggerType)triggerIndex : TriggerType.Time; //if custom trigger is not registered, default to Time
                    trigger.EventType = eventIndex != -1 ? (EventType)eventIndex : (EventType)5; //if custom event is not registered, default to LogEvent.

                    trigger.EventData.RemoveRange(0,2);
                }
            }
        }
    }
}
