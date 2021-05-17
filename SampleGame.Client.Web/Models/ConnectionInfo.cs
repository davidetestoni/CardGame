namespace SampleGame.Client.Web.Models
{
    public class ConnectionInfo
    {
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 9050;
        public string Key { get; set; }
        public string Name { get; set; } = "Player";
    }
}
