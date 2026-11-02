using Raylib_cs;
using System.Numerics;

namespace PortalGame;

public class Player : GameObject {
    Tether targetTether;
    const float airResistance = 0.5f; // TODO: for prototyping only. maybe this belongs in the GameObject class instead

    public Player(Vector2 pos, Vector2 size, Tether tether) : base(pos, size){
        targetTether = tether;
    }

    public override void Update() {

        // cancel all other physics calculations if attached to tether
        if (targetTether.isActive) {
            vel = targetTether.endPointVel;
            accel.X = 0;
            accel.Y = 0;
        }
        else {
            // TODO: make accel dependent on gravity from physics simulation

            accel.Y = 0.8f;

            // gradually slow down player
            if (vel.X > 0) {
                vel.X = Math.Max(0, vel.X - airResistance);
            }
            else if (vel.X < 0) {
                vel.X = Math.Min(0, vel.X + airResistance);
            }

        }

        // call default update method
        base.Update();
    }

    public override void Draw() {
        Raylib.DrawRectangle((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y, Color.Black);
    }
}
