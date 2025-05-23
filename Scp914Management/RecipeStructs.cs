using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Features.Wrappers;

namespace Scp914Management
{
    public struct Probability<T>
    {
        public T item;
        public int rate, num;
    }
    public class BaseItemRecipe
    {
        public ItemType Origin { get; set; }
        public List<Probability<ItemType>> Target { get; set; }
        public bool DestoryByDefault { get; set; } = true;
        public virtual bool Match(ItemType origin) => Origin == origin;
        public virtual void ProcessCrafting(Scp914ProcessingPickupEventArgs ev)
        {
            int sumrate = 0;
            foreach (var item in Target) 
            {
                sumrate += item.rate;
            }
            int num;
            if (sumrate > 100) num = new Random().Next(1, sumrate + 1);
            else num = new Random().Next(1, 101);
            foreach (var item in Target)
            {
                if (num - item.rate <= 0)
                {
                    ev.IsAllowed = false;
                    for (int i = 0; i < item.num; ++i)
                    {
                        Pickup.Create(item.item, ev.NewPosition);
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
    }
}
