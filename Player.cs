using Raylib_cs;
using System.Numerics;

namespace PortalGame;

public class Player : GameObject {
    Tether targetTether;
    const float airResistance = 0.5f; // TODO: for prototyping only. maybe this belongs in the GameObject class instead

    public Player(Vector2 pos, Vector2 size, Tether tether, List<GameObject> otherObjs) : base(pos, size, otherObjs){
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

    protected override void ResolveCollisionsX() {
        // check current object against every other object
        foreach (GameObject other in otherObjs) {
            if (IsOverlapping(other)) {
                // left side collision
                if (this.vel.X > 0) {
                    this.vel.X = 0;
                    this.pos.X = other.pos.X - this.size.X;
                }
                // right side collision
                else if (this.vel.X < 0) {
                    this.vel.X = 0;
                    this.pos.X = other.pos.X + other.size.X;
                }
                if (targetTether.isActive) {
                    targetTether.Reset();
                }
            }
        }

    }

    protected override void ResolveCollisionsY() {
        // check current object against every other object
        foreach (GameObject other in otherObjs) {
            if (IsOverlapping(other)) {
                // left side collision
                if (this.vel.Y > 0) {
                    this.vel.Y = 0;
                    this.pos.Y = other.pos.Y - this.size.Y;
                }
                // right side collision
                else if (this.vel.Y < 0) {
                    this.vel.Y = 0;
                    this.pos.Y = other.pos.Y + other.size.Y;
                }
                if (targetTether.isActive) {
                    targetTether.Reset();
                }
            }
        }

    }

    public override void Draw() {
        Raylib.DrawRectangle((int) pos.X, (int) pos.Y, (int) size.X, (int) size.Y, Color.Black);
    }
}
