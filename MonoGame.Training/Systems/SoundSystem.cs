
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using MonoGame.Training.Components;
using Microsoft.Xna.Framework.Media;
using System;
using MonoGame.Training.Models;

namespace MonoGame.Training.Systems
{
    public class SoundSystem : System
    {
        private readonly Configuration _configuration;
        private readonly IResourceRepository _resourceRepository;
        public SoundSystem(Configuration configuration, IComponentRepository componentRepository, IResourceRepository resourceRepository) : base(componentRepository)
        {
            _configuration = configuration;
            _resourceRepository = resourceRepository;
        }   

        public void Update(GameTime gameTime)
        {
            // TODO : Only allow one
            foreach (var entityId in EntityIds)
            {
                var soundComponent = _componentRepository.GetComponent<SoundComponent>(entityId);

                if (soundComponent.Background)
                {

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
                        Song song = _resourceRepository.GetSong(soundComponent.SoundId);
                        MediaPlayer.Play(song);
                        MediaPlayer.IsRepeating = soundComponent.IsLooping;
                        MediaPlayer.Volume = _configuration.Volume;
                    }
                }
                else
                {
                    // TODO : Implement SoundEffect
                    throw new NotImplementedException("Foreground sounds not yet implemented");
                }
            }
        }

        protected override void OnRegister(int entityId)
        {
            // Only allow one background sound
        }

        protected override void OnDeregister(int entityId)
        {
            // Stop sound if playing
        }
    }
}