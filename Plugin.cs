using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using TriggerAPI;

namespace TriggersAPI;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
public class Plugin : BasePlugin
{
    public static Plugin Instance;
    public bool HasInitializedEvents = false;
    Harmony harmony;
    const string Guid = "me.ytarame.TriggerHelper";
    const string Name = "TriggerAPI";
    const string Version = "0.0.1";


    public override void Load()
    {     
        RegisterTriggerEvents.RegisterCustomEvent(new ExampleLogEvent(), new List<string>(){"Message to Log"});
        
        Instance = this;
        harmony = new Harmony(Guid);
        harmony.PatchAll();

        // Plugin startup logic
        Log.LogInfo($"Plugin {Guid} is loaded!");
    }


}

public class ExampleLogEvent : CustomEvent
{
    public override string EventName => "Log_Message";
    public override void EventTriggered(Il2CppSystem.Collections.Generic.List<string> data)
    {
        Plugin.Instance.Log.LogError("Log Event or an event not registered was triggered!");
        
        foreach (var message in data)
        {
            Plugin.Instance.Log.LogInfo(message);
        }
    }
}