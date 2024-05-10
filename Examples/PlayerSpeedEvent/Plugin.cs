using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace PlayerSpeedEvent;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]
[BepInDependency("me.ytarame.TriggerHelper")]

public class Plugin : BasePlugin
{
    public static Plugin Inst;
    
    Harmony _harmony;
    const string Guid = "me.ytarame.PlayerTriggerEvent";
    const string Name = "PlayerTriggerEvent";
    const string Version = "1.0.0";


    public override void Load()
    {
        TriggerAPI.RegisterTriggerEvents.RegisterCustomEvent(new PlayerSpeedCustomEvent()
            , new(){"Base Moving Speed | Default[20]", "Boost Moving Speed | Default[85]"});
        
        Inst = this;
        _harmony = new Harmony(Guid);
        _harmony.PatchAll();

        // Plugin startup logic
        Log.LogInfo($"Plugin {Guid} is loaded!");
    }
    
}
