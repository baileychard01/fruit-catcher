using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace fruitCollecting;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _backgroundImage;
    private Texture2D _fruitBasketImage;
    private SoundEffect _coinSound;
    private Vector2 _playerLocation;
    private readonly int _speed;
    private Texture2D _fruitTexture;
    private SpriteFont _text;
    private int _playerScore;
    private float _defaultScoreScale = 1f;
    private float _scoreScale = 1f;

    private readonly List<Fruit> _fruitList = new();
    private Rectangle[] _fruitRectangles;

    private readonly Random _random = new();


    private const int WindowWidth = 928;
    public const int WindowHeight = 793;

    public const int MaxScore = 10;
    public const int MinScore = 1;

    private float _currentTimer;
    private float _spawnRate = 1f;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = WindowWidth; // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = WindowHeight; // set this value to the desired height of your window
        _graphics.ApplyChanges();

        _playerLocation.X = 100;
        _playerLocation.Y = 643;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _speed = 5;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _backgroundImage = Content.Load<Texture2D>("Background");
        _fruitBasketImage = Content.Load<Texture2D>("gamePlayer");
        _text = Content.Load<SpriteFont>("File");
        _coinSound = Content.Load<SoundEffect>("coinSound");

        _fruitTexture = Content.Load<Texture2D>("gameFruit");
        var horizontalCount = _fruitTexture.Width / 16;
        var verticleCount = _fruitTexture.Height / 16;

        _fruitRectangles = new Rectangle[horizontalCount * verticleCount];
        var currentRect = 0;

        for (int i = 0; i < horizontalCount; i++)
        {
            for (int j = 0; j < verticleCount; j++)
            {
                _fruitRectangles[currentRect] = new Rectangle(i * 16, j * 16, 16, 16);
                currentRect++;
            }
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            _playerLocation.X += _speed;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            _playerLocation.X -= _speed;
        }

        _currentTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_currentTimer >= 1)
        {
            _currentTimer = 0;
            SpawnFruit();
        }

        // Make sure the player can't walk out the screen
        _playerLocation = Vector2.Clamp(_playerLocation, new Vector2(0, 0),
            new Vector2(WindowWidth - _fruitBasketImage.Width, WindowHeight - _fruitBasketImage.Height));

        base.Update(gameTime);
    }

    public void SpawnFruit()
    {
        var selectedRect = _random.Next(0, _fruitRectangles.Length);

        var fruit = new Fruit(
            _fruitTexture,
            _fruitRectangles[selectedRect],
            new Vector2(_random.Next(0, WindowWidth), -100),
            _random.Next(1, 5),
            _random.Next(MinScore, MaxScore)
        );
        _fruitList.Add(fruit);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        _spriteBatch.Draw(_backgroundImage, new Vector2(0, 0), Color.White);
        var removalList = new List<Fruit>();
        foreach (var fruit in _fruitList)
        {
            fruit.Update();
            fruit.Draw(_spriteBatch);

            Vector2 fruitpos = fruit.Position;

            if (fruitpos.Y > WindowHeight)
            {
                removalList.Add(fruit);
            }
            

            var fruitRectangle = fruit.GetRectangle();

            var playerRectangle = GetPlayerRectangle();

            if (fruitRectangle.Intersects(playerRectangle))
            {
                removalList.Add(fruit);
                _playerScore += fruit.Score;
                _scoreScale = 3f;
                _coinSound.Play();
            }
        }

        _scoreScale = MathHelper.Lerp(_scoreScale, _defaultScoreScale, (float)gameTime.ElapsedGameTime.TotalSeconds * 5f);
        foreach (Fruit fruit in removalList)
        {
            _fruitList.Remove(fruit);
        }

        _spriteBatch.Draw(_fruitBasketImage, new Vector2(_playerLocation.X, _playerLocation.Y), Color.White);
        var scoreString = _playerScore + "";
        var stringWidth = _text.MeasureString(scoreString);
        _spriteBatch.DrawString(_text, scoreString, new Vector2(WindowWidth / 2f, 10 + stringWidth.Y),
            Color.White, 0.0f, stringWidth / 2f, new Vector2(_scoreScale, _scoreScale), SpriteEffects.None, 0f);
        _spriteBatch.End();


        base.Draw(gameTime);
    }

    public Rectangle GetPlayerRectangle()
    {
        var basketX = _playerLocation.X + 27;
        var basketY = _playerLocation.Y + 59;
        var basketWidth = _fruitBasketImage.Width - 27;
        var basketHeight = _fruitBasketImage.Height - 59;

        return new Rectangle((int)basketX, (int)basketY, basketWidth, basketHeight);
    }
}