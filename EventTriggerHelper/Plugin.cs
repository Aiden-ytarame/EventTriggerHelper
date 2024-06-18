using System;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Collections.Generic;
using BepInEx.Logging;

namespace TriggerAPI;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
public class Plugin : BasePlugin
{
    public static Plugin Inst;
    public static ManualLogSource Logger;
    
    internal bool HasInitializedEvents = false;
    public int DefaultEventsCount { get; private set; }
    public int DefaultTriggersCount { get; private set; }
    
    Harmony _harmony;
    const string Guid = "me.ytarame.TriggerHelper";
    const string Name = "TriggerAPI";
    const string Version = "1.0.0";


    public override void Load()
    {     
        RegisterTriggerEvents.Init();
        RegisterTriggerEvents.RegisterCustomEvent(new LogCustomEvent(), new List<string>(){"Messages to Log"});
        
        DefaultEventsCount = Enum.GetValues<DataManager.GameData.BeatmapData.EventTriggers.EventType>().Length;
        DefaultTriggersCount = Enum.GetValues<DataManager.GameData.BeatmapData.EventTriggers.TriggerType>().Length;

        Inst = this;
        Logger = Log;
        _harmony = new Harmony(Guid);
        _harmony.PatchAll();
        
        // Plugin startup logic
        Log.LogInfo($"Plugin {Guid} is loaded!");
    }


}

public class LogCustomEvent : CustomEvent
{
    public override string EventName => "Log_Message";
    public override void EventTriggered(Il2CppSystem.Collections.Generic.List<string> data)
    {
        Plugin.Logger.LogError("Log Event or an event not registered was triggered!");
        
        foreach (var message in data)
        {
            Plugin.Logger.LogInfo(message);
        }
    }
}