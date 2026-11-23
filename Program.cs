using System.Numerics;
using System.Text.Json;
using Raylib_cs;

namespace PortalGame;

// starter code for Raylib-cs sourced from https://github.com/raylib-cs/raylib-cs
internal static class Program
{
    public static List<GameObject> platforms = new List<GameObject>();
    public static Camera2D camera = new Camera2D();

    // STAThread is required if you deploy using NativeAOT on Windows
    // See https://github.com/raylib-cs/raylib-cs/issues/301
    [STAThread]
    public static void Main()
    {

        const int windowWidth = 1200;
        const int windowHeight = 900;

        // initialize game window
        Raylib.InitWindow(windowWidth, windowHeight, "Portal Game");
        Raylib.SetTargetFPS(60);

        // initialize camera
        camera.Offset = new Vector2(Raylib.GetRenderWidth() / 2, Raylib.GetRenderHeight() / 2);
        camera.Zoom = 1.0f;

        // load level data from JSON file
        string levelFile = "level.json";
        string levelJSON = File.ReadAllText(levelFile);
        Console.WriteLine(levelJSON);
        LevelData levelData = JsonSerializer.Deserialize<LevelData>(levelJSON)!;

        // initialize game objects
        Tether tether = new Tether();
        Player player = new(new Vector2(levelData.spawnX, levelData.spawnY), new Vector2(50, 50), tether, platforms);
        camera.Target = new Vector2(player.pos.X + player.size.X/2, player.pos.Y + player.pos.Y/2);

        // load platforms from level data
        foreach (var i in levelData.platforms) {
            platforms.Add( new Platform(new Vector2(i[0], i[1]), new Vector2(i[2], i[3])) );
        }
        // TODO: also load moving platforms once we get those implemented

        // main game loop
        while (!Raylib.WindowShouldClose())
        {
            // handle inputs

            // if right mouse button was just pressed
            if (Raylib.IsMouseButtonPressed(MouseButton.Right)) {
                // TODO: spawn a portal
            }

            // update game

            tether.Update(player);
            player.Update();

            // update camera target
            camera.Target = Vector2.Lerp(camera.Target, new Vector2(player.pos.X + player.size.X / 2, player.pos.Y + player.size.Y / 2), 0.2f);

            // draw frame
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            Raylib.BeginMode2D(camera);

            player.Draw();
            foreach (Platform i in platforms) {
                i.Draw();
            }
            tether.Draw();

            Raylib.EndMode2D();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
