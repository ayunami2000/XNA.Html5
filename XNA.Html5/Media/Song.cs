using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using H5;

namespace Microsoft.Xna.Framework.Media
{
    public class Song : SoundEffect
    {
        public void SetLoop(bool loop)
        {
            int index = Index;
            Script.Write("audioContexts[index].loop = loop;");
        }

        internal void Suspend()
        {
            int index = Index;
            Script.Write("audioContexts[index].suspend();");
        }

        internal void Resume()
        {
            int index = Index;
            Script.Write("audioContexts[index].resume();");
        }
    }
}
