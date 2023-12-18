using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace PartialDeathdrop
{
    public class PartialDeathdrop : RocketPlugin<PdRegionPolicies>
    {
        private Mutex deathHandlerMutex;
        private MethodInfo inventoryLifeUpdateMethod;
        private Dictionary<Player, Tuple<LifeUpdated, List<LifeUpdated>>> listenerModifyRecord;

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "", "" }
        };

        public PdRegionPolicies PdRP => Configuration.Instance;

        protected override void Load()
        {
            Configuration.Load();
            listenerModifyRecord = new Dictionary<Player, Tuple<LifeUpdated, List<LifeUpdated>>>();
            inventoryLifeUpdateMethod = typeof(PlayerInventory).GetMethod("onLifeUpdated", BindingFlags.NonPublic);
            if (inventoryLifeUpdateMethod == null)
                throw new PdUnhookException("PlayerInventory.onLifeUpdated method not found.");

            deathHandlerMutex = new Mutex();
            U.Events.OnPlayerConnected += EventsOnPlayerConnected;
        }

        private void EventsOnPlayerConnected(UnturnedPlayer player)
        {
            var p = player.Player;

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
                    Policy? previousPolicy = null, newPolicy = PdRP.GetCorrespondingPolicyFor(p);

                    deathHandlerMutex.WaitOne();
                    Logger.Log("Hook hit for player " + p.name);
                    if (newPolicy.HasValue)
                    {
                        previousPolicy = Policy.FromServerPresent();
                        newPolicy.Value.EnableToServer();
                    }

                    inventoryLifeUpdateMethod.Invoke(p.inventory, new object[] { true });
                    if (newPolicy.HasValue) previousPolicy.Value.EnableToServer();

                    deathHandlerMutex.ReleaseMutex();
                }
                else
                {
                    inventoryLifeUpdateMethod.Invoke(p.inventory, new object[] { false });
                }
            };
            listenerModifyRecord.Add(p, Tuple.Create(newLifeUpdated, removeListenerList));

            p.life.onLifeUpdated += newLifeUpdated;
        }

        protected override void Unload()
        {
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
    }
}