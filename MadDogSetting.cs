using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace MadDog
{

    public class MadDogSetting : ISettings
    {
        public ToggleNode Enable { get; set; }

        public MadDogSetting() //test
        {
            Enable = new ToggleNode(true);
            Cameras = new UnitSettings(0x008000ff, 0);

        }

        [Menu("Cameras", 1)]
        public UnitSettings Cameras { get; set; }
    }

    public class UnitSettings : ISettings
    {
        public UnitSettings(uint color, uint outline)
        {
            
            Height = new RangeNode<float>(10, 10, 100);
            Color = color;
            Outline = outline;
            BackGround = SharpDX.Color.Black;

        }

        public ToggleNode Enable { get; set; }
        public RangeNode<float> Height { get; set; }
        public ColorNode Color { get; set; }
        public ColorNode Outline { get; set; }
        public ColorNode BackGround { get; set; }



    }
}
