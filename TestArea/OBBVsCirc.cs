using System.Collections.Generic;
using System.IO;
using System.Linq;
using Guilred.Input;
using Guilred.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestArea;

public class OBBVsCirc : Game {
    private readonly GraphicsDeviceManager _graphics;
    private GuilBatch _guilBatch = null!;
    private readonly InputManager _input;
    private readonly Dictionary<string, Texture2D> _textures = [];
    private Vector2 _screenSize => new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
    public OBBVsCirc() {
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

    private bool _firstInit = true;
    protected override void Update(GameTime gameTime) {
        if (_input.KeyTapped(Keys.F1))
            Exit();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _input.Update(dt);
        if (_input.KeyTappedAndHeld(Keys.Tab) || _firstInit && !(_firstInit = false)) {

        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        var time = gameTime.TotalGameTime.TotalSeconds;
        var angle = (float)(time % double.Tau);
        var wave = (float)double.Pow(double.Sin(time), 0);

        _guilBatch.Begin();

        _guilBatch.End();

        base.Draw(gameTime);
    }
}
