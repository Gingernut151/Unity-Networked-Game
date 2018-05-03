using SharedLibrary;

namespace Multiplayer_Server
{
    struct playerLobbyData
    {
        public bool isReady;
        public string username;
        public string address;
        public int ping;
    }

    struct playerGameData
    {
        public string username;
        public string address;
        public Vec3 position;
        public Vec4 rotation;
    }
}
