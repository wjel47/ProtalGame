using System.Numerics;
using Raylib_cs;

namespace PortalGame;

public class Game {
    public List<GameObject> platforms = new List<GameObject>();
    public Camera2D camera = new Camera2D(); 
    Tether tether;
    Player player;

    public Game(LevelData levelData) {

        // initialize tether
        tether = new Tether(this);

        // initialize player
        player = new(new Vector2(levelData.spawnX, levelData.spawnY), new Vector2(50, 50), platforms);

        // link tether to player
        tether.targetPlayer = player;
        player.targetTether = tether;

        // initialize camera
        camera.Offset = new Vector2(Raylib.GetRenderWidth() / 2, Raylib.GetRenderHeight() / 2);
        camera.Zoom = 1.0f;
        camera.Target = new Vector2(player.pos.X + player.size.X/2, player.pos.Y + player.pos.Y/2);

        // load platforms from level data
        foreach (var i in levelData.platforms) {
            platforms.Add( new Platform(new Vector2(i[0], i[1]), new Vector2(i[2], i[3])) );
        }

        // TODO: also load moving platforms once we get those implemented

    }

    public void Update(float dt) {
        // if right mouse button was just pressed
        if (Raylib.IsMouseButtonPressed(MouseButton.Right)) {
            // TODO: spawn a portal
        }

        tether.Update(dt);
        player.Update(dt);

        // update camera target
        camera.Target = Vector2.Lerp(camera.Target, new Vector2(player.pos.X + player.size.X / 2, player.pos.Y + player.size.Y / 2), 0.2f);
    }

    public void Draw() {
        Raylib.ClearBackground(new Color(50, 50, 60));
        Raylib.BeginMode2D(camera);

        player.Draw();
        foreach (Platform i in platforms) {
            i.Draw();
        }
        tether.Draw();

        Raylib.EndMode2D();
    }
}
