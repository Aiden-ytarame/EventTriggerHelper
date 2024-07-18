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
        /// <summary>
        /// where the modded enums are injected
        /// </summary>
        [HarmonyPatch(nameof(GameManager.Start))]
        [HarmonyPrefix]
        static void StartTriggersPre()
        {
            if (Plugin.Inst.HasInitializedEvents)
                return;

            Plugin.Inst.HasInitializedEvents = true;

            Dictionary<string, object> newEvents = new Dictionary<string, object>();
            Dictionary<string, object> newTriggers = new Dictionary<string, object>();

            foreach (var keyValuePair in RegisterTriggerEvents.ModdedEvents)
            {
                newEvents.Add(keyValuePair.Key, 99);
            }
            
            EnumInjector.InjectEnumValues(typeof(EventType), newEvents);
            
            
            foreach (var triggerString in RegisterTriggerEvents.ModdedTriggers)
            {
                newTriggers.Add(triggerString, 99);
            }

            EnumInjector.InjectEnumValues(typeof(TriggerType), newTriggers);

        }

        /// <summary>
        /// gets all triggers that use a custom trigger type and put them in a dictionary with the respective custom trigger
        /// </summary>
        [HarmonyPatch(nameof(GameManager.PlayGame))]
        [HarmonyPostfix]
        static void PostPlay()
        {
            RegisterTriggerEvents.Triggers.Clear();
            //handle trigger callbacks
            foreach (var triggerString in RegisterTriggerEvents.ModdedTriggers)
            {
                RegisterTriggerEvents.Triggers.TryAdd(triggerString, new List<Trigger>());

                var list = RegisterTriggerEvents.Triggers[triggerString];
                var type = (TriggerType)RegisterTriggerEvents.GetTriggerEnumFromName(triggerString);
                
                foreach (var trigger in DataManager.inst.gameData.beatmapData.eventTriggers.triggers)
                {
                    if (type == trigger.EventTrigger)
                        list.Add(trigger);
                }

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
        //needed?
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

