using Raylib_cs;
using System.Numerics;

namespace PortalGame;

public class Player : GameObject {
    public Tether targetTether;
    const float maxDistFromTether = 50;
    const float airResistance = 1800; // TODO: for prototyping only. maybe this belongs in the GameObject class instead

    public Player(Vector2 pos, Vector2 size, List<GameObject> otherObjs) : base(pos, size, otherObjs){
    }

    public override void Update(float dt) {

        // cancel all other physics calculations if attached to tether
        if (targetTether.isActive) {
            vel = targetTether.endPointVel;
            accel.X = 0;
            accel.Y = 0;

        }
        else {
            // TODO: make accel dependent on gravity from physics simulation

            accel.Y = 2880;

            // gradually slow down player
            if (vel.X > 0) {
                vel.X = Math.Max(0, vel.X - airResistance * dt);
            }
            else if (vel.X < 0) {
                vel.X = Math.Min(0, vel.X + airResistance * dt);
            }

        }

        // call default update method
        base.Update(dt);

        // deactivate tether if player gets too far from the endpoint
        if (targetTether.isActive) {
            if (Vector2.Distance(targetTether.points[0], new Vector2(pos.X + size.X / 2, pos.Y + size.Y / 2)) > maxDistFromTether) {
                targetTether.Reset();
            }
        }
    }

    public override void Draw() {
        Raylib.DrawRectangle((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y, new Color(255, 200, 255));
    }
}
