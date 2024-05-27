
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using MonoGame.Training.Components;
using Microsoft.Xna.Framework.Media;
using System.Runtime.Serialization;

namespace MonoGame.Training.Systems
{
    public class MusicSystem : System
    {
        public MusicSystem(IComponentRepository componentRepository) : base(componentRepository)
        {
 
        }

        public void Update(GameTime gameTime)
        {
            // TODO : Only allow one
            foreach (var entityId in EntityIds)
            {
                var soundComponent = _componentRepository.GetComponent<SoundComponent>(entityId);

                if (MediaPlayer.State == MediaState.Playing && !soundComponent.IsPaused)
                {
                    continue;
                }

                if (MediaPlayer.State == MediaState.Paused && soundComponent.IsPaused)
                {
                    continue;
                }

                if (MediaPlayer.State == MediaState.Paused && !soundComponent.IsPaused)
                {
                    MediaPlayer.Resume();
                    continue;
                }

                if (MediaPlayer.State == MediaState.Playing && soundComponent.IsPaused)
                {
                    MediaPlayer.Pause();
                    continue;
                }

                if (MediaPlayer.State == MediaState.Stopped && !soundComponent.IsPaused)
                {
                    MediaPlayer.Play(soundComponent.Song);
                    MediaPlayer.IsRepeating = soundComponent.IsLooping;
                }
            }
        }
    }
}