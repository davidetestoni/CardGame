using System.Threading.Tasks;

namespace CardGame.Logging
{
    public interface IGameLogger
    {
        Task Log(string message);
    }
}
