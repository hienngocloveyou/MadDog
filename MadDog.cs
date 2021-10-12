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
using GameOffsets;

namespace MadDog
{
    public class MadDog : BaseSettingsPlugin<MadDogSetting>
    {

        
        private Camera camera;
        private CameraOffsets cameraOffsets;
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

            camera = GameController.Game.IngameState.Camera;
            cameraOffsets = GameController.Game.IngameState.Camera.CameraOffsets;
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

           

            return null;
        }

        

        public override void Render()
        {

            //if (!Settings.Enable) return;
            updateCamera();

        }

        private void updateCamera()
        {

            //Settings.Cameras.Height.Value = camera.CameraOffsets.ZFar;
            //camera.HalfHeight = Settings.Cameras.Height.Value;
            cameraOffsets.ZFar = Settings.Cameras.Height.Value;
        }



    }
}
