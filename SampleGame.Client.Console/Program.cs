using CardGame.Client.Networking;

namespace SampleGame.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "127.0.0.1";
            var port = 9050;

            var client = new GameClient();
            client.MessageReceived += (sender, message) =>
            {
                System.Console.WriteLine(message);
            };

            System.Console.Write("Key: ");
            var key = System.Console.ReadLine();

            client.Connect(host, port, key);


        }
    }
}
