using Raylib_cs;
using System.Numerics;

namespace PortalGame;

public class Player : GameObject {
    const float airResistance = 0.5f; // TODO: for prototyping only. maybe this belongs in the GameObject class instead

    public Player(Vector2 pos, Vector2 size) : base(pos, size){}

    // TODO: make accel dependent on gravity from physics simulation
    public override void Update() {

        // apply air resistance
        if (vel.X > 0) {
            vel.X = Math.Max(0, vel.X - airResistance);
        }
        else if (vel.X < 0) {
            vel.X = Math.Min(0, vel.X + airResistance);
        }

        accel.Y = 1;

        // call default update method
        base.Update();
    }

    public override void Draw() {
        Raylib.DrawRectangle((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y, Color.Black);
    }
}
