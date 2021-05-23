using System;
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

namespace HealthBars
{
    public class HealthBars : BaseSettingsPlugin<HealthBarsSettings>
    {
        private static string IgnoreFile => Path.Combine("config", "ignored_entities.txt");
        private List<string> IgnoredEntities { get; set; }
        private Camera Camera => GameController.Game.IngameState.Camera;
        private IngameUIElements IngameUi { get; set; }
        private CachedValue<bool> IngameUiCheckVisible { get; set; }
        private Vector2 OldPlayerHealthbarPosition { get; set; }
        private Entity Player { get; set; }
        private RectangleF WindowRectangle { get; set; }
        private Size2F WindowSize { get; set; }

        public override void OnLoad()
        {
            Graphics.InitImage("healthbar.png");
        }

        public override bool Initialise()
        {
            Player = GameController.Player;
            IngameUi = GameController.IngameState.IngameUi;

            GameController.EntityListWrapper.PlayerUpdate += (sender, args) =>
            {
                Player = GameController.Player;
            };

            IngameUiCheckVisible = new TimeCache<bool>(() =>
            {
                WindowRectangle = GameController.Window.GetWindowRectangleReal();
                WindowSize = new Size2F(WindowRectangle.Width / 1920, WindowRectangle.Height / 1080);

                return IngameUi.BetrayalWindow.IsVisibleLocal 
                       || IngameUi.SellWindow.IsVisibleLocal 
                       || IngameUi.DelveWindow.IsVisibleLocal 
                       || IngameUi.IncursionWindow.IsVisibleLocal 
                       || IngameUi.UnveilWindow.IsVisibleLocal 
                       || IngameUi.TreePanel.IsVisibleLocal 
                       || IngameUi.Atlas.IsVisibleLocal 
                       || IngameUi.CraftBench.IsVisibleLocal;
            }, 250);
            ReadIgnoreFile();

            return true;
        }

        private void ReadIgnoreFile()
        {
            var path = Path.Combine(DirectoryFullName, IgnoreFile);
            if (File.Exists(path))
            {
                IgnoredEntities = File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#")).ToList();
            } 
            else
            {
                LogError($"HealthBars.ReadIgnoreFile -> Ignored entities file does not exist. Path: {path}");
            }
        }

        public override void AreaChange(AreaInstance area)
        {
            IngameUi = GameController.IngameState.IngameUi;
            ReadIgnoreFile();
        }

        public override Job Tick()
        {
            return !ShouldRun() ? null : new Job(nameof(HealthBars), AddHealthBarComponents);
        }

        public override void Render()
        {
            if (!ShouldRun()) return;

            // own player
            if (Settings.SelfHealthBarShow != null && Settings.SelfHealthBarShow)
            {
                var playerBar = Player.GetHudComponent<HealthBar>();

                // if this check is not done the player health bar is jiggling around
                var location = playerBar.BackGround.Location;
                if (Math.Abs(OldPlayerHealthbarPosition.X - location.X) < 50
                    || Math.Abs(OldPlayerHealthbarPosition.Y - location.Y) < 50)
                {
                    playerBar.BackGround = new RectangleF(
                        OldPlayerHealthbarPosition.X,
                        OldPlayerHealthbarPosition.Y,
                        playerBar.BackGround.Width,
                        playerBar.BackGround.Height);
                }
                OldPlayerHealthbarPosition = playerBar.BackGround.Location;

                DrawBar(playerBar);
            }

            // other entities
            foreach (var idEntityPair in GameController.EntityListWrapper.EntityCache)
            {
                var entity = idEntityPair.Value;
                if (!IsEntityRelevant(entity)) continue;
                if (entity.Address == Player.Address) continue;

                var healthBar = entity.GetHudComponent<HealthBar>();
                if (healthBar?.Skip != false) continue;

                DrawBar(healthBar);
            }
        }

        private bool ShouldRun()
        {
            if (IngameUiCheckVisible.Value && Settings.HideOverUi?.Value == true) return false;
            if (Camera == null) return false;
            if (GameController.Area.CurrentArea.IsTown && !Settings.ShowInTown) return false;

            return true;
        }

        private void AddHealthBarComponents()
        {
            try
            {
                foreach (var idEntityPair in GameController.EntityListWrapper.EntityCache)
                {
                    var entity = idEntityPair.Value;
                    if (!IsEntityRelevant(entity)) continue;

                    AddHealthBarComponent(entity);
                }
                AddHealthBarComponent(Player);
            }
            catch (Exception e)
            {
                DebugWindow.LogError($"HealthBars.AddHealthBarComponents -> {e.Message}");
            }
        }

        private void AddHealthBarComponent(Entity entity)
        {
            if (entity == null) return;
            var healthBar = entity.GetHudComponent<HealthBar>();
            if (healthBar == null)
            {
                healthBar = new HealthBar(entity, Settings);
                entity.SetHudComponent(healthBar);
            }
            HpBarWork(healthBar);
        }

        private bool IsEntityRelevant(Entity entity)
        {
            if (entity == null || !entity.IsValid || entity.IsDead) return false;
            if (entity.Type != EntityType.Monster && entity.Type != EntityType.Player) return false;
            return true;
        }

        private bool SkipHealthBar(HealthBar healthBar)
        {
            if (healthBar?.Settings == null) return true;
            if (!healthBar.Settings.Enable) return true;
            if (!healthBar.Entity.IsAlive) return true;
            if (healthBar.HpPercent < 0.001f) return true;
            if (healthBar.Distance > Settings.LimitDrawDistance) return true;
            if (healthBar.Type == CreatureType.Minion
                && healthBar.HpPercent * 100 > Settings.ShowMinionOnlyBelowHp) return true;
            if (IgnoredEntities.Any(ignoreString => ignoreString.StartsWith(healthBar.Entity.Metadata))) return true;

            return false;
        }

        private void HpBarWork(HealthBar healthBar)
        {
            if (healthBar == null) return;
            healthBar.Skip = SkipHealthBar(healthBar);
            if (healthBar.Skip) return;

            var worldCoords = healthBar.Entity.Pos;
            var mobScreenCoords = Camera.WorldToScreen(worldCoords);
            if (mobScreenCoords == Vector2.Zero) return;
            var scaledWidth = healthBar.Settings.Width * WindowSize.Width;
            var scaledHeight = healthBar.Settings.Height * WindowSize.Height;

            healthBar.BackGround = new RectangleF(
                mobScreenCoords.X - scaledWidth / 2f,
                mobScreenCoords.Y - scaledHeight / 2f + healthBar.Settings.BarOffsetY.Value,
                scaledWidth,
                scaledHeight);

            if (healthBar.Distance > 80 && !WindowRectangle.Intersects(healthBar.BackGround))
            {
                healthBar.Skip = true;
                return;
            }

            healthBar.HpWidth = healthBar.HpPercent * scaledWidth;
            healthBar.EsWidth = healthBar.Life.ESPercentage * scaledWidth;
        }


        private void DrawBar(HealthBar bar)
        {
            if (bar == null) return;
            Graphics.DrawBox(bar.BackGround, bar.Settings.BackGround);
            Graphics.DrawBox(new RectangleF(bar.BackGround.X, bar.BackGround.Y, bar.HpWidth, bar.BackGround.Height), bar.Color);
            Graphics.DrawBox(new RectangleF(bar.BackGround.X, bar.BackGround.Y, bar.EsWidth, bar.BackGround.Height * 0.33f), Color.Aqua);
            Graphics.DrawFrame(bar.BackGround, bar.Settings.Outline, 1);

            ShowPercents(bar);
            ShowNumbersInHealthbar(bar);
        }

        private void ShowNumbersInHealthbar(HealthBar bar)
        {
            if (!bar.Settings.ShowHealthText && !bar.Settings.ShowEnergyShieldText) return;

            string healthBarText = "";
            if (bar.Settings.ShowHealthText)
            {
                healthBarText = $"{bar.Life.CurHP:N0}/{bar.Life.MaxHP:N0}";
            } 
            else if (bar.Settings.ShowEnergyShieldText)
            {
                healthBarText = $"{bar.Life.CurES:N0}/{bar.Life.MaxES:N0}";
            }

            Graphics.DrawText(healthBarText,
                new Vector2(bar.BackGround.Center.X, bar.BackGround.Center.Y - Graphics.Font.Size / 2f),
                bar.Settings.HealthTextColor, 
                FontAlign.Center);
        }

        private void ShowPercents(HealthBar bar)
        {
            if (!bar.Settings.ShowHealthPercents && !bar.Settings.ShowEnergyShieldPercents) return;

            float percents = 0;
            if (bar.Settings.ShowHealthPercents)
            {
                percents = bar.Life.HPPercentage;
            }
            else if (bar.Settings.ShowEnergyShieldPercents)
            {
                percents = bar.Life.ESPercentage;
            }

            Graphics.DrawText(FloatToPercentString(percents),
                new Vector2(bar.BackGround.Right, bar.BackGround.Center.Y - Graphics.Font.Size / 2f),
                bar.Settings.PercentTextColor);
        }

        private string FloatToPercentString (float number)
        {
            return $"{Math.Floor(number * 100).ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
