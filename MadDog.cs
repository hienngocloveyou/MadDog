﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using SharpDX;

namespace MadDog
{
    public class MadDog : BaseSettingsPlugin<MadDogSetting>
    {
  

        public override void OnLoad()
        {
            CanUseMultiThreading = true;
            //Graphics.InitImage("healthbar.png");
        }

        public override bool Initialise()
        {
            //Player = GameController.Player;
            //camera = new Camera(Player, Settings);

            //ReadIgnoreFile();
            base.Initialise();
            Name = "Testing";

            return true;
        }

        private void ReadIgnoreFile()
        {
           
        }

        public override void AreaChange(AreaInstance area)
        {
            
            ReadIgnoreFile();
        }

       

        

        public override Job Tick()
        {

            Settings.Cameras.Height.Value = 15;

            return null;
        }

        

        public override void Render()
        {
            
            

            Settings.Cameras.Height.Value = 15;

        }

        
    }
}
