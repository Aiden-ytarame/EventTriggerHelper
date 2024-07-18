using HarmonyLib;
using Il2CppInterop.Runtime;
using static DataManager.GameData.BeatmapData.EventTriggers;

namespace TriggerAPI.Patchers
{

    [HarmonyPatch(typeof(TriggerDialog))]
    internal class SetupEventDataOnEditor
    {
        /// <summary>
        /// adds custom triggers to the editor UI
        /// </summary>
        [HarmonyPatch(nameof(TriggerDialog.OnStart))]
        [HarmonyPostfix]
        public static void StartTriggersPost()
        {
            foreach (var keyValuePair in RegisterTriggerEvents.EventsData)
            {
                Il2CppSystem.Collections.Generic.List<string> newData = new();
                keyValuePair.Value.ForEach(x =>
                {
                    newData.Add(x);
                }); //is there a way to cast System List to Il2cppSystem List?
                
                DataManager.inst.gameData.beatmapData.eventTriggers.EventTypeData.TryAdd(
                    (EventType)Il2CppType.Of<EventType>().GetEnumNames().IndexOf(keyValuePair.Key), newData);
            }
        }
    }

    [HarmonyPatch(typeof(GameManager))]
    internal class SetupEventTypes
    {
        /// <summary>
        /// loads the triggers and events
        /// </summary>
        [HarmonyPatch(nameof(GameManager.PlayGame))]
        [HarmonyPrefix]
        public static void PrePlay()
        {
            foreach (var trigger in DataManager.inst.gameData.beatmapData.eventTriggers.triggers)
            {
                //if you download multiple events mods this ensures it has the right enum id.
                if (trigger.EventData.Count == 0)
                    return;

                if (trigger.EventData[0].Contains("(CustomTrigger)"))
                {
                    Plugin.Logger.LogError(trigger.EventData[0].Substring(15));
                    var triggerIndex = RegisterTriggerEvents.GetTriggerEnumFromName(trigger.EventData[0].Substring(15));
                    
                    trigger.EventTrigger =
                        triggerIndex != -1
                            ? (TriggerType)triggerIndex
                            : (TriggerType)Plugin.Inst.DefaultTriggersCount; //if custom trigger is not registered, default to NULL
                    
                    trigger.EventData.RemoveAt(0);
                }
                if (trigger.EventData[0].Contains("(CustomEvent)"))
                {
                    Plugin.Logger.LogError(trigger.EventData[0].Substring(13));
                    int eventIndex = RegisterTriggerEvents.GetEventEnumFromName(trigger.EventData[0].Substring(13));
                    
                    trigger.EventType =
                        eventIndex != -1
                            ? (EventType)eventIndex
                            : (EventType)Plugin.Inst.DefaultEventsCount; //if custom event is not registered, default to LogEvent.
                    
                    trigger.EventData.RemoveAt(0);
                }
            }
        }
    }
}
