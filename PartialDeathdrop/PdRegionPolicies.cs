using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace PartialDeathdrop
{
    public class PdRegionPolicies : IRocketPluginConfiguration
    {
        public List<RegionPolicy> CustomRegions;

        public void LoadDefaults()
        {
            CustomRegions = new List<RegionPolicy>();
        }

        public RegionPolicy GetCorrespondingRPFor(UnturnedPlayer player)
        {
            foreach (var rp in CustomRegions)
                if (rp.ValidRegion.InsideFor(player.Player.transform.position))
                    return rp;

            return null;
        }

        public Policy GetCorrespondingPolicyFor(Player player)
        {
            foreach (var rp in CustomRegions)
                if (rp.ValidRegion.InsideFor(player.transform.position))
                    return rp.PolicyUsing;

            return null;
        }
    }
}