using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using SharpDX;

namespace HealthBars
{
    public class HealthBar
    {
        public HealthBar(Entity entity, HealthBarsSettings settings)
        {
            Entity = entity;
            _distance = new TimeCache<float>(() => entity.DistancePlayer, 200);

            Update(entity, settings);
        }
        public RectangleF BackGround { get; set; }
        public RectangleF HpRectangle { get; private set; }
        public RectangleF EsRectangle { get; private set; }
        public Color Color { get; private set; }
        public bool Skip { get; set; }= false;
        public float HpPercent => Life?.HPPercentage ?? 100;
        private readonly TimeCache<float> _distance;
        public float Distance => _distance.Value;
        public Life Life => Entity.GetComponent<Life>();
        public Entity Entity { get; }
        public UnitSettings Settings { get; private set; }
        public CreatureType Type { get; private set; }
        public float HpWidth { get; set; }
        public float EsWidth { get; set; }

        public void CreateColor()
        {
            Color = ChooseColor();
        }

        private Color ChooseColor()
        {
            if (HpPercent <= 0.1f) return Settings.Under10Percent;
            if (Entity.IsHidden) return Color.LightGray;
            return Settings.Color;
        }

        public void CreateHpRectangle()
        {
            HpRectangle = new RectangleF(BackGround.X, BackGround.Y, HpWidth, BackGround.Height);
        }

        public void CreateEsRectangle(float esHeight = 0.33f)
        {
            EsRectangle = new RectangleF(BackGround.X, BackGround.Y, EsWidth, BackGround.Height * esHeight);
        }

        public void Update(Entity entity, HealthBarsSettings settings)
        {
            if (entity.HasComponent<Player>())
            {
                Type = CreatureType.Player;
                Settings = settings.Players;
            }
            else if (entity.HasComponent<Monster>())
            {
                if (entity.IsHostile)
                {
                    switch (entity.GetComponent<ObjectMagicProperties>().Rarity)
                    {
                        case MonsterRarity.White:
                            Type = CreatureType.Normal;
                            Settings = settings.NormalEnemy;
                            break;

                        case MonsterRarity.Magic:
                            Type = CreatureType.Magic;
                            Settings = settings.MagicEnemy;
                            break;

                        case MonsterRarity.Rare:
                            Settings = settings.RareEnemy;
                            Type = CreatureType.Rare;
                            break;

                        case MonsterRarity.Unique:
                            Settings = settings.UniqueEnemy;
                            Type = CreatureType.Unique;
                            break;
                        default:
                            Settings = settings.Minions;
                            Type = CreatureType.Minion;
                            break;
                    }
                }
                else
                {
                    Type = CreatureType.Minion;
                    Settings = settings.Minions;
                }
            }
        }
    }
}
