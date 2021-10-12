using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Windows.Forms;

namespace MadDog
{

    public class MadDogSetting : ISettings
    {
        [Menu("Enable")]
        public ToggleNode Enable { get; set; }

        public MadDogSetting() //test
        {
            Enable = new ToggleNode(false);
            Distance = new UnitSettings(0x008000ff, 0);

        }

        public HotkeyNode ContagionKey { get; set; } = Keys.Q;
        [Menu("Distance", 1)]
        public UnitSettings Distance { get; set; }
    }

    public class UnitSettings : ISettings
    {
        
        public UnitSettings(uint color, uint outline)
        {
            Enable = new ToggleNode(true);
            distance = new RangeNode<int>(10, 10, 100);
            
        }

        //[Menu("Enable")]
        public ToggleNode Enable { get; set; }
        public RangeNode<int> distance { get; set; }
        



    }
}
