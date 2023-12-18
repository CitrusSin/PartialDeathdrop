using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;

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
            if (command[0] == "create")
                if (!(caller is RocketPlayer))
                    UnturnedChat.Say(caller, "");
        }
    }
}