using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Guilred.Input;
using Guilred.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestArea;

public class PointSnap : Game {
    private readonly GraphicsDeviceManager _graphics;
    private GuilBatch _guilBatch = null!;
    private readonly InputManager _input;
    private readonly Dictionary<string, Texture2D> _textures = [];
    private Vector2 _screenSize => new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
    public PointSnap() {
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
    private readonly List<Vector2> _points = [];
    private bool _firstInit = true;
    protected override void Update(GameTime gameTime) {
        if (_input.KeyTapped(Keys.Escape))
            Exit();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _input.Update(dt);
        if (_input.KeyTappedAndHeld(Keys.Tab) || _firstInit && !(_firstInit = false)) {
            _points.Clear();
            var screenCenter = _screenSize / 2;
            for (int i = 0; i < 10; i++) {
                var rng = Random.Shared;
                _points.Add(screenCenter + new Vector2(rng.Next(-200, 200), rng.Next(-200, 200)));
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        var time = gameTime.TotalGameTime.TotalSeconds;
        var angle = (float)(time % double.Tau);
        var wave = (float)double.Pow(double.Sin(time), 0);

        _guilBatch.Begin();

        var snapTol = 25;
        foreach (var point in _points) {
            _guilBatch.FillCircle(point, Color.Red, 5);
            _guilBatch.BorderCircle(point, Color.Red, snapTol, 3);
        }
        _guilBatch.FillCircle(Snap(_input.VCMousePos, _points, snapTol), Color.Green, 5);

        _guilBatch.End();

        base.Draw(gameTime);
    }
    public static Vector2 Snap(Vector2 value, IReadOnlyList<Vector2> snapPoints, float tolerance) {
        if (snapPoints == null || snapPoints.Count == 0)
            return value;

        if (tolerance < 0f)
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be non-negative.");

        Vector2 nearest = snapPoints[0];
        float nearestDistanceSq = Vector2.DistanceSquared(value, snapPoints[0]);

        for (int i = 1; i < snapPoints.Count; i++) {
            float distanceSq = Vector2.DistanceSquared(value, snapPoints[i]);

            if (distanceSq < nearestDistanceSq) {
                nearestDistanceSq = distanceSq;
                nearest = snapPoints[i];
            }
        }

        return nearestDistanceSq <= tolerance * tolerance ? nearest : value;
    }
}
