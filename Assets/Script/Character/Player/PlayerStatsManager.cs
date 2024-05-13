using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();

            // why cacluate these here?
            // When we make a character creation menu, and set the stats depending on the class, this will be calculated therer
            // until then however, stats are never calculated, so we do it here on start, if a save file exists they will be over written when loading into a scene
            CalcuateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
            CalcuateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }
    }
}
