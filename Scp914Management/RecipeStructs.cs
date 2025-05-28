using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Scp914;

namespace Scp914Management
{
    public class Probability<T>
    {
        public T result { get; set; }
        public int rate { get; set; }
        public int amount { get; set; }
    }
    public class BaseItemRecipe
    {
        public ItemType Origin { get; set; }
        public List<Probability<ItemType>> Target { get; set; }
        public Scp914KnobSetting Mode { get; set; }
        public bool DestoryByDefault { get; set; } = true;
        public virtual bool Match(ItemType origin) => Origin == origin;
        public virtual void ProcessPickupCrafting(Scp914ProcessingPickupEventArgs ev)
        {
            int sumrate = 0;
            foreach (var item in Target) 
            {
                sumrate += item.rate;
            }
            int num;
            if (sumrate > 100) num = new Random().Next(1, sumrate + 1);
            else num = new Random().Next(1, 101);
            Logger.Info(num);
            foreach (var item in Target)
            {
                if (num - item.rate <= 0)
                {
                    ev.IsAllowed = false;
                    for (int i = 0; i < item.amount; ++i)
                    {
                        Pickup.Create(item.result, ev.NewPosition).Spawn();
                    }
                    ev.Pickup.Destroy();
                    return;
                }
                else num -= item.rate;
            }
            if (DestoryByDefault) ev.Pickup.Destroy();
            else
            {
                ev.IsAllowed = false;
                ev.Pickup.Position = ev.NewPosition;
            }
        }
        public virtual void ProcessInventoryCrafting(Scp914ProcessingInventoryItemEventArgs ev)
        {
            int sumrate = 0;
            foreach (var item in Target)
            {
                sumrate += item.rate;
            }
            int num;
            if (sumrate > 100) num = new Random().Next(1, sumrate + 1);
            else num = new Random().Next(1, 101);
            Logger.Info(num);
            foreach (var item in Target)
            {
                if (num - item.rate <= 0)
                {
                    ev.IsAllowed = false;
                    ev.Player.RemoveItem(ev.Item);
                    for (int i = 0; i < item.amount; ++i)
                    {
                        if (!ev.Player.IsInventoryFull) ev.Player.AddItem(item.result);
                        else Pickup.Create(item.result, Scp914Controller.Singleton.OutputChamber.position).Spawn();
                    }
                    return;
                }
                else num -= item.rate;
            }
            ev.IsAllowed = false;
            if (DestoryByDefault) ev.Player.RemoveItem(ev.Item);
        }
    }
}
