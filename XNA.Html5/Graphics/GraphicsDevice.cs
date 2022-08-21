using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using static H5.Core.dom;

namespace Microsoft.Xna.Framework.Graphics
{
    public delegate void OnResizeHandler();

    public class Html5
    {
        public static HTMLCanvasElement Canvas;
        internal static CanvasRenderingContext2D Context;
        internal static MouseState MouseState;
        internal static List<TouchLocation> Touches = new List<TouchLocation>();
        public static OnResizeHandler OnResize;
    }

    public class GraphicsDevice
    {
        public Viewport Viewport { get; private set; }

        public GraphicsDevice()
        {
            Html5.Canvas = new HTMLCanvasElement();
            Html5.Canvas.width = (uint)window.innerWidth;
            Html5.Canvas.height = (uint)window.innerHeight;
            document.body.appendChild(Html5.Canvas);
            Viewport = new Viewport(0, 0, (int)Html5.Canvas.width, (int)Html5.Canvas.height);
            Html5.Context = Html5.Canvas.getContext("2d").As<CanvasRenderingContext2D>();
            document.body.setAttribute("style", "margin:0px;overflow:hidden;");
            Html5.Canvas.onmousedown = (e) =>
            {
                Html5.MouseState = new MouseState();
                Html5.MouseState.LeftButton = ButtonState.Pressed;
                Html5.MouseState.Position = new Point((int)e.clientX, (int)e.clientY);
                Html5.Touches.Clear();
                TouchLocation loc = new TouchLocation(0, TouchLocationState.Pressed, new Vector2((int)e.clientX, (int)e.clientY));
                Html5.Touches.Add(loc);
            };
            Html5.Canvas.onmousemove = (e) =>
            {
                if (TouchPanel.didPress)
                {
                    Html5.Touches.Clear();
                    TouchLocation loc = new TouchLocation(0, TouchLocationState.Moved, new Vector2((int)e.clientX, (int)e.clientY));
                    Html5.Touches.Add(loc);
                }
            };
            Html5.Canvas.onmouseup = (e) =>
            {
                Html5.MouseState = new MouseState();
                Html5.MouseState.LeftButton = ButtonState.Released;
                Html5.MouseState.Position = new Point((int)e.clientX, (int)e.clientY);
                List<TouchLocation> locs = new List<TouchLocation>();
                TouchLocation loc = new TouchLocation(0, TouchLocationState.Released, new Vector2((int)e.clientX, (int)e.clientY));
                locs.Add(loc);
                Html5.Touches.Clear();
                Html5.Touches = locs;
            };
            document.body.ontouchstart = (e) =>
            {
                e.preventDefault();
            };
            Html5.Canvas.ontouchstart = (e) =>
            {
                e.preventDefault();
                Html5.Touches.Clear();
                foreach (var touch in e.touches)
                {
                    TouchLocation loc = new TouchLocation(0, TouchLocationState.Pressed, new Vector2((int)touch.clientX, (int)touch.clientY));
                    Html5.Touches.Add(loc);
                }
            };
            Html5.Canvas.ontouchmove = (e) =>
            {
                e.preventDefault();
                if (TouchPanel.didPress)
                {
                    Html5.Touches.Clear();
                    foreach (var touch in e.touches)
                    {
                        TouchLocation loc = new TouchLocation(0, TouchLocationState.Moved, new Vector2((int)touch.clientX, (int)touch.clientY));
                        Html5.Touches.Add(loc);
                    }
                }
            };
            Html5.Canvas.ontouchend = (e) =>
            {
                e.preventDefault();
                List<TouchLocation> locs = new List<TouchLocation>();
                foreach (var touch in Html5.Touches)
                {
                    TouchLocation loc = new TouchLocation(0, TouchLocationState.Released, new Vector2(touch.Position.X, touch.Position.Y));
                    locs.Add(loc);
                }
                Html5.Touches.Clear();
                Html5.Touches = locs;
            };
            Html5.Canvas.ontouchcancel = (e) =>
            {
                e.preventDefault();
                List<TouchLocation> locs = new List<TouchLocation>();
                foreach (var touch in Html5.Touches)
                {
                    TouchLocation loc = new TouchLocation(0, TouchLocationState.Released, new Vector2(touch.Position.X, touch.Position.Y));
                    locs.Add(loc);
                }
                Html5.Touches.Clear();
                Html5.Touches = locs;
            };
            window.onresize = (e) =>
            {
                Html5.Canvas.width = (uint)window.innerWidth;
                Html5.Canvas.height = (uint)window.innerHeight;
                Viewport = new Viewport(0, 0, (int)Html5.Canvas.width, (int)Html5.Canvas.height);
                try
                {
                    Html5.OnResize();
                }
                catch { }
            };            
        }

        public void Clear(Color color)
        {
            Html5.Context.fillStyle = string.Format("rgba({0},{1},{2},{3})",
                            Convert.ToInt32(color.R),
                            Convert.ToInt32(color.G),
                            Convert.ToInt32(color.B),
                            Convert.ToInt32(color.A)
                            );
            Html5.Context.fillRect(0, 0, Html5.Canvas.width, Html5.Canvas.height);
        }

        internal void Draw(DrawSpec spec)
        {
            if (spec.transform != null)
            {
                var transform = spec.transform.Value;
                Html5.Context.setTransform(transform.M11, transform.M12, transform.M21, transform.M22, transform.M41, transform.M42);
            }
            foreach (var sprite in spec.spriteSpecs)
            {
                Html5.Context.save();
                Html5.Context.translate(sprite.position.X, sprite.position.Y);
                Html5.Context.rotate(sprite.rotation);
                if (sprite.text == null)
                {
                    if (sprite.color.PackedValue != Color.White.PackedValue) //Save some CPU/GPU resource
                    {
                        Html5.Context.globalAlpha = (float)sprite.color.PackedValue / (float)Color.White.PackedValue;
                    }
                }
                float dx = -sprite.origin.X * (sprite.useVScale ? sprite.vScale.X : sprite.scale);
                float dy = -sprite.origin.Y * (sprite.useVScale ? sprite.vScale.Y : sprite.scale);
                if (sprite.rectangle == null)
                {
                    if (sprite.text == null) //texture
                    {
                        float dw = sprite.texture.Width * (sprite.useVScale ? sprite.vScale.X : sprite.scale);
                        float dh = sprite.texture.Height * (sprite.useVScale ? sprite.vScale.Y : sprite.scale);
                        if (sprite.effects == SpriteEffects.FlipHorizontally)
                        {
                            Html5.Context.scale(-1, 1);
                            Html5.Context.translate(-dw, 0f);
                        }
                        else if (sprite.effects == SpriteEffects.FlipVertically)
                        {
                            Html5.Context.scale(1, -1);
                            Html5.Context.translate(0f, -dh);
                        }
                        Html5.Context.drawImage(sprite.texture.Image,
                            dx, dy, dw, dh
                            );
                    }
                    else //font
                    {
                        Html5.Context.textAlign = "start";
                        var color = sprite.color;
                        Html5.Context.fillStyle = string.Format("rgba({0},{1},{2},{3})",
                            Convert.ToInt32(color.R),
                            Convert.ToInt32(color.G),
                            Convert.ToInt32(color.B),
                            Convert.ToInt32(color.A)
                            );
                        float size = SpriteFont.Size * sprite.scale;
                        Html5.Context.font = Convert.ToInt32(size) + "px " + SpriteFont.Font;
                        Html5.Context.fillText(sprite.text, Convert.ToInt32(dx), 0);
                    }
                }
                else
                {
                    var rec = sprite.rectangle.Value;
                    float sx = rec.X;
                    float sy = rec.Y;
                    float sw = rec.Width;
                    float sh = rec.Height;
                    float dw = rec.Width * (sprite.useVScale ? sprite.vScale.X : sprite.scale);
                    float dh = rec.Height * (sprite.useVScale ? sprite.vScale.Y : sprite.scale);
                    if (sprite.effects == SpriteEffects.FlipHorizontally)
                    {
                        Html5.Context.scale(-1, 1);
                        Html5.Context.translate(-dw, 0f);
                    }
                    else if (sprite.effects == SpriteEffects.FlipVertically)
                    {
                        Html5.Context.scale(1, -1);
                        Html5.Context.translate(0f, -dh);
                    }
                    Html5.Context.drawImage(sprite.texture.Image,
                        sx, sy, sw, sh, dx, dy, dw, dh
                        );
                }
                Html5.Context.restore();
            }
            if (spec.transform != null)
            {
                Html5.Context.setTransform(1, 0, 0, 1, 0, 0);
            }
        }
    }
}
