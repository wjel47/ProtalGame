using System.Numerics;

namespace PortalGame;

public abstract class GameObject {
    public Vector2 pos;
    protected Vector2 size;
    public Vector2 vel;
    protected Vector2 accel;
    float mass;

    public GameObject(Vector2 pos, Vector2 size) {
        this.pos = pos;
        this.size = size;
    }

    public virtual void Update() {
        // apply acceleration
        vel.X += accel.X;
        vel.Y += accel.Y;

        // horizontal movement and collision detection
        pos.X += vel.X;
        ResolveCollisionsX(Program.platforms);

        // vertical movement and collision detection
        pos.Y += vel.Y;
        ResolveCollisionsY(Program.platforms);
    }

    public abstract void Draw();

    private bool IsOverlapping(GameObject other) {
        return this.pos.X + this.size.X > other.pos.X
            && this.pos.X < other.pos.X + other.size.X
            &&  this.pos.Y + this.size.Y > other.pos.Y
            && this.pos.Y < other.pos.Y + other.size.Y;
    }

    private void ResolveCollisionsX(List<GameObject> otherObjs) {
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
            }
        }

    }

    private void ResolveCollisionsY(List<GameObject> otherObjs) {
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
            }
        }

    }

}
