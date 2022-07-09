using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Extensions;
using SkiaGame.Input;
using SkiaSharp;

namespace TestGame;

public class PreyHackGame : Engine
{
    private const float BaseCharForce = 1000f;
    private const float BaseCharSize = 40;
    private const float MinimalBoxSize = 30;
    private const float BoxBaseSize = 40;
    private const int MinNumberOfBoxes = 12;
    private const int MaxNumberOfBoxes = 18;
    private const int MaxTries = 10;

    private GameObject _char = new();

    protected override void OnStart()
    {
        PhysicsEngine.Gravity = Vector2.Zero;
        PhysicsEngine.MinimalVelocity = 3;
        var charSize = BaseCharSize * ScreenInfo.Density;
        _char = new GameObject
        {
            HasGravity = false,
            Primitive = Primitive.Circle,
            Diameter = charSize,
            Color = SKColors.Blue,
            Locked = false,
            Position = new Vector2(100,
                ScreenInfo.Size.Height - charSize),
            RigidBody =
            {
                Restitution = 1.2f,
                Friction = 2f,
                HasGravity = false
            }
        };
        var charRect = SKRect.Create(_char.Position.ToSkPoint(), _char.Size);
        PhysicsEngine.CreateBoundingBox(ScreenInfo.Size);
        AddRects(charRect);
        AddToEngine(_char);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
    }

    protected override void BeforePhysicsUpdate(float timeStep)
    {
        base.BeforePhysicsUpdate(timeStep);
        HandleKeyboardAndTouchKeys(timeStep);
    }

    private void HandleKeyboardAndTouchKeys(float timeStep)
    {
        if (TouchKeys.Up.IsPressed || Keyboard[KeyCode.w].IsPressed ||
            Keyboard[KeyCode.W].IsPressed || Keyboard[KeyCode.Up].IsPressed)
            _char.RigidBody.AddForce(-Vector2.UnitY, BaseCharForce, timeStep);
        if (TouchKeys.Down.IsPressed || Keyboard[KeyCode.s].IsPressed ||
            Keyboard[KeyCode.S].IsPressed || Keyboard[KeyCode.Down].IsPressed)
            _char.RigidBody.AddForce(Vector2.UnitY, BaseCharForce, timeStep);
        if (TouchKeys.Right.IsPressed || Keyboard[KeyCode.d].IsPressed ||
            Keyboard[KeyCode.D].IsPressed || Keyboard[KeyCode.Right].IsPressed)
            _char.RigidBody.AddForce(Vector2.UnitX, BaseCharForce, timeStep);
        if (TouchKeys.Left.IsPressed || Keyboard[KeyCode.a].IsPressed ||
            Keyboard[KeyCode.A].IsPressed || Keyboard[KeyCode.Left].IsPressed)
            _char.RigidBody.AddForce(-Vector2.UnitX, BaseCharForce, timeStep);
    }

    private void AddRects(SKRect charRect)
    {
        var random = new Random(DateTime.Now.Millisecond);
        var nRects = random.Next(MinNumberOfBoxes, MaxNumberOfBoxes);
        var tries = MaxTries;
        var rects = new List<SKRect>
        {
            charRect
        };
        while (nRects > 0 && tries > 0)
        {
            var posX = random.NextSingle() * ScreenInfo.Size.Width;
            var posY = random.NextSingle() * ScreenInfo.Size.Height;
            var width = MinimalBoxSize + random.NextSingle() * BoxBaseSize;
            var height = MinimalBoxSize + random.NextSingle() * BoxBaseSize;
            var rect = SKRect.Create(new SKPoint(posX, posY), new SKSize(width, height));
            if (rects.Any(skRect => skRect.IntersectsWith(rect)) ||
                !PhysicsEngine.IsInsideBounds(rect))
            {
                tries--;
                continue;
            }

            nRects--;
            rects.Add(rect);
        }

        rects.Remove(charRect);

        foreach (var rect in rects)
        {
            var box = new GameObject
            {
                Primitive = Primitive.RoundRect,
                RoundRectCornerRadius = new SKPoint(BoxBaseSize / 5, BoxBaseSize / 5),
                Size = rect.Size,
                Color = SKColors.Red,
                Locked = true,
                Position = rect.Location.ToVector2(),
            };
            AddToEngine(box);
        }
    }
}