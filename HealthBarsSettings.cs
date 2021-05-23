using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace HealthBars
{
    public class HealthBarsSettings : ISettings
    {
        public HealthBarsSettings()
        {
            Enable = new ToggleNode(true);
            ShowInTown = new ToggleNode(false);
            Players = new UnitSettings(0x008000ff, 0);
            Minions = new UnitSettings(0x90ee90ff, 0);
            NormalEnemy = new UnitSettings(0xff0000ff, 0, 0x66ff66ff, false, 75, 10);
            MagicEnemy = new UnitSettings(0x8888ffff, 0x8888ffff, 0x66ff99ff, false, 100, 15);
            RareEnemy = new UnitSettings(0xf4ff19ff, 0xf4ff19ff, 0x66ff99ff, false, 125, 20);
            UniqueEnemy = new UnitSettings(0xffa500ff, 0xffa500ff, 0x66ff99ff, true, 200, 25);
        }

        public ToggleNode Enable { get; set; }
        [Menu("Hide Over UI")]
        public ToggleNode HideOverUi { get; set; } = new ToggleNode(true);
        public ToggleNode ShowInTown { get; set; }
        public RangeNode<int> LimitDrawDistance { get; set; } = new RangeNode<int>(133, 0, 1000);
        public RangeNode<int> ShowMinionOnlyBelowHp { get; set; } = new RangeNode<int>(50, 1, 100);
        public ToggleNode SelfHealthBarShow { get; set; } = new ToggleNode(true);

        [Menu("Players", 1)]
        public UnitSettings Players { get; set; }
        [Menu("Minions", 2)]
        public UnitSettings Minions { get; set; }
        [Menu("Normal enemy", 3)]
        public UnitSettings NormalEnemy { get; set; }
        [Menu("Magic enemy", 4)]
        public UnitSettings MagicEnemy { get; set; }
        [Menu("Rare enemy", 5)]
        public UnitSettings RareEnemy { get; set; }
        [Menu("Unique enemy", 6)]
        public UnitSettings UniqueEnemy { get; set; }

        
    }

    public class UnitSettings : ISettings
    {
        public UnitSettings(uint color, uint outline)
        {
            Enable = new ToggleNode(true);
            Width = new RangeNode<float>(100, 20, 250);
            Height = new RangeNode<float>(20, 5, 150);
            Color = color;
            Outline = outline;
            Under10Percent = 0xffffffff;
            PercentTextColor = 0xffffffff;
            HealthTextColor = 0xffffffff;
            HealthTextColorUnder10Percent = 0xffff00ff;
            ShowHealthPercents = new ToggleNode(false);
            ShowEnergyShieldPercents = new ToggleNode(false);
            ShowHealthText = new ToggleNode(false);
            ShowEnergyShieldText = new ToggleNode(false);
            BackGround = SharpDX.Color.Black;
            BarOffsetY = new RangeNode<int>(-100, -300, 100);
        }

        public UnitSettings(uint color, uint outline, uint percentTextColor, bool showHealthText, int width, int height) : this(color, outline)
        {
            PercentTextColor = percentTextColor;
            ShowHealthText.Value = showHealthText;
            Width = new RangeNode<float>(width, 20, 250);
            Height = new RangeNode<float>(height, 5, 150);
        }

        public ToggleNode Enable { get; set; }
        public RangeNode<float> Width { get; set; }
        public RangeNode<float> Height { get; set; }
        public ColorNode Color { get; set; }
        public ColorNode Outline { get; set; }
        public ColorNode BackGround { get; set; }
        public ColorNode Under10Percent { get; set; }
        public ColorNode PercentTextColor { get; set; }
        public ColorNode HealthTextColor { get; set; }
        public ColorNode HealthTextColorUnder10Percent { get; set; }
        public ToggleNode ShowHealthPercents { get; set; }
        public ToggleNode ShowEnergyShieldPercents { get; set; }
        public ToggleNode ShowHealthText { get; set; }
        public ToggleNode ShowEnergyShieldText { get; set; }
        public RangeNode<int> BarOffsetY { get; set; }
    }
}
