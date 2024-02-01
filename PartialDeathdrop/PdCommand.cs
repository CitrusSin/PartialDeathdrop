using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace PartialDeathdrop
{
    public class PdCommand : IRocketCommand
    {

        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "partialdeathdrop";
        public string Help => "Partial Deathdrop management command";
        public string Syntax => "/partialdeathdrop <options>";
        public List<string> Aliases => new List<string> { "pd" };
        public List<string> Permissions => new List<string> { "partialdeathdrop" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            PartialDeathdrop Pd = (PartialDeathdrop) R.Plugins.GetPlugin(Assembly.GetExecutingAssembly());

            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, "/partialdeathdrop <create/delete/set/list/now>");
                return;
            }
            
            string subCommand = command[0].ToLower();
            if (subCommand == "create")
            {
                if (!(caller is UnturnedPlayer player))
                {
                    UnturnedChat.Say(caller, Pd.Translate("not_a_player"));
                    return;
                }

                if (command.Length < 2 || !float.TryParse(command[1], out var radius))
                {
                    UnturnedChat.Say(player, Pd.Translate("argument_wrong", 2, "number"));
                    UnturnedChat.Say(player, "/partialdeathdrop create <radius>");
                    return;
                }

                var rp = new RegionPolicy(new RegionSphere(player.Position, radius));
                Pd.PdRP.CustomRegions.Add(rp);
                Pd.SaveConfiguration();
                Pd.TellRegionPolicy(player, rp);
            }
            else if (subCommand == "set")
            {
                if (command.Length < 3)
                {
                    UnturnedChat.Say(caller, "/partialdeathdrop set <index> <property name> <value>\nor \"/partialdeathdrop set <property name> <value>\" if in a region");
                    return;
                }
                var fieldName = command[1];
                var val = command[2];
                var valId = 2;
                var rp = caller is UnturnedPlayer player ? Pd.PdRP.GetCorrespondingRPFor(player) : null;
                if (rp == null)
                {
                    if (int.TryParse(command[1], out var id) && command.Length >= 4)
                    {
                        rp = Pd.PdRP.CustomRegions[id];
                        fieldName = command[2];
                        val = command[3];
                        valId = 3;
                    }
                    else
                    {
                        UnturnedChat.Say(caller,
                            Pd.Translate(caller is UnturnedPlayer ? "not_in_a_region" : "not_a_player"));
                        UnturnedChat.Say(caller, "/partialdeathdrop set <index> <property name> <value>\nor \"/partialdeathdrop set <property name> <value>\" if in a region");
                        return;
                    }
                }

                try
                {
                    var type = rp.PolicyUsing.GetEntryType(fieldName);
                    Type[] argTypes = { typeof(string), type.MakeByRefType() };
                    var method = type.GetMethod(
                        "TryParse", 
                        BindingFlags.Public | BindingFlags.Static, 
                        null, 
                        argTypes, 
                        null);
                    object[] args = { val, null };
                    if (method == null || !(bool)method.Invoke(null, args))
                    {
                        UnturnedChat.Say(caller, Pd.Translate("argument_wrong", valId, type.Name));
                        return;
                    }

                    rp.PolicyUsing.SetEntry(fieldName, args[1]);
                }
                catch (Exception e)
                {
                    UnturnedChat.Say(caller, Pd.Translate("set_failed"));
                    Logger.Log(e);
                    return;
                }

                Pd.TellRegionPolicy(caller, rp);
            }
            else if (subCommand == "delete")
            {
                var indexStr = command[1];
                if (!int.TryParse(indexStr, out int index))
                {
                    UnturnedChat.Say(caller, Pd.Translate("argument_wrong", 1, "int"));
                    UnturnedChat.Say(caller, "/partialdeathdrop delete <index>");
                    return;
                }

                Pd.PdRP.CustomRegions.RemoveAt(index);
            }
            else if (subCommand == "list")
            {
                UnturnedChat.Say(caller, Pd.Translate("region_list_count", Pd.PdRP.CustomRegions.Count), Color.blue);
                for (var index = 0; index < Pd.PdRP.CustomRegions.Count; index++)
                {
                    UnturnedChat.Say(caller, Pd.Translate("region_index", index), Color.cyan);
                    Pd.TellRegionPolicy(caller, Pd.PdRP.CustomRegions[index]);
                }
            }
            else if (subCommand == "now")
            {
                if (!(caller is UnturnedPlayer player))
                {
                    UnturnedChat.Say(caller, Pd.Translate("not_a_player"));
                    return;
                }
                int index = Pd.PdRP.CustomRegions.IndexOf(Pd.PdRP.GetCorrespondingRPFor(player));
                UnturnedChat.Say(caller, Pd.Translate("region_now", index));
            }
            else
            {
                UnturnedChat.Say(caller, "/partialdeathdrop <create/delete/set/list/now>");
            }
        }
    }
}
