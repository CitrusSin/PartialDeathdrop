using System.Collections.Generic;
using Rocket.API;
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

        public Policy? GetCorrespondingPolicyFor(Player player)
        {
            foreach (var rp in CustomRegions)
                if (rp.ValidRegion.InsideFor(player.transform.position))
                    return rp.PolicyUsing;

            return null;
        }
    }
}