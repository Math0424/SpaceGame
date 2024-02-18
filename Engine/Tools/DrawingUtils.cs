using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Engine.Systems;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    internal static class DrawingUtils
    {

        public static void DrawLine(Vector3 start, Vector3 dir, Color color)
        {
            Render.EnqueueMessage(new RenderMessageDrawLine(start, start + dir, color));
        }

        public static void DrawMatrix(Matrix matrix)
        {
            DrawLine(matrix.Translation, Vector3.Normalize(matrix.Up), Color.Green);
            DrawLine(matrix.Translation, Vector3.Normalize(matrix.Right), Color.Red);
            DrawLine(matrix.Translation, Vector3.Normalize(matrix.Forward), Color.Blue);
        }

        public static void DrawWorldText(Camera camera, string text, Vector3 worldPos, Color color, TextDrawOptions options = TextDrawOptions.Left)
        {
            if (Vector3.Dot(camera.Forward, camera.Translation - worldPos) > .5f)
                return;
            
            Vector3 screen = camera.WorldToScreen(worldPos);
            Render.EnqueueMessage(new RenderMessageDrawText("Fonts/Debug", text, 1, 1 / screen.Z, new Vector2(screen.X, screen.Y), color, options));
        }

    }
}
