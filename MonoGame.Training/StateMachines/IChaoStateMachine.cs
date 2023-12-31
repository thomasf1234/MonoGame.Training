using MonoGame.Training.Constants;
using MonoGame.Training.Entities;
using System.Threading.Tasks;

namespace MonoGame.Training.StateMachines
{
    public interface IChaoStateMachine
    {
        void Assign(ChaoEntity chaoEntity);
        ChaoState GetState();
        Task TriggerWalkAsync(Direction direction);
        Task TriggerStopAsync();
        string ExportToDotGraph();
    }
}
