using System.Numerics;

namespace PortalGame;

public abstract class GameObject {
    protected Vector2 pos;
    protected Vector2 size;
    protected Vector2 vel;
    protected Vector2 accel;

    public GameObject(Vector2 pos, Vector2 size) {
        this.pos = pos;
        this.size = size;
    }

    public virtual void Update() {
        // apply acceleration
        vel.X += accel.X;
        vel.Y += accel.Y;

        // move object
        pos.X += vel.X;
        pos.Y += vel.Y;
    }

    public abstract void Draw();
}
