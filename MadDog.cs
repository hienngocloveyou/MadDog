using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using System.Windows.Forms;
using GameOffsets;

namespace MadDog
{
    public class MadDog : BaseSettingsPlugin<MadDogSetting>
    {
        private bool _aiming = false;
        private Vector2 monster_point;
        private Coroutine _mainCoroutine;
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly Stopwatch _aimTimer = Stopwatch.StartNew();
        Camera camera;
        Entity player;

        public override void OnLoad()
        {
            CanUseMultiThreading = true;
            //Graphics.InitImage("healthbar.png");
            camera = GameController.Game.IngameState.Camera;
            player = GameController.Player;
        }

        public override bool Initialise()
        {
            Input.RegisterKey(Keys.LButton);
            //Input.RegisterKey(Settings.EnableAim.Value);

            //ReadIgnoreFile();
            //_mainCoroutine = new Coroutine(MainCoroutine(),this,"EDC");
            //Core.ParallelRunner.Run(_mainCoroutine);
            
        

            return true;
        }

        private IEnumerator MainCoroutine()
        {
            while (true)
            {
                try
                {
                    
                }
                catch
                {
                    // ignored
                }


                if (Settings.Aimbot.Enable && player.IsAlive)
                {
                    if (!Input.IsKeyDown(Keys.LButton)
                   && !GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible
                   && !GameController.Game.IngameState.IngameUi.OpenLeftPanel.IsVisible)
                    {
                        _aiming = true;
                        yield return Attack();
                    }

                    if (Input.IsKeyDown(Keys.LButton) && _aiming == true)
                    {
                        Input.SetCursorPos(camera.WorldToScreen(player.Pos + new Vector3(0,-500,0)));
                        _aiming = false;

                    }
                    if(GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible || GameController.Game.IngameState.IngameUi.InventoryPanel.IsVisible)
                    {
                        _aiming = false;
                    }

                    yield return new WaitTime(10);
                }
                   
            }
            // ReSharper disable once IteratorNeverReturns
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
            if(Settings.Enable)
            {
                if (Settings.Aimbot.Enable)
                {
                    DrawEllipseToWorld(GetLocalPlayerPos(), Settings.Aimbot.Distance.Value, 25, 2, Color.LawnGreen);
                }
                
                FindMonsters();
                //RemoveMonsters();
                //DrawLineToMonster();
                

                
            }
            
            //Settings.Distance.distance.Value = (int)GetLocalPlayerPos().X;

        }

        
        private Vector3 GetLocalPlayerPos()
        {
            //Vector3 pos = GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Render>().Pos;
            Vector3 pos = GameController.Player.Pos;
            return pos;
        }

        
        private void DrawEllipseToWorld(Vector3 vector3Pos, int radius, int points, int lineWidth, Color color)
        {
            //var camera = GameController.Game.IngameState.Camera;
            var plottedCirclePoints = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var x = (decimal)vector3Pos.X + decimal.Multiply(radius, (decimal)Math.Cos(angle));
                var y = (decimal)vector3Pos.Y + decimal.Multiply(radius, (decimal)Math.Sin(angle));
                plottedCirclePoints.Add(new Vector3((float)x, (float)y, vector3Pos.Z));
            }

            for (var i = 0; i < plottedCirclePoints.Count; i++)
            {
                if (i >= plottedCirclePoints.Count - 1)
                {
                    var pointEnd1 = camera.WorldToScreen(plottedCirclePoints.Last());
                    var pointEnd2 = camera.WorldToScreen(plottedCirclePoints[0]);
                    Graphics.DrawLine(pointEnd1, pointEnd2, lineWidth, color);
                    return;
                }

                var point1 = camera.WorldToScreen(plottedCirclePoints[i]);
                var point2 = camera.WorldToScreen(plottedCirclePoints[i + 1]);
                Graphics.DrawLine(point1, point2, lineWidth, color);
            }
        }

        public override void EntityAdded(Entity entityWrapper) { _entities.Add(entityWrapper); }

        public override void EntityRemoved(Entity entityWrapper) { _entities.Remove(entityWrapper); }

        private void FindMonsters()
        {
            _entities.Clear();
            //var monster = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Monster];
            //foreach (Entity entity in monster)
            foreach (Entity entity in GameController.Entities)
            {
                if(entity.Type == EntityType.Monster) 
                {
                    if (entity.HasComponent<Actor>())
                    {
                        Actor actor = entity.GetComponent<Actor>();
                        foreach (ActorSkill s in actor.ActorSkills)
                        {
                            if(s.Name == "SlaveCatcherNetThrow" && actor.isAttacking)
                            {
                                //DebugWindow.LogError("Yes");
                                Vector2 destination = actor.CurrentAction.CastDestination;
                                DebugWindow.LogError(destination.X.ToString());
                                //DrawLineTOPoint(new Vector3(destination.X, destination.Y, 0));
                            }
                            //DebugWindow.LogError(s.Name);
                        }
                    }
                }    
                
                //if (GetDistanceFromPlayer(entity) < Settings.Distance.distance.Value && entity.IsAlive)
                //if (GetDistanceFromPlayer(entity) < Settings.Distance.distance.Value && entity.HasComponent<Monster>() && entity.IsAlive)
                if (ValidTarget(entity))
                {
                    EntityAdded(entity);
                }
            }

            
        }
        private bool ValidTarget(Entity entity)
        {
            try
            {
                return entity != null &&
                       entity.IsValid &&
                       entity.IsAlive &&
                       entity.HasComponent<Monster>() &&
                       entity.IsHostile &&
                       entity.HasComponent<Targetable>() &&
                       entity.GetComponent<Targetable>().isTargetable &&
                       entity.HasComponent<Life>() &&
                       entity.GetComponent<Life>().CurHP > 0 &&
                       GetDistanceFromPlayer(entity) < Settings.Aimbot.Distance.Value &&
                       GameController.Window.GetWindowRectangleTimeCache.Contains(
                           GameController.Game.IngameState.Camera.WorldToScreen(entity.Pos));
            }
            catch
            {
                return false;
            }
        }

        private void RemoveMonsters()
        {
            foreach (var entity in _entities)
            {
                if (GetDistanceFromPlayer(entity) > Settings.Aimbot.Distance.Value)
                {
                    EntityRemoved(entity);
                }
            }
        }

        private int GetDistanceFromPlayer(Entity entity)
        {
            var p = entity.Pos;           
            var distance = Math.Sqrt(Math.Pow(player.Pos.X - p.X, 2) + Math.Pow(player.Pos.Y - p.Y, 2));
            return (int)distance;
        }

        private void DrawLineToMonster()
        {
            
            //var camera = GameController.Game.IngameState.Camera;

            Vector2 point1 = camera.WorldToScreen(player.Pos);

            Vector2 point2;
            foreach (var entity in _entities)
            {
                point2 = camera.WorldToScreen(entity.Pos);               
                Graphics.DrawLine(point1, point2, 2, Color.LawnGreen);
            }
        }

        private void DrawLineTOPoint(Vector3 pt)

        {

            //var camera = GameController.Game.IngameState.Camera;

            Vector2 point1 = camera.WorldToScreen(player.Pos);
            Vector2 point2 = camera.WorldToScreen(pt);
            Graphics.DrawLine(point1, point2, 2, Color.LawnGreen);




        }




        private void MonsterAim(Entity monster)
        {
            monster_point = camera.WorldToScreen(monster.Pos);
            Input.SetCursorPos(monster_point);
            Input.KeyPressRelease(Settings.Activeskill.Value);
        }

        private IEnumerator Attack()
        {
            if (Settings.Aimbot.Enable && player.IsAlive)
            {
                if (_entities.Count > 0)
                {

                    monster_point = camera.WorldToScreen(_entities[0].Pos);
                    Input.SetCursorPos(monster_point);
                    yield return Input.KeyPress(Settings.Activeskill.Value);

                }
                else
                {
                    yield break;
                }
            }
            else
            {
                yield break;
            }
            
        }

    }
}
