using System.Text.Json;
using Raylib_cs;

namespace PortalGame;

// starter code for Raylib-cs sourced from https://github.com/raylib-cs/raylib-cs
internal static class Program
{

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

        // load level data from JSON file
        string levelFile = "level.json";
        string levelJSON = File.ReadAllText(levelFile);
        LevelData levelData = JsonSerializer.Deserialize<LevelData>(levelJSON)!;
        Game game = new Game(levelData);


        // main game loop
        while (!Raylib.WindowShouldClose())
        {

            // update game
            float dt = Raylib.GetFrameTime();
            // ensure delta time doesn't get too high
            dt = Math.Min(dt, 0.05f);

            game.Update(dt);

            // draw frame
            Raylib.BeginDrawing();
            game.Draw();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
