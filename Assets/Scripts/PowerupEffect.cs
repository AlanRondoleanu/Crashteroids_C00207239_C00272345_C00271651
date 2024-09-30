using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PowerupEffect
{
    // Start is called before the first frame update
    ShipSpeed playerSpeed;
    float speedBonus;
    bool active = false;
    const float speedMultiplier = 0.5f;

    const float PowerupTimeout = 4;
    float currentTime = 0;
    public PowerupEffect(ShipSpeed t_playerSpeed)
    {
        playerSpeed = t_playerSpeed;
        speedBonus = (playerSpeed.Speed * speedMultiplier);
        playerSpeed.Speed += speedBonus;
        active = true;
    }

    public void Update(float dt)
    {
        currentTime += dt;

        if (currentTime > PowerupTimeout)
        {
            playerSpeed.Speed -= speedBonus;
            active = false;
        }
    }

    public bool isActive()
    {
        return active;
    }
}
