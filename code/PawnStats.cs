using Sandbox;

namespace TheOrangeRun
{
    public partial class PawnStats : BaseNetworkable
    {
        [Net]
        public int Id { get; set; }

        [Net]
        public string Name { get; set; }

        [Net]
        public int TotalOranges { get; set; }
    }
}
