using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Windows.Forms;

namespace MadDog
{

    public class MadDogSetting : ISettings
    {
        //[Menu("Enable")]
        public ToggleNode Enable { get; set; }

        public MadDogSetting() //test
        {
            Enable = new ToggleNode(true);
            Aimbot = new UnitSettings(0x008000ff, 0);

        }

        public HotkeyNode Activeskill { get; set; } = Keys.Q;
        public HotkeyNode EnableAim { get; set; } = Keys.Space;

        [Menu("Aimbot", 1)]
        public UnitSettings Aimbot { get; set; }
    }

    public class UnitSettings : ISettings
    {
        
        public UnitSettings(uint color, uint outline)
        {
            Enable = new ToggleNode(true);
            Distance = new RangeNode<int>(100, 100, 1000);
            AimLoopDelay = new RangeNode<int>(124, 1, 200);


        }

        [Menu("Enable")]
        public ToggleNode Enable { get; set; }
        public RangeNode<int> Distance { get; set; }
        public RangeNode<int> AimLoopDelay { get; set; } 




    }
}
