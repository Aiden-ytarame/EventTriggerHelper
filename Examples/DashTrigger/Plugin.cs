using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BoostTrigger;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
[BepInDependency("me.ytarame.TriggerHelper")]

public class Plugin : BasePlugin
{
    public static Plugin Inst;
    
    Harmony _harmony;
    const string Guid = "me.ytarame.BoostTrigger";
    const string Name = "BoostTrigger";
    const string Version = "1.0.0";


    public override void Load()
    {
        TriggerAPI.RegisterTriggerEvents.RegisterCustomTrigger("Player_Boost");
        TriggerAPI.RegisterTriggerEvents.RegisterCustomEvent(new NoDamage(), new() { "true or false" });
        TriggerAPI.RegisterTriggerEvents.RegisterCustomEvent(new IsGay(), new() { "true or false" });
        Inst = this;
        _harmony = new Harmony(Guid);
        _harmony.PatchAll();

        // Plugin startup logic
        Log.LogInfo($"Plugin {Guid} is loaded!");
    }
    
}
