using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Guilred.Input;
using Guilred.Rendering;
using Guilred.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestArea;

public class VertsOBB : Game {
    private readonly GraphicsDeviceManager _graphics;
    private GuilBatch _guilBatch = null!;
    private readonly InputManager _input;
    private readonly Dictionary<string, Texture2D> _textures = [];

    private Vector2 _screenSize => new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
    public VertsOBB() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        (_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight) = (768, 432);
        (_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight) = (1280, 720);
        _graphics.ApplyChanges();
        _input = new InputManager(Window);
    }

    protected override void LoadContent() {
        _guilBatch = new GuilBatch(GraphicsDevice);
        var pngs = Directory.EnumerateDirectories("Content/Textures").Where(png => png.EndsWith(".png"));
        foreach (var png in pngs) {
            _textures[new FileInfo(png).Name] = Texture2D.FromFile(GraphicsDevice, png);
        }
    }

    private readonly List<Vector2> _verts = [];
    private bool _firstInit = true;
    protected override void Update(GameTime gameTime) {
        if (_input.KeyTapped(Keys.Escape))
            Exit();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _input.Update(dt);
        if (_input.KeyTappedAndHeld(Keys.Tab) || _firstInit && !(_firstInit = false)) {
            _verts.Clear();
            var screenCenter = _screenSize / 2;
            for (int i = 0; i < 20; i++) {
                var rng = Random.Shared;
                _verts.Add(screenCenter + new Vector2(rng.Next(-200, 200), rng.Next(-100, 100)));
            }
            //_verts.Add(screenCenter + new Vector2(-1, -1) * 200);
            //_verts.Add(screenCenter + new Vector2(1, -1) * 200);
            //_verts.Add(screenCenter + new Vector2(1, 1) * 200);
            //_verts.Add(screenCenter + new Vector2(-1, 1) * 200);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(new Color(20, 20, 20));
        var time = gameTime.TotalGameTime.TotalSeconds;

        _guilBatch.Begin();

        foreach (var vert in _verts) {
            _guilBatch.FillCircle(vert, Color.Red, 5);
        }
        
        var angle = (float)(time % double.Tau);
        var pivot = _screenSize / 2;
        var obb = GetOrientedBoundingBox(_verts, angle, pivot);
        obb.Inflate(5);
        _guilBatch.BorderRectangle(
            obb.Position, obb.Size,
            Color.Green,
            borderThickness: 2,
            rotation: angle,
            origin: pivot - obb.Position
        );

        _guilBatch.End();

        base.Draw(gameTime);
    }

    public static RectangleF GetOrientedBoundingBox(IReadOnlyList<Vector2> verts, float angleRadians, Vector2 pivot) {
        if (verts.Count == 0)
            return RectangleF.Empty;

        (float sin, float cos) = MathF.SinCos(-angleRadians);

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var p in verts) {
            var local = p - pivot;
            float rx = cos * local.X - sin * local.Y + pivot.X;
            float ry = sin * local.X + cos * local.Y + pivot.Y;

            minX = float.Min(minX, rx);
            maxX = float.Max(maxX, rx);
            minY = float.Min(minY, ry);
            maxY = float.Max(maxY, ry);
        }

        return new RectangleF(minX, minY, maxX - minX, maxY - minY);
    }
}
