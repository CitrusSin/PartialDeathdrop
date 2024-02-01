using System;
using System.Linq;
using System.Reflection;
using System.Text;
using SDG.Unturned;

namespace PartialDeathdrop
{
    public class Policy
    {
        public bool LoseWeaponsPvP;
        public bool LoseWeaponsPvE;
        public bool LoseClothesPvP;
        public bool LoseClothesPvE;
        public float LoseItemsPvP;
        public float LoseItemsPvE;

        public void SetEntry(string entryName, Object val)
        {
            var field = this.GetType().GetField(entryName);
            field.SetValue(this, val);
        }

        public Object GetEntry(string entryName)
        {
            var field = this.GetType().GetField(entryName);
            return field.GetValue(this);
        }
        
        public Type GetEntryType(string entryName)
        {
            Type p = this.GetType();
            FieldInfo field = p.GetField(entryName);
            return field.FieldType;
        }

        public string[] GetEntryNames()
        {
            return this.GetType().GetFields().Select(fi => fi.Name).ToArray();
        }

        public void EnableToServer()
        {
            Provider.modeConfigData.Players.Lose_Weapons_PvP = LoseWeaponsPvP;
            Provider.modeConfigData.Players.Lose_Weapons_PvE = LoseWeaponsPvE;
            Provider.modeConfigData.Players.Lose_Clothes_PvP = LoseClothesPvP;
            Provider.modeConfigData.Players.Lose_Clothes_PvE = LoseClothesPvE;
            Provider.modeConfigData.Players.Lose_Items_PvP = LoseItemsPvP;
            Provider.modeConfigData.Players.Lose_Items_PvE = LoseItemsPvE;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("LoseWeaponsPvP: {0}\n", LoseWeaponsPvP);
            sb.AppendFormat("LoseWeaponsPvE: {0}\n", LoseWeaponsPvE);
            sb.AppendFormat("LoseClothesPvP: {0}\n", LoseClothesPvE);
            sb.AppendFormat("LoseClothesPvE: {0}\n", LoseClothesPvE);
            sb.AppendFormat("LoseItemsPvP: {0:0.000}\n", LoseItemsPvP);
            sb.AppendFormat("LoseItemsPvE: {0:0.000}\n", LoseItemsPvE);
            return sb.ToString();
        }

        public static Policy FromServerPresent()
        {
            return new Policy
            {
                LoseWeaponsPvP = Provider.modeConfigData.Players.Lose_Weapons_PvP,
                LoseWeaponsPvE = Provider.modeConfigData.Players.Lose_Weapons_PvE,
                LoseClothesPvP = Provider.modeConfigData.Players.Lose_Clothes_PvP,
                LoseClothesPvE = Provider.modeConfigData.Players.Lose_Clothes_PvE,
                LoseItemsPvP = Provider.modeConfigData.Players.Lose_Items_PvP,
                LoseItemsPvE = Provider.modeConfigData.Players.Lose_Items_PvE
            };
        }
    }
}
