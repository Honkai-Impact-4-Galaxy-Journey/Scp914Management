using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;

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
    public class PluginMain : Plugin<Config>
    {
        public override string Name => "Scp914Management";

        public override string Description => "A plugin that can add new crafting recipes to 914";

        public override string Author => "Honkai Impact 4: Galaxy Journey";

        public override Version Version => new Version("1.0.0.0");

        public override Version RequiredApiVersion => new Version("0.0.0.0");

        public override void Disable()
        {
            Scp914Handler.OnDisabled();
            Scp914Handler.itemRecipes.Clear();
        }
        public override void Enable()
        {
            Scp914Handler.OnEnabled();
            Scp914Handler.RegisterRecipes(Config.ItemRecipes);
            Logger.Info("Plugin Loaded");
        }
    }
    public class Scp914Handler
    {
        public static List<BaseItemRecipe> itemRecipes = new List<BaseItemRecipe>();
        public static void OnEnabled()
        {
            Scp914Events.ProcessingInventoryItem += OnInventoryItemCrafting;
            Scp914Events.ProcessingPickup += OnItemCrafting;
        }
        public static void OnDisabled()
        {
            Scp914Events.ProcessingInventoryItem -= OnInventoryItemCrafting;
            Scp914Events.ProcessingPickup -= OnItemCrafting;
        }
        public static void RegisterRecipes(params BaseItemRecipe[] recipes)
        {
            foreach (var recipe in recipes)
            {
                itemRecipes.Add(recipe);
            }
        }
        public static void RegisterRecipes(IEnumerable<BaseItemRecipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                itemRecipes.Add(recipe);
            }
        }
        public static void OnItemCrafting(Scp914ProcessingPickupEventArgs ev)
        {
            BaseItemRecipe recipe = (from item in itemRecipes
                                    where item.Match(ev.Pickup.Type) && item.Mode == ev.KnobSetting
                                    select item).FirstOrDefault();
            recipe?.ProcessPickupCrafting(ev);
        }
        public static void OnInventoryItemCrafting(Scp914ProcessingInventoryItemEventArgs ev)
        {
            BaseItemRecipe recipe = (from item in itemRecipes
                                     where item.Match(ev.Item.Type) && item.Mode == ev.KnobSetting
                                     select item).FirstOrDefault();
            recipe?.ProcessInventoryCrafting(ev);
        }
    }
}
