using System.Numerics;
using System.Text.Json;
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

        const int windowWidth = 1200;
        const int windowHeight = 900;
        Raylib.InitWindow(windowWidth, windowHeight, "Portal Game");
        Raylib.SetTargetFPS(60);

        // initialize game objects
        string levelFile = "level.json";
        string levelJSON = File.ReadAllText(levelFile);
        Console.WriteLine(levelJSON);
        LevelData levelData = JsonSerializer.Deserialize<LevelData>(levelJSON)!;

        Tether tether = new Tether();
        Player player = new(new Vector2(levelData.spawnX, levelData.spawnY), new Vector2(50, 50), tether, platforms);

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
