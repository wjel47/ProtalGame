using Raylib_cs;
using System.Numerics;

namespace PortalGame;

public class Player : GameObject {

    public Player(Vector2 pos, Vector2 size) : base(pos, size){}

    public override void Update() {
        // TODO: make accel dependent on gravity
        accel.Y = 1;
        // call default update method
        base.Update();
    }

    public override void Draw() {
        Raylib.DrawRectangle((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y, Color.Black);
    }
}
