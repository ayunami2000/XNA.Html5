using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static H5.Core.dom;

namespace Microsoft.Xna.Framework.Graphics
{
    public class Texture2D : GraphicsResource
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private HTMLImageElement image;
        public HTMLImageElement Image
        {
            get { return image; }
            set
            {
                image = value;
                Width = (int)image.naturalWidth;
                Height = (int)image.naturalHeight;
            }
        }

        public Texture2D()
        {
            
        }

        public Texture2D(GraphicsDevice graphicsDevice, int width, int height)
        {

        }

        public void GetData<T>(T[] data)
        {

        }
        public void SetData(Color[] color)
        {

        }
    }
}
