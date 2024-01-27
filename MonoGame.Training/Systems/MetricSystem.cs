
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using System;
using MonoGame.Training.Components;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Training.Systems
{
    /*
    Frame Rate
    Draw Call
    Frame Time (ms)
    Triangle Count
    Game Logic (ms)
     */
    public class MetricSystem : System
    {
        private Stopwatch _stopwatch;
        private List<double> _frameRates;
        public MetricSystem(IComponentRepository componentRepository) : base(componentRepository)
        {
            _frameRates = new List<double>();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Update(GameTime gameTime)
        {
            var frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameRates.Add(frameRate);
            if (_stopwatch.ElapsedMilliseconds < 1000)
            {
                return;
            }
            _stopwatch.Restart();
            var averageFrameRate = _frameRates.Average();
            _frameRates = new List<double>();

            foreach (var entityId in EntityIds)
            {
                var textComponent = _componentRepository.GetComponent<TextComponent>(entityId);

                textComponent.Text = $"{Math.Round(averageFrameRate, 1)} AVG FPS";
            }
        }
    }
}