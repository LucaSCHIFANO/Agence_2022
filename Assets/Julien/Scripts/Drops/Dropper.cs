using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Dropper : NetworkBehaviour
{
    [SerializeField] private List<Drops> _dropsList;
    [SerializeField] private TruckReference _truckReference;

    public void DropperDied()
    {
        if (Runner.IsServer)
        {
            // Drop ici
            float chance = Random.value;

            foreach (Drops drop in _dropsList)
            {
                if (chance <= drop.DropChance)
                {
                    if (drop.DropObject)
                    {
                        // Runner.Spawn()
                    }
                    else
                    {
                        if (drop.AddFuel)
                        {
                            _truckReference.Acquire().GetComponent<TruckFuel>().AddFuel(drop.AmountDropped);
                        }

                        if (drop.AddScrap)
                        {
                            ScrapMetal.Instance.AddServerScrap(Mathf.CeilToInt(drop.AmountDropped));
                        }
                    }
                }
            }
        }
    }
}
