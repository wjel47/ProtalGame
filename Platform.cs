using Raylib_cs;
using System.Numerics;

namespace PortalGame;

public class Platform : GameObject {

    public Platform(Vector2 pos, Vector2 size) : base(pos, size){}

    public override void Update() {
        // no updates needed for stationary platform
    }

    public override void Draw() {
        Raylib.DrawRectangle((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y, Color.Black);
    }
}
