using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;

namespace PartialDeathdrop
{
    public class RegionNoticer
    {
        private PartialDeathdrop inst;
        private Dictionary<ulong, RegionPolicy> lastRegions = new Dictionary<ulong, RegionPolicy>();

        public RegionNoticer(PartialDeathdrop instance)
        {
            inst = instance;
        }
        
        public void RegisterEvents()
        {
            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
        }

        public void UnregisterEvents()
        {
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= OnPlayerUpdatePosition;
        }
        
        private void OnPlayerConnected(UnturnedPlayer player)
        {
            var rp = inst.PdRP.GetCorrespondingRPFor(player);
            lastRegions[player.CSteamID.m_SteamID] = rp;

            if (rp != null)
            {
                UnturnedChat.Say(player, inst.Translate("in_region"));
                inst.TellPolicyLocalized(player, rp.PolicyUsing);
            }
        }
        
        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            lastRegions.Remove(player.CSteamID.m_SteamID);
        }

        private void OnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {
            var rp = inst.PdRP.GetCorrespondingRPFor(player);
            if (rp == null)
            {
                if (lastRegions[player.CSteamID.m_SteamID] != null)
                {
                    UnturnedChat.Say(player, inst.Translate("leave_region"), Color.blue);
                }
                lastRegions[player.CSteamID.m_SteamID] = null;
                return;
            }

            if (rp != lastRegions[player.CSteamID.m_SteamID])
            {
                UnturnedChat.Say(player, inst.Translate("enter_region"), Color.blue);
                inst.TellPolicyLocalized(player, rp.PolicyUsing);
                lastRegions[player.CSteamID.m_SteamID] = rp;
            }
        }
    }
}