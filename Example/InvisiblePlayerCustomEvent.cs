using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TriggerAPI;
using UnityEngine;

namespace InvisblePlayerEvent;


//Just deActivating the objects gives off a null exception cuz its trying to update the player's colors.
//So I just move them out of screen
//I could disable their MeshRenderer, but isn't GetComponent expansive? plus moving it out of screen stops one of the boost effects from playing
public class InvisiblePlayerCustomEvent : CustomEvent
{
    public override void EventTriggered(List<string> data)
    {
        var playerEnu = VGPlayerManager.inst.players.GetEnumerator();
        while (playerEnu.MoveNext())
        {
            VGPlayer player = playerEnu.Current.PlayerObject;
            if(player?.Player_Wrapper == null)
                continue;
            
            player.Player_Wrapper.GetChild(0).localPosition = new Vector3(0, 9999, 0); //Player Core
            player.Player_Wrapper.GetChild(1).localPosition = new Vector3(0, 9999, 0); //Player Boost
            
            foreach (var trailTransform in player.Player_Trail.trail)
            {
                trailTransform.GetComponentInChildren<TrailRenderer>().enabled = false;
                trailTransform.GetChild(0).localPosition = new Vector3(0, 9999, 0);
            }
        }
        playerEnu.Dispose();
    }

    public override string EventName => "Turn_Player_Invisible";
}

public class VisiblePlayerCustomEvent : CustomEvent
{
    public override void EventTriggered(List<string> data)
    {
        var playerEnu = VGPlayerManager.inst.players.GetEnumerator();
        while (playerEnu.MoveNext())
        {
            VGPlayer player = playerEnu.Current.PlayerObject;
            if(player?.Player_Wrapper == null)
                continue;
            
            player.Player_Wrapper.GetChild(0).localPosition = new Vector3(0, 0, 0); //Player Core
            player.Player_Wrapper.GetChild(1).localPosition = new Vector3(0, 0, 0); //Player Boost
            
            foreach (var trailTransform in player.Player_Trail.trail)
            {
                trailTransform.GetComponentInChildren<TrailRenderer>().enabled = true;
                trailTransform.GetChild(0).localPosition = new Vector3(0, 0, 0);
            }
        }
        playerEnu.Dispose();
    }

    public override string EventName => "Turn_Player_Visible";
}

[HarmonyPatch(typeof(VGPlayer))]
internal class PlayParticlePatch
{
    [HarmonyPatch(nameof(VGPlayer.PlayParticles))]
    [HarmonyPrefix]
    public static bool PrePlayParticle(ref VGPlayer __instance, VGPlayer.ParticleTypes _type)
    {
        if (_type == VGPlayer.ParticleTypes.Boost && __instance.Player_Wrapper.GetChild(0).localPosition.y == 9999)
        {
            return false;
        }
        return true;
    }
}