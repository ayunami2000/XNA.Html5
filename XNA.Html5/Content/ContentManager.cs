using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using static H5.Core.dom;

namespace Microsoft.Xna.Framework.Content
{
    public class Loadable
    {
        public virtual string Name { get; set; }
    }

    public class ContentManager
    {
        public string RootDirectory { get; set; }
        public GraphicsDevice _graphicsDevice { get; set; }
        protected Dictionary<string, bool> ResourcesReady;
        public Action OnAllResourceLoaded;
        public Action<int> OnResourceLoaded;

        public ContentManager()
        {
            ResourcesReady = new Dictionary<string, bool>();
            WebAudioHelper.Init();
            MediaPlayer.contentManager = this;
        }

        public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
        {
        }

        private void NotifyIfResourcesLoaded()
        {
            int percent = ResourcesReady.Where(t => t.Value).Count() * 100 / ResourcesReady.Count();
            try
            {
                OnResourceLoaded(percent);
            }
            catch { }
            if (percent == 100)
            {
                try
                {
                    OnAllResourceLoaded();
                }
                catch { }
            }
        }

        public T Load<T>(string name) where T : Loadable, new()
        {
            ResourcesReady.Add(name, false);
            if (typeof(T) == typeof(Texture2D))
            {
                var t = new Texture2D();
                var img = new HTMLImageElement();
                img.onload = (e) =>
                {
                    t.Image = img;
                    t.Name = name;
                    ResourcesReady[name] = true;
                    NotifyIfResourcesLoaded();
                };
                img.src = RootDirectory + "/" + name + ".png";            
                return t as T;
            }
            else if (typeof(T) == typeof(SoundEffect))
            {
                var t = new SoundEffect();
                t.Load(RootDirectory + "/" + name + ".mp3", () => {
                    t.Name = name;
                    ResourcesReady[name] = t.Loaded = true;
                    NotifyIfResourcesLoaded();
                });
                return t as T;
            }
            else if (typeof(T) == typeof(Song))
            {
                var t = new Song();
                t.Load(RootDirectory + "/" + name + ".mp3", () => {
                    t.Name = name;
                    ResourcesReady[name] = t.Loaded = true;
                    NotifyIfResourcesLoaded();
                });
                return t as T;
            }
            else if (typeof(T) == typeof(SpriteFont))
            {
                var t = new SpriteFont();
                t.Name = name;
                ResourcesReady[name] = true;
                return t as T;
            }
            else
            {
                return new T();
            }
        }

        public bool AllResoucesLoaded
        {
            get
            {
                return ResourcesReady.Where(t => !t.Value).Count() == 0;
            }
        }

        public void Unload()
        {
            ResourcesReady.Clear();
        }
    }
}
