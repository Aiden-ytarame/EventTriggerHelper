using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using static DataManager.GameData.BeatmapData.EventTriggers;

namespace TriggerAPI.Patchers
{
    [HarmonyPatch(typeof(GameManager))]
    internal class StartTriggers
    {
        [HarmonyPatch(nameof(GameManager.Start))]
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
                var eventEnu = RegisterTriggerEvents.ModdedEvents.GetEnumerator();
                while (eventEnu.MoveNext())
                {
                    newEvents.Add(eventEnu.Current.Value.EventName, 99);
                }
                eventEnu.Dispose();

                EnumInjector.InjectEnumValues(typeof(EventType), newEvents);
            }

            if (RegisterTriggerEvents.ModdedTriggers.Count != 0)
            {
                var triggerEnu = RegisterTriggerEvents.ModdedTriggers.GetEnumerator();
                while (triggerEnu.MoveNext())
                {
                    newTriggers.Add(triggerEnu.Current, 99);
                }
                triggerEnu.Dispose();

                EnumInjector.InjectEnumValues(typeof(TriggerType), newTriggers);
            }
        }
    }


    //so ideally we override EventTriggered, but the data parameter gives an exception when you try to access it for some reason.
    //so CallEvent is the way
    [HarmonyPatch(typeof(GameManager))]
    internal class TriggerCustomEvent
    {
        [HarmonyPatch(nameof(GameManager.CallEventRaw))]
        [HarmonyPrefix]
        public static bool CallRawPre(ref GameManager __instance, ref Trigger _trigger)
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
            
            if(RegisterTriggerEvents.ModdedEvents.TryGetValue(Il2CppType.Of<EventType>().GetEnumNames()[(int)_trigger.EventType], out CustomEvent customEvent))
            {
                customEvent.EventTriggered(_trigger.EventData);
            } 
            else
            {
                Plugin.Logger.LogError("Invalid Custom Event!");
            }

            return false;
        }
        [HarmonyPatch(nameof(GameManager.CallEvent))]
        [HarmonyPrefix]
        public static bool CallPre(ref GameManager __instance, ref Trigger _trigger)
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

