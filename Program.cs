using System.Numerics;
using Raylib_cs;

namespace PortalGame;

// starter code for Raylib-cs sourced from https://github.com/raylib-cs/raylib-cs
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
        Tether tether = new Tether();
        Player player = new(new Vector2(50, 50), new Vector2(50, 50), tether, platforms);

        // temporary platforms for testing
        platforms.Add( new Platform(new Vector2(0, 400), new Vector2(800, 20)) );
        platforms.Add( new Platform(new Vector2(300, 200), new Vector2(200, 50)) );

        // main game loop
        while (!Raylib.WindowShouldClose())
        {
            // handle inputs

            // if right mouse button was just pressed
            if (Raylib.IsMouseButtonPressed(MouseButton.Right)) {
                // TODO: spawn a portal
            }

            // update game
            
            // TODO: right now, the tether controls some of the player's movement. consider moving all player movement code into the player class.
            tether.Update(player);
            player.Update();

            // draw frame
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.White);
            player.Draw();
            foreach (Platform i in platforms) {
                i.Draw();
            }
            tether.Draw();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
