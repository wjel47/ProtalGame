using System.Numerics;
using Raylib_cs;

namespace PortalGame;

internal static class Program
{
    public static List<GameObject> platforms = new List<GameObject>();

    // STAThread is required if you deploy using NativeAOT on Windows
    // See https://github.com/raylib-cs/raylib-cs/issues/301
    [STAThread]
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "Portal Game");
        Raylib.SetTargetFPS(60);

        // initialize game objects
        Player player = new(new Vector2(50, 50), new Vector2(20, 20));
        Platform platform = new(new Vector2(0, 200), new Vector2(200, 20));
        Tether tether = new Tether();
        platforms.Add(platform);

        // main game loop
        while (!Raylib.WindowShouldClose())
        {
            // handle inputs

            // get x and y position of cursor
            Vector2 mousePos = Raylib.GetMousePosition();

            // if left mouse is down
            if (Raylib.IsMouseButtonDown(MouseButton.Left)) {
                // TODO: draw the tether
                tether.TryAddingPoint();
            }

            // if right mouse button was just pressed
            if (Raylib.IsMouseButtonPressed(MouseButton.Right)) {
                // TODO: spawn a portal
            }

            // update game
            
            player.Update();
            tether.Update();

            // draw frame
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.White);
            player.Draw();
            platform.Draw();
            tether.Draw();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
