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
            
        }
    }

    public class UnitSettings : ISettings
    {
        public UnitSettings(uint color, uint outline)
        {
            Enable = new ToggleNode(true);
            
        }

        public UnitSettings(uint color, uint outline, uint percentTextColor, bool showHealthText, int width, int height) : this(color, outline)
        {
           
        }

        public ToggleNode Enable { get; set; }
        
    }
}
