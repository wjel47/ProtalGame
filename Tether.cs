using System.Numerics;
using Raylib_cs;

namespace PortalGame;

public class Tether {
    const int maxPoints = 20;
    const int spacing = 10;
    int curPoint = 0;

    Vector2[] points = new Vector2[maxPoints];

    public void Update() {
        if (!Raylib.IsMouseButtonDown(MouseButton.Left)) {
            // update all the points (todo), then set curPoint to 0 to reset the tether
            curPoint = 0;
        }
    }

    public void TryAddingPoint() {
        Vector2 mousePos = Raylib.GetMousePosition();
        // add first point
        if (curPoint == 0) {
            AddPoint(mousePos);
        }
        else if (curPoint > 0 && curPoint < maxPoints) {
            // only add a new point once there is enough distance from the previous one
            float distanceFromPrev = Vector2.Distance(mousePos, points[curPoint-1]);
            if (distanceFromPrev > spacing) {
                AddPoint(mousePos);
            }
        }
    }

    // TODO change this so that distance between every point is the same
    private void AddPoint(Vector2 pos) {
        points[curPoint] = pos;
        // ensure current point does not go out of bounds
        if (curPoint < maxPoints - 1) {
            curPoint++;
        }
    }

    public void Draw() {
        for (int i = 0; i < curPoint; i++) {
            Raylib.DrawCircle((int) points[i].X, (int) points[i].Y, 5, Color.Black);
        }
    }

}
