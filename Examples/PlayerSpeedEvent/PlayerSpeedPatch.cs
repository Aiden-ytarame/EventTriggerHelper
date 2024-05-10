using Il2CppSystem.Collections.Generic;
using TriggerAPI;

namespace PlayerSpeedEvent;

public class PlayerSpeedCustomEvent : CustomEvent
{
    public override void EventTriggered(List<string> data)
    {
        float baseSpeed = 20;
        float boostSpeed = 85;

        if (data.Count == 0) return;

        float.TryParse(data[0], out baseSpeed);
        
        if (data.Count > 1)
            float.TryParse(data[1], out boostSpeed);
        
        VGPlayer.DEFAULT_SPEED = baseSpeed;
        VGPlayer.DEFAULT_BOOST_SPEED = boostSpeed;
    }

    public override string EventName => "Player_Speed";
}