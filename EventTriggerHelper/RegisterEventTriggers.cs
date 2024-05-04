using System.Collections.Generic;
using Il2CppInterop.Runtime;

namespace TriggerAPI
{
    /// <summary>
    /// Static class used for registering new Events.
    /// Call the static function RegisterCustomEvent().
    /// </summary>
    public static class RegisterTriggerEvents
    {
        internal static Dictionary<string, CustomEvent> ModdedEvents { get; private set; }
        internal static Dictionary<string, List<string>> EventsData{ get; private set; }
        internal static List<string> ModdedTriggers{ get; private set; }
        

/// <summary>
/// <param name="customEvent">Pass in an instance of a class that inherits from the abstract class CustomEvent</param>
/// <param name="eventData">The Event Data field names that show up in the Level Editor</param>
/// <returns>Was successful</returns>
/// <example>RegisterCustomEvent(new ExampleLogEvent(), new List{string}(){"Message to Log"});</example>
/// </summary>
        public static bool RegisterCustomEvent(CustomEvent customEvent, List<string> eventData = null)
        {
            if (ModdedEvents == null)
            {
                ModdedEvents = new Dictionary<string, CustomEvent>();
                EventsData = new Dictionary<string, List<string>>();
            }
            
            if(eventData != null)
                EventsData.TryAdd(customEvent.EventName, eventData);
            else
                EventsData.TryAdd(customEvent.EventName, new(){"Misc Data Field"});
            
            return ModdedEvents.TryAdd(customEvent.EventName, customEvent);
        }

        /// <summary>
        /// <param name="triggerName">The Name of your trigger. please follow variable naming rules, such as no spaces and not starting with a number</param>
        /// <returns>Was successful</returns>
        /// <example>RegisterCustomTrigger("Dodged_Hit");</example>
        /// </summary>
        public static bool RegisterCustomTrigger(string triggerName)
        {
            if (ModdedTriggers == null)
            {
                ModdedTriggers = new List<string>();
            }

            if (ModdedTriggers.Contains(triggerName))
            {
                Plugin.Inst.Log.LogError($"Trigger [{triggerName}] already exists!");
                return false;
            }
            ModdedTriggers.Add(triggerName);
            return true;
        }

        public static int GetTriggerEnumFromName(string triggerName)
        {
            return Il2CppType.Of<DataManager.GameData.BeatmapData.EventTriggers.TriggerType>().GetEnumNames()
                .IndexOf(triggerName);
        }
        public static int GetEventEnumFromName(string eventName)
        {
            return Il2CppType.Of<DataManager.GameData.BeatmapData.EventTriggers.EventType>().GetEnumNames()
                .IndexOf(eventName);
        }
    }
    /// <summary>
    /// Custom events should Inherit this class
    /// </summary>
    public abstract class CustomEvent
    {
        /// <summary>
        /// <para>The Name of your Event, it's also the name of the Enum entry.</para>
        /// The name Should follow Enum naming rules, such as no spaces or not starting with a number
        /// <example> Spawn_Prefab </example>
        /// </summary>
        public abstract string EventName { get; }
        
        ///<summary>
        /// The function called when the event is Triggered.
        /// <param name="data"> The data defined by the user in the level editor</param>
        /// </summary>
        public abstract void EventTriggered(Il2CppSystem.Collections.Generic.List<string> data);
    }

}
