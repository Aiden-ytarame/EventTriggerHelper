using System.Collections.Generic;

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

/// <summary>
/// <param name="customEvent">Pass in an instance of a class that inherits from the abstract class CustomEvent</param>
/// <param name="eventData">The Event Data field names that show up in the Level Editor</param>
/// <returns>Was successful</returns>
/// <example>RegisterCustomEvent{ExampleLogEvent, new List{string}(){"Message to Log"});</example>
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
    }
    /// <summary>
    /// Custom events should Inherit this class
    /// </summary>
    public abstract class CustomEvent
    {
        /// <summary>
        /// <para>The Name of your Event is the name of the Enum entry.</para>
        /// The name Should follow Enum naming rules, such as no spaces or not starting with a number
        /// <example> Spawn_Prefab </example>
        /// </summary>
        public abstract string EventName { get; }
        
        /// <param name="data"> The data defined by the user in the level editor</param>
        public abstract void EventTriggered(Il2CppSystem.Collections.Generic.List<string> data);
    }

}
