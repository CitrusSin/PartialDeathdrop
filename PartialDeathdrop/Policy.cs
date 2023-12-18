using SDG.Unturned;

namespace PartialDeathdrop
{
    public struct Policy
    {
        public bool LoseWeaponsPvP;
        public bool LoseWeaponsPvE;
        public bool LoseClothesPvP;
        public bool LoseClothesPvE;
        public float LoseItemsPvP;
        public float LoseItemsPvE;

        public void EnableToServer()
        {
            Provider.modeConfigData.Players.Lose_Weapons_PvP = LoseWeaponsPvP;
            Provider.modeConfigData.Players.Lose_Weapons_PvE = LoseWeaponsPvE;
            Provider.modeConfigData.Players.Lose_Clothes_PvP = LoseClothesPvP;
            Provider.modeConfigData.Players.Lose_Clothes_PvE = LoseClothesPvE;
            Provider.modeConfigData.Players.Lose_Items_PvP = LoseItemsPvP;
            Provider.modeConfigData.Players.Lose_Items_PvE = LoseItemsPvE;
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