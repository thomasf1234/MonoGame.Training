using MonoGame.Training.Models;

namespace MonoGame.Training.Repositories
{
    public interface IConfigurationRepository
    {
        public Configuration Load();

        public void Save(Configuration configuration);
    }
}
