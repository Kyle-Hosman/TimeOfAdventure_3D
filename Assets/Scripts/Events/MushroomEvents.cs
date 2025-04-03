using System;
using UnityEngine;

public class MushroomEvents
{
    public event Action<int> onMushroomGained;
    public void mushroomsCollected(int mushrooms) 
    {
        if (onMushroomGained != null) 
        {
            onMushroomGained(mushrooms);
        }
    }

    public event Action<int> onMushroomChange;
    public void MushroomChange(int mushrooms) 
    {
        if (onMushroomChange != null) 
        {
            onMushroomChange(mushrooms);
        }
    }
}
