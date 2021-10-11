using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using SharpDX;

namespace MadDog
{
    public class Camera
    {
        public Entity Entity { get; }
        public UnitSettings Settings { get; private set; }
        public Camera(Entity entity, MadDogSetting settings)
        {
            Entity = entity;
            
            Update(entity, settings);
        }

        public void Update(Entity entity, MadDogSetting settings)
        {
            
        }
    }
}
