using System.Collections.Generic;

namespace Scp914Management
{
    public class Config
    {
        public List<BaseItemRecipe> ItemRecipes { get; set; } = new List<BaseItemRecipe>() 
        { 
            new BaseItemRecipe 
            {
                Origin = ItemType.Flashlight,
                Target = new List<Probability<ItemType>>
                {
                    new Probability<ItemType> { result = ItemType.GrenadeFlash, amount = 1, rate = 75 },
                    new Probability<ItemType> { result = ItemType.SCP2176, amount = 1, rate = 10 },
                    new Probability<ItemType> { result = ItemType.Jailbird, amount = 1, rate = 1 },
                    new Probability<ItemType> { result = ItemType.SCP207, amount = 1, rate = 4 }
                },
                Mode = Scp914.Scp914KnobSetting.VeryFine,
                DestoryByDefault = true
            },
            new BaseItemRecipe
            {
                Origin = ItemType.KeycardO5,
                Target = new List<Probability<ItemType>>
                {
                    new Probability<ItemType> { result = ItemType.KeycardFacilityManager, amount = 1, rate = 75 },
                    new Probability<ItemType> { result = ItemType.MicroHID, amount = 1, rate = 5 }
                },
                Mode = Scp914.Scp914KnobSetting.Rough,
                DestoryByDefault = false
            }
        };
    }
}
