using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using static H5.Core.dom;

namespace Microsoft.Xna.Framework
{
    public class Game
    {
        private GameComponentCollection _components;
        public GraphicsDevice GraphicsDevice { get; set; }
        public bool IsActive { get; set; }
        public ContentManager Content { get; set; }
        public bool IsFixedTimeStep { get; set; }
        private GameTime gameTime;
        private DateTime timeNow, t1, t2, u1, u2;
        private int leadingTime;
        private int timeoutId = 0;
        private TimeSpan _targetElapsedTime;
        public GameComponentCollection Components
        {
            get { return _components; }
        }

        public TimeSpan TargetElapsedTime
        {
            get { return _targetElapsedTime; }
            set { _targetElapsedTime = value; }
        }

        public Game()
        {
            _targetElapsedTime = TimeSpan.FromMilliseconds(1000f / 60f);
            Content = new ContentManager();
            _components = new GameComponentCollection();
        }

        protected virtual void Initialize()
        {
            foreach (GameComponent com in _components)
            {
                com.Initialize();
            }
        }

        protected virtual void LoadContent()
        {
        }

        protected virtual void Update(GameTime gameTime)
        {
            foreach (GameComponent com in _components)
            {
                com.Update(gameTime);
            }
        }

        protected virtual void Draw(GameTime gameTime)
        {
            foreach (DrawableGameComponent com in _components)
            {
                com.Draw(gameTime);
            }            
        }

        public void Run()
        {
            IsActive = true;
            gameTime = new GameTime();
            gameTime.TotalGameTime = new TimeSpan(0);
            gameTime.ElapsedGameTime = new TimeSpan(0);
            timeNow = DateTime.Now;
            Initialize();
            GameLoop();
        }

        private void GameLoop()
        {
            window.clearTimeout(timeoutId);
            if (IsActive)
            {
                gameTime.ElapsedGameTime = DateTime.Now - timeNow;
                gameTime.TotalGameTime += gameTime.ElapsedGameTime;
                timeNow = DateTime.Now;
                t1 = DateTime.Now;
                if (Content.AllResoucesLoaded)
                {
                    u1 = DateTime.Now;
                    Update(gameTime);
                    u2 = DateTime.Now;
                    Draw(gameTime);
                }
                t2 = DateTime.Now;
                leadingTime = Convert.ToInt32(_targetElapsedTime.TotalMilliseconds - (t2 - t1).TotalMilliseconds);
                //System.Console.WriteLine("elapsedTime: " + gameTime.ElapsedGameTime.TotalMilliseconds + " totalTime: " + gameTime.TotalGameTime.TotalSeconds + " leadingTime: " + leadingTime + " updateTime: " + (u2 - u1).TotalMilliseconds + " totalProcessingTime: " + (t2 - t1).TotalMilliseconds);
                if (leadingTime <= 0)
                {
                    leadingTime = 1;
                }
            }
            else
            {
                timeNow = DateTime.Now;
                leadingTime = 100;
            }
            timeoutId = (int) window.setTimeout(new WindowTimers.setTimeoutFn(args =>
            {
                GameLoop();
            }), leadingTime);

        }

        public void Exit()
        {

        }
    }
}
