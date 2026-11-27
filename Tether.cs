using System.Numerics;
using Raylib_cs;

namespace PortalGame;

public class Tether {
    Game game; // the game that this tether is a part of
    List<GameObject> anchorObjs;
    public Player targetPlayer;

    const int maxPoints = 20;
    const int spacing = 20;
    float secsToUnwind = 0.8f;
    const int startDrawRange = 50;
    const double overshootFactor = 1.2;

    int curPoint = 0;
    float secsCounter = 0;
    public bool isActive = false;

    public Vector2[] points = new Vector2[maxPoints];
    double[] rotateAmount = new double[maxPoints];

    Vector2 prevEndPointPos;
    public Vector2 endPointVel;

    public Tether(Game game) {
        this.game = game;
        anchorObjs = game.platforms;
    }

    public void Update(float dt) {

        // if there is no active tether, reset on mouse click
        // also allows player to cancel their swing by clicking
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) {
            Reset();
        }

        // if left mouse is down
        if (Raylib.IsMouseButtonDown(MouseButton.Left)) {
            if (!isActive) {
                // move existing points along with player as the tether is being drawn
                for (int i = 0; i < curPoint; i++) {
                    points[i] = Vector2.Add(points[i], targetPlayer.vel * dt);
                }
                TryAddingPoint();
            }
        }

        // if there is a valid tether, begin unwinding once player releases mouse button
        if (Raylib.IsMouseButtonReleased(MouseButton.Left)) {
            if (IsDrawnTetherValid()) {
                CalculateRotateAmounts();
                secsCounter = 0;
                isActive = true;
            }
            else {
                Reset();
            }
        }

        if (isActive) {
            // update all the points for the designated animation length
            if (secsCounter < secsToUnwind) {
                Unwind(dt);
            }
            else {
                Reset();
            }
        }

    }

    private void Unwind(float dt) {
        // track position of end point before updates are applied
        prevEndPointPos = points[0];

        // rotate current point and all further points
        for (int i = curPoint - 3; i >= 0; i--) {
            for (int j = i; j >= 0; j--) {
                double curRotateAmount = rotateAmount[i] * dt / secsToUnwind * overshootFactor;
                points[j] = RotatePointAroundCenter(points[j], points[i+1], curRotateAmount);
            }
        }

        // get velocity of end point between previous frame and current frame
        endPointVel = Vector2.Subtract(points[0], prevEndPointPos) / dt;

        secsCounter += dt;
    }

    public void Reset() {
        isActive = false;
        curPoint = 0;
    }

    public void Draw() {
        for (int i = 0; i < curPoint; i++) {
            Color color = Raylib.ColorFromHSV(220 + i * 5, 0.6f, 1.0f);
            Raylib.DrawCircle((int) points[i].X, (int) points[i].Y, 4, color);
            if (i < curPoint - 1) {
                Raylib.DrawLineEx(points[i], points[i+1], 8, color);
            }
        }
    }

    public void TryAddingPoint() {

        if (!isActive) {
            Vector2 mousePos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), game.camera);
            Vector2 playerCenter = new Vector2(targetPlayer.pos.X + targetPlayer.size.X / 2, targetPlayer.pos.Y + targetPlayer.size.Y / 2);

            // add first point if mouse is within range of player's center
            if (curPoint == 0 && Vector2.Distance(mousePos, playerCenter) <= startDrawRange) {
                AddPoint(playerCenter);
            }
            else if (curPoint > 0 && curPoint < maxPoints) {

                // only add a new point once there is enough distance from the previous one
                Vector2 prevPoint = points[curPoint-1];
                float distanceFromPrev = Vector2.Distance(mousePos, prevPoint);

                // keep adding points until newest point is near mouse cursor
                while (distanceFromPrev > spacing && curPoint < maxPoints) {

                    // normalize distance so all points are evenly spaced
                    Vector2 diff = new Vector2(mousePos.X - prevPoint.X, mousePos.Y - prevPoint.Y);
                    diff = Vector2.Normalize(diff);
                    Vector2 pos = new Vector2(prevPoint.X + diff.X * spacing, prevPoint.Y + diff.Y * spacing);

                    AddPoint(pos);
                    prevPoint = points[curPoint-1];
                    distanceFromPrev = Vector2.Distance(mousePos, prevPoint);
                }
            }
        }

    }

    private void AddPoint(Vector2 pos) {
        points[curPoint] = pos;
        if (curPoint < maxPoints) {
            curPoint++;
        }
    }

    private bool IsDrawnTetherValid() {

        // do not create a new tether if one is already active
        if (isActive) {
            return false;
        }

        // require at least 3 points
        if (curPoint <= 2) {
            return false;
        }

        // check if the last drawn point to be anchored to a platform
        Vector2 lastPoint = points[curPoint-1];
        bool isAnchored = false;
        // allow points to be slightly off a platform for lenience
        const int padding = 10;
        foreach (GameObject platform in anchorObjs) {
            if (lastPoint.X >= platform.pos.X - padding
                    && lastPoint.X <= platform.pos.X + platform.size.X + padding 
                    && lastPoint.Y >= platform.pos.Y - padding
                    && lastPoint.Y <= platform.pos.Y + platform.size.Y + padding) {
                isAnchored = true;
            }
        }

        if (!isAnchored) {
            return false;
        }

        return true;

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

        // simulate the whole unwinding sequence to get the top speed of the end point
        // we will use this speed to determine how long the real unwinding sequence should last for
        simulatedPoints = (Vector2[]) points.Clone();
        float maxEndPointSpeed = 0;
        const int numSteps = 10;
        for (int step = 0; step < numSteps; step++) {
            Vector2 simulatedPrevEndPointPos = simulatedPoints[0];
            for (int i = curPoint - 3; i >= 0; i--) {
                for (int j = i; j >= 0; j--) {
                    double curRotateAmount = rotateAmount[i] / numSteps * overshootFactor;
                    simulatedPoints[j] = RotatePointAroundCenter(simulatedPoints[j], simulatedPoints[i+1], curRotateAmount);
                }
            }
            maxEndPointSpeed = Math.Max(maxEndPointSpeed, Vector2.Distance(simulatedPoints[0], simulatedPrevEndPointPos) * numSteps);
        }

        // make the duration of unwinding dependent on the tether's max speed
        secsToUnwind = maxEndPointSpeed / 1200 + 0.2f;

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
