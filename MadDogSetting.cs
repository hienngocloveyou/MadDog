using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace MadDog
{

    public class MadDogSetting : ISettings
    {
        [Menu("Enable")]
        public ToggleNode Enable { get; set; }

        public MadDogSetting() //test
        {
            Enable = new ToggleNode(false);
            Cameras = new UnitSettings(0x008000ff, 0);

        }

        [Menu("Cameras", 1)]
        public UnitSettings Cameras { get; set; }
    }

    public class UnitSettings : ISettings
    {

        public UnitSettings(uint color, uint outline)
        {
            Enable = new ToggleNode(true);
            Height = new RangeNode<float>(10, 10, 100);
            
        }

        public ToggleNode Enable { get; set; }
        public RangeNode<float> Height { get; set; }
        



    }
}
