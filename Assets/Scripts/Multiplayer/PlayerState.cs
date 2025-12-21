
namespace GDS
{
    public class PlayerState
    {
        public string PlayerID { get; }
        public ulong ClientID { get; set; }
        public string DisplayName { get; set; }
        public bool IsConnected { get; set; }

        public PlayerState(string playerID)
        {
            PlayerID = playerID;
        }
    }
}