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

            Dictionary<string, object> newEvents = new Dictionary<string, object>();
            Dictionary<string, object> newTriggers = new Dictionary<string, object>();

            if (RegisterTriggerEvents.ModdedEvents.Count != 0)
            {
                var Enu = RegisterTriggerEvents.ModdedEvents.GetEnumerator();
                while (Enu.MoveNext())
                {
                    newEvents.Add(Enu.Current.Value.EventName, Plugin.Inst.DefaultEventsCount);
                }

                Enu.Dispose();
                
                EnumInjector.InjectEnumValues(typeof(EventType), newEvents);
            }
            if (RegisterTriggerEvents.ModdedTriggers.Count != 0)
            {
                var Enu = RegisterTriggerEvents.ModdedTriggers.GetEnumerator();
                while (Enu.MoveNext())
                {
                    newTriggers.Add(Enu.Current, Plugin.Inst.DefaultTriggersCount + 1); // for some reason +1 has to be here but not on events??
                }
                Enu.Dispose();
                
                EnumInjector.InjectEnumValues(typeof(TriggerType), newTriggers);
            }
        }
    } 
    
    //so ideally we override EventTriggered, but the data parameter gives an exception when you try to access it for some reason.
    //so CallEvent is the way
    [HarmonyPatch(typeof(GameManager2))]
    internal class TriggerCustomEvent
    {
        [HarmonyPatch(nameof(GameManager2.CallEventRaw))]
        [HarmonyPrefix]
        public static bool CallRawPre(ref GameManager2 __instance, ref Trigger _trigger)
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
        [HarmonyPatch(nameof(GameManager2.CallEvent))]
        [HarmonyPrefix]
        public static bool CallPre(ref GameManager2 __instance, ref Trigger _trigger)
        {
            
            float startTime = _trigger.EventTriggerTime.x;
            float endTime = _trigger.EventTriggerTime.y;

            // make it so you can have start and end values 
            if (startTime == -1) startTime = 0;
            if (endTime == -1) endTime = 6000;

            // skip if not in time range
            if (__instance.CurrentSongTimeSmoothed < startTime || __instance.CurrentSongTimeSmoothed > endTime) return false;

            __instance.CallEventRaw(_trigger);

            return false;
        }
    }
}

