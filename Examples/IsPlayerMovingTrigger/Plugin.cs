using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace InvisblePlayerEvent;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
[BepInDependency("me.ytarame.TriggerHelper")]

public class Plugin : BasePlugin
{
    public static Plugin Inst;
    
    Harmony _harmony;
    const string Guid = "me.ytarame.IsPlayerMovingTrigger";
    const string Name = "IsPLayerMovingTrigger";
    const string Version = "1.0.0";


    public override void Load()
    {
        TriggerAPI.RegisterTriggerEvents.RegisterCustomTrigger("Player_Moved");
        TriggerAPI.RegisterTriggerEvents.RegisterCustomTrigger("Player_Stopped");
        
        Inst = this;
        _harmony = new Harmony(Guid);
        _harmony.PatchAll();

        // Plugin startup logic
        Log.LogInfo($"Plugin {Guid} is loaded!");
    }
    
}
