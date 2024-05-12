using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace MovingTrigger;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
[BepInDependency("me.ytarame.TriggerHelper")]

public class Plugin : BasePlugin
{
    public static Plugin Inst;
    
    Harmony _harmony;
    const string Guid = "me.ytarame.CloseCallTrigger";
    const string Name = "CloseCallTrigger";
    const string Version = "1.0.0";


    public override void Load()
    {
        TriggerAPI.RegisterTriggerEvents.RegisterCustomTrigger("Close_Call");
        
        Inst = this;
        _harmony = new Harmony(Guid);
        _harmony.PatchAll();

        // Plugin startup logic
        Log.LogInfo($"Plugin {Guid} is loaded!");
    }
    
}
