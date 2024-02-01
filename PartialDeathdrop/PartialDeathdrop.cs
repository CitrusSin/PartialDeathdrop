using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace PartialDeathdrop
{
    public class PartialDeathdrop : RocketPlugin<PdRegionPolicies>
    {
        private Mutex deathHandlerMutex;
        private MethodInfo inventoryLifeUpdateMethod;
        private Dictionary<Player, Tuple<LifeUpdated, List<LifeUpdated>>> listenerModifyRecord;
        private RegionNoticer regionNoticer;

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "not_a_player", "You're not a player therefore you're not allowed to use this command!" },
            { "argument_wrong", "Argument wrong! argument #{0} is expected to be a {1}." },
            { "not_in_a_region", "You're not in a created region!"},
            { "region_list_count", "There are {0} regions in total: " },
            { "region_index", "Region #{0}" },
            { "true_value", "Yes" },
            { "false_value", "No" },
            { "LoseWeaponsPvP", "PvP drop weapons" },
            { "LoseWeaponsPvE", "PvE drop weapons" },
            { "LoseClothesPvP", "PvP drop clothes" },
            { "LoseClothesPvE", "PvE drop clothes" },
            { "LoseItemsPvP", "PvP item drop rate" },
            { "LoseItemsPvE", "PvE item drop rate" },
            { "set_failed", "Unable to set field."},
            { "enter_region", "You are entering a region now with those properties below:" },
            { "leave_region", "You are leaving a region now." },
            { "in_region", "You are now in a region with those properties below:" },
            { "region_now", "You are now in region #{0}" }
        };

        public PdRegionPolicies PdRP => Configuration.Instance;

        public void SaveConfiguration()
        {
            Configuration.Save();
        }

        protected override void Load()
        {
            Configuration.Load();
            listenerModifyRecord = new Dictionary<Player, Tuple<LifeUpdated, List<LifeUpdated>>>();
            inventoryLifeUpdateMethod = typeof(PlayerInventory).GetMethod("onLifeUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
            if (inventoryLifeUpdateMethod == null)
                throw new PdUnhookException("PlayerInventory.onLifeUpdated method not found.");

            deathHandlerMutex = new Mutex();
            U.Events.OnPlayerConnected += EventsOnPlayerConnected;

            regionNoticer = new RegionNoticer(this);
            regionNoticer.RegisterEvents();
        }
        
        
        protected override void Unload()
        {
            regionNoticer.UnregisterEvents();
            
            U.Events.OnPlayerConnected -= EventsOnPlayerConnected;
            Configuration.Save();
            if (listenerModifyRecord != null)
            {
                foreach (var (p, (newListener, oldListeners)) in listenerModifyRecord)
                {
                    p.life.onLifeUpdated -= newListener;
                    foreach (var dg in oldListeners)
                        p.life.onLifeUpdated += dg;
                }

                listenerModifyRecord.Clear();
            }

            deathHandlerMutex.Dispose();
        }

        private void EventsOnPlayerConnected(UnturnedPlayer player)
        {
            var p = player.Player;
            
            Logger.Log("Player " + p.name + " connected. Injecting...");

            var removeListenerList = p.life.onLifeUpdated.GetInvocationList()
                .Where(dg => dg.Method == inventoryLifeUpdateMethod)
                .Cast<LifeUpdated>()
                .ToList();
            foreach (var dg in removeListenerList)
                p.life.onLifeUpdated -= dg;

            LifeUpdated newLifeUpdated = dead =>
            {
                if (dead)
                {
                    Policy previousPolicy = null, newPolicy = PdRP.GetCorrespondingPolicyFor(p);

                    deathHandlerMutex.WaitOne();
                    Logger.Log("Hook hit for player " + p.name);
                    if (newPolicy != null)
                    {
                        previousPolicy = Policy.FromServerPresent();
                        newPolicy.EnableToServer();
                    }

                    inventoryLifeUpdateMethod.Invoke(p.inventory, new object[] { true });
                    if (newPolicy != null) previousPolicy.EnableToServer();

                    deathHandlerMutex.ReleaseMutex();
                }
                else
                {
                    inventoryLifeUpdateMethod.Invoke(p.inventory, new object[] { false });
                }
            };
            listenerModifyRecord.Add(p, Tuple.Create(newLifeUpdated, removeListenerList));

            p.life.onLifeUpdated += newLifeUpdated;
            
            Logger.Log("Player " + p.name + " inject complete.");
        }
        
        public void TellRegionPolicy(IRocketPlayer target, RegionPolicy rp)
        {
            string[] textLines = rp.ToString().Split('\n');
            for (var i = 1; i < textLines.Length; i += 2)
            {
                UnturnedChat.Say(target, textLines[i - 1] + "\n" + textLines[i], Color.yellow);
            }

            if (textLines.Length % 2 == 1)
            {
                UnturnedChat.Say(target, textLines.Last(), Color.yellow);
            }
        }

        public void TellPolicyLocalized(IRocketPlayer target, Policy policy)
        {
            var entries = policy.GetEntryNames();
            var textLines = new List<string>();
            foreach (var entry in entries)
            {
                var localizedEntry = Translate(entry);
                var val = policy.GetEntry(entry);
                string translated;
                if (val is bool b)
                {
                    translated = Translate(b ? "true_value" : "false_value");
                }
                else if (val is float f)
                {
                    translated = $"{f:0.000}";
                }
                else
                {
                    translated = val.ToString();
                }

                textLines.Add($"{localizedEntry}: {translated}");
            }
            
            for (var i = 1; i < textLines.Count; i += 2)
            {
                UnturnedChat.Say(target, textLines[i - 1] + "\n" + textLines[i], Color.yellow);
            }
            if (textLines.Count % 2 == 1)
            {
                UnturnedChat.Say(target, textLines.Last(), Color.yellow);
            }
        }
    }
}