using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using H5;

namespace Microsoft.Xna.Framework.Audio
{

    public class SoundEffect : Loadable
    {
        internal int Index;
        internal bool Loaded = false;

        public SoundEffect()
        {
            Index = Script.Write<int>("addAudioContext()");
        }
        

        public void Play()
        {
            int index = Index;
            Script.Write("audioContexts[index].play();");
        }

        internal void Load(string src, Action d)
        {
            int index = Index;
            Script.Write("audioContexts[index].load(src, d);");
        }
    }
}
