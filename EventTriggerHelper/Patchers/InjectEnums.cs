using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using static DataManager.GameData.BeatmapData.EventTriggers;

namespace TriggerAPI.Patchers
{
    [HarmonyPatch(typeof(GameManager2))]
    internal class StartTriggers
    {
        [HarmonyPatch(nameof(GameManager2.Start))]
        [HarmonyPrefix]
        static void StartTriggersPre()
        {
            if (Plugin.Inst.HasInitializedEvents)
                return;

            Plugin.Inst.HasInitializedEvents = true;

            Plugin.Inst.Log.LogWarning("STARTED GM2");

            Dictionary<string, object> newEvents = new Dictionary<string, object>();
            if (RegisterTriggerEvents.ModdedEvents.Count == 0)
                return;
            
            var Enu = RegisterTriggerEvents.ModdedEvents.GetEnumerator();
            while (Enu.MoveNext())
            {
                newEvents.Add(Enu.Current.Value.EventName, 7);
            }
            Enu.Dispose();

            EnumInjector.InjectEnumValues(typeof(EventType), newEvents);
        }
    } 
    
    [HarmonyPatch(typeof(GameManager2))]
    internal class TriggerCustomEvent
    {
        [HarmonyPatch(nameof(GameManager2.CallEventRaw))]
        [HarmonyPrefix]
        public static bool StartTriggersPost(ref GameManager2 __instance, ref Trigger _trigger)
        {
            if ((int)_trigger.EventType < Plugin.Inst.DefaultEventsCount)
                return true;
            
            if (_trigger.EventTriggerRetrigger != -1)
            {
                // skip if it has the key and has already triggered
                if (__instance.hasTriggered.ContainsKey(_trigger.ID) &&
                    __instance.hasTriggered[_trigger.ID] >= _trigger.EventTriggerRetrigger) return false;

                if (__instance.hasTriggered.ContainsKey(_trigger.ID))
                    __instance.hasTriggered[_trigger.ID] += 1;
                // setup triggered state
                else
                    __instance.hasTriggered.Add(_trigger.ID, 1);
            }

            Plugin.Inst.Log.LogInfo($"Custom Event Triggered: {Il2CppType.Of<EventType>().GetEnumNames()[(int)_trigger.EventType]}"); 
            
            if(RegisterTriggerEvents.ModdedEvents.TryGetValue(Il2CppType.Of<EventType>().GetEnumNames()[(int)_trigger.EventType], out CustomEvent customEvent))
            {
                customEvent.EventTriggered(_trigger.EventData);
            }
            else
            {
                Plugin.Inst.Log.LogError("Invalid Custom Event!");
            }

            return false;
        }
    }
}

