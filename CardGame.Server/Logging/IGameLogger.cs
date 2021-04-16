using System.Threading.Tasks;

namespace CardGame.Server.Logging
{
    public interface IGameLogger
    {
        Task Log(string message);
    }
}
