using Terraria.ModLoader;

namespace StupidMode
{
    public class StupidMode : Mod
    {
        public StupidMode() { Instance = this; }
        public static StupidMode Instance { get; private set; }
    }
}