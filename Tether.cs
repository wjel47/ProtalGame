using System.Numerics;
using Raylib_cs;

namespace PortalGame;

public class Tether {
    const int maxPoints = 12;
    const int spacing = 20;
    const int framesToUnwind = 30;
    const int startDrawRange = 50;
    const double overshootFactor = 1.2;
    int curPoint = 0;
    int frameCounter = 0;

    public Vector2[] points = new Vector2[maxPoints];
    double[] rotateAmount = new double[maxPoints];

    Vector2 prevEndPointPos;
    Vector2 endPointVel = new Vector2(0, 0);

    public void Update(Player player) {
        if (Raylib.IsMouseButtonReleased(MouseButton.Left)) {
            if (curPoint > 2) {
                CalculateRotateAmounts();
                frameCounter = 0;
            }
        }
        if (!Raylib.IsMouseButtonDown(MouseButton.Left)) {

            // update all the points
            if (curPoint > 2 && frameCounter < framesToUnwind) {

                // track position of end point before updates are applied
                prevEndPointPos = points[0];

                // rotate current point and all further points
                for (int i = curPoint - 3; i >= 0; i--) {
                    for (int j = i; j >= 0; j--) {
                        points[j] = RotatePointAroundCenter(points[j], points[i+1], rotateAmount[i]);
                    }
                }

                // get velocity of end point between previous frame and current frame
                endPointVel = Vector2.Subtract(points[0], prevEndPointPos);

                // drag player along with the end of the tether
                player.vel = endPointVel;

                frameCounter++;
            }
        }
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            curPoint = 0;
        }
    }

    public void TryAddingPoint(Player player) {
        Vector2 mousePos = Raylib.GetMousePosition();
        Vector2 playerCenter = new Vector2(player.pos.X + player.size.X / 2, player.pos.Y + player.size.Y / 2);
        // add first point if mouse is within range of player's center
        if (curPoint == 0 && Vector2.Distance(mousePos, playerCenter) <= startDrawRange) {
            AddPoint(playerCenter);
        }
        else if (curPoint > 0 && curPoint < maxPoints) {
            // only add a new point once there is enough distance from the previous one
            Vector2 prevPoint = points[curPoint-1];
            float distanceFromPrev = Vector2.Distance(mousePos, prevPoint);
            if (distanceFromPrev > spacing) {

                // normalize distance so all points are evenly spaced
                Vector2 diff = new Vector2(mousePos.X - prevPoint.X, mousePos.Y - prevPoint.Y);
                diff = Vector2.Normalize(diff);
                Vector2 pos = new Vector2(prevPoint.X + diff.X * spacing, prevPoint.Y + diff.Y * spacing);

                AddPoint(pos);
            }
        }
    }

    private void AddPoint(Vector2 pos) {
        points[curPoint] = pos;
        if (curPoint < maxPoints) {
            curPoint++;
        }
    }

    // TODO: we might have to rework this so that the target angle is based on more than just the last two points
    private void CalculateRotateAmounts() {

        // use the slope of the last two drawn points as the target angle
        double targetAngle = GetAngleBetweenPoints(points[curPoint-1], points[curPoint-2]);

        // make a copy of each point to simulate the unwinding sequence
        Vector2[] simulatedPoints = (Vector2[]) points.Clone();

        // always keep last two points still, start checking from third last point
        for (int i = curPoint - 3; i >= 0; i--) {
            double curAngle = GetAngleBetweenPoints(simulatedPoints[i+1], simulatedPoints[i]);

            rotateAmount[i] = GetShorterAngleDifference(curAngle, targetAngle);

            // apply rotation to current point and all further points
            for (int j = i; j >= 0; j--) {
                simulatedPoints[j] = RotatePointAroundCenter(simulatedPoints[j], simulatedPoints[i+1], rotateAmount[i]);
            }

        }

        // convert total rotate amount to amount needed each frame
        for (int i = curPoint - 3; i >= 0; i--) {
            rotateAmount[i] /= framesToUnwind;
            // apply a slight overshoot to each point
            rotateAmount[i] *= overshootFactor;
        }

    }

    private double GetShorterAngleDifference(double curAngle, double targetAngle) {
        double diff = targetAngle - curAngle;
        diff = (diff + Math.PI) % (2 * Math.PI);
        if (diff < 0) {
            diff += 2 * Math.PI;
        }
        diff -= Math.PI;
        return diff;
    }

    public void Draw() {
        for (int i = 0; i < curPoint; i++) {
            Raylib.DrawCircle((int) points[i].X, (int) points[i].Y, 5, Color.Blue);
        }
    }

    private double GetAngleBetweenPoints(Vector2 a, Vector2 b) {
        double deltaX = b.X - a.X; 
        double deltaY = b.Y - a.Y;
        return Math.Atan2(deltaY, deltaX);
    }

    private Vector2 RotatePointAroundCenter(Vector2 point, Vector2 center, double radians) {
        // offset points so center is at origin
        Vector2 diff = new Vector2(point.X - center.X, point.Y - center.Y);

        // apply rotation formula (source: https://www.mathwords.com/r/rotation_formula.htm)
        Vector2 newPos;
        newPos.X = (float) (diff.X * Math.Cos(radians) - diff.Y * Math.Sin(radians));
        newPos.Y = (float) (diff.X * Math.Sin(radians) + diff.Y * Math.Cos(radians));

        // add back the original offset
        newPos.X += center.X;
        newPos.Y += center.Y;

        return newPos;
    }

}
