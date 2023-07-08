using Sandbox;

namespace TheOrangeRun
{
    public partial class PawnStats : BaseNetworkable
    {
        public PawnStats()
        {
        }

        [Net]
        public int Id { get; set; }

        [Net]
        public string Name { get; set; }

        [Net]
        public int Oranges { get; set; }
    }
}
