using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace fruitCollecting;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _backgroundImage;
    private Texture2D _fruitBasketImage;
    private Texture2D _bombImage;
    private SoundEffect _coinSound;
    private SoundEffect _splatSound;
    private Song _song;
    private Vector2 _playerLocation;
    private readonly int _speed;
    private Texture2D _fruitTexture;
    private SpriteFont _text;
    private int _playerScore;
    private float _defaultScoreScale = 1f;
    private float _scoreScale = 1f;

    private readonly List<FallingItem> _fruitList = new();
    private readonly List<Explosion> _explosionList = new();
    
    private Rectangle[] _fruitRectangles;
    private Rectangle[] _explosionRectangles;

    private readonly Random _random = new();

    private float xVelocity;

    private bool _paused;

    private const int WindowWidth = 928;
    public const int WindowHeight = 793;

    public const int MaxScore = 10;
    public const int MinScore = 1;

    private float _currentTimer;
    private float _spawnRate = 1f;
    private Texture2D _explosionParticleTexture;
    private float _titleRotation;
    private float _titleScale = 1f;

    private Color currentColor = Color.White;
    private Color targetColor = Color.White;
    
    private KeyboardState _lastKeyState;
    
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
        _speed = 32;

    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _backgroundImage = Content.Load<Texture2D>("Background");
        _fruitBasketImage = Content.Load<Texture2D>("gamePlayer");
        _text = Content.Load<SpriteFont>("File");
        _coinSound = Content.Load<SoundEffect>("coinSound");
        _splatSound = Content.Load<SoundEffect>("splatNoise");
        _song = Content.Load<Song>("song");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.5f;
        MediaPlayer.Play(_song);
        
        _explosionParticleTexture = Content.Load<Texture2D>("explosionparticle");

        var explosionWidth = _explosionParticleTexture.Width / 10;
        var explosionHeight = _explosionParticleTexture.Height / 7;
        var explosionHorizontalCount = _explosionParticleTexture.Width / explosionWidth;
        var explosionVerticalCount = _explosionParticleTexture.Height / explosionHeight;
        var currentExplosionRect = 0;

        _explosionRectangles = new Rectangle[explosionHorizontalCount * explosionVerticalCount];
        for (int i = 0; i < explosionVerticalCount; i++)
        {
            for (int j = 0; j < explosionHorizontalCount; j++)
            {
                _explosionRectangles[currentExplosionRect] = new Rectangle(j * explosionWidth, i * explosionHeight, explosionWidth, explosionHeight);
                currentExplosionRect++;
            }
        }
        
        _bombImage = Content.Load<Texture2D>("bombImage");
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
        if (Keyboard.GetState().IsKeyDown(Keys.Escape) && _lastKeyState.IsKeyUp(Keys.Escape))
            _paused = !_paused;

        
        _lastKeyState = Keyboard.GetState();
        
        if (_paused)
        {
            return;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            xVelocity += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            xVelocity -= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        xVelocity *= 0.95f;
        _playerLocation.X += xVelocity;

        xVelocity = MathHelper.Clamp(xVelocity, -10, 10);

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

        var isBomb = _random.Next(0, 10);

        if (isBomb > 8)
        {
            var bomb = new FallingItem(
                _bombImage,
                new Rectangle(0, 0, _bombImage.Width, _bombImage.Height),
                new Vector2(_random.Next(0, WindowWidth), -100),
                _random.Next(1, 5),
                -20,
                0.1f
            );
            _fruitList.Add(bomb);
        }
        else
        {
            var fruit = new FallingItem(
                _fruitTexture,
                _fruitRectangles[selectedRect],
                new Vector2(_random.Next(0, WindowWidth), -100),
                _random.Next(1, 5),
                _random.Next(MinScore, MaxScore),
                _random.Next(0, 10) / 30f
            );
            _fruitList.Add(fruit);
        }
    }

    private float h = 10;
    private float s = 0;
    private float v = 0;
    private float targetH = 360;
    private float scaleOffset = 0;

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        _spriteBatch.Draw(_backgroundImage, new Vector2(0, 0), Color.White);
        
        var titleText = "Bailey's Super Fruit Splat";

        if (_paused)
        {
            titleText = "PAUSED";
        }
        
        var titleSize = _text.MeasureString(titleText);
        var rotation = (float)Math.Sin(_titleRotation * 50f) / 45.0f;
        var scale = Math.Abs((float)Math.Sin(_titleRotation + _titleRotation / 10)) + 0.25f + scaleOffset;
        scale = Math.Clamp(scale, 0.25f, 1f);
        _titleRotation += (float) gameTime.ElapsedGameTime.TotalSeconds;


        s = 1f;
        v = 1f;

        if (h >= 355f)
        {
            targetH = 0;
        } else if (h <= 5f)
        {
            targetH = 360;
        }

        h = MathHelper.Lerp(h, targetH, (float)gameTime.ElapsedGameTime.TotalSeconds);
        var targetColor = HsvToRgb(h, s, v);

        _spriteBatch.DrawString(
            _text, 
            titleText, 
            new Vector2(WindowWidth / 2f, WindowHeight / 2f - 50), 
            targetColor * 0.5f, 
            rotation, 
            titleSize / 2f, 
            Vector2.One * scale, 
            SpriteEffects.None,
            0
        );
        
        var removalList = new List<FallingItem>();
        foreach (var fruit in _fruitList)
        {
            if (!_paused)
            {
                fruit.Update();

            }

            fruit.Draw(_spriteBatch);

            Vector2 fruitpos = fruit.Position;

            if (fruitpos.Y > WindowHeight - 70)
            {
                removalList.Add(fruit);

                var newExplosion = new Explosion(_explosionParticleTexture, fruit.Position, _explosionRectangles);
                _explosionList.Add(newExplosion);

                _splatSound.Play();

                if (fruit.Score > 0)
                {
                    _playerScore--;   
                    
                }
                else
                {
                    _playerScore += 10;
                }
                
                _scoreScale = 1.5f;
                
                // Continue cause it can't hit the floor and the player at the same time
                continue;
            }

            var fruitRectangle = fruit.GetRectangle();

            var playerRectangle = GetPlayerRectangle();

            if (fruitRectangle.Intersects(playerRectangle))
            {
                removalList.Add(fruit);
                _playerScore += fruit.Score;

                if (fruit.Score < 0)
                {
                    _scoreScale = 5f;
                    _splatSound.Play();

                    currentColor = Color.Red;
                    
                    var newExplosion = new Explosion(_explosionParticleTexture, fruit.Position, _explosionRectangles);
                    _explosionList.Add(newExplosion);
                }
                else
                {
                    _scoreScale = 3f;
                    _coinSound.Play();
                }
                
            }
            
        }

        _playerScore = MathHelper.Clamp(_playerScore, 0, int.MaxValue);
        _scoreScale = MathHelper.Lerp(_scoreScale, _defaultScoreScale, (float)gameTime.ElapsedGameTime.TotalSeconds * 5f);
        currentColor = Color.Lerp(currentColor, targetColor, (float)gameTime.ElapsedGameTime.TotalSeconds);
        foreach (FallingItem fruit in removalList)
        {
            _fruitList.Remove(fruit);
        }

        var explosionsToRemove = new List<Explosion>();
        foreach (var explosion in _explosionList)
        {
            if (!_paused)
            {
                explosion.Update(gameTime);

            }
            explosion.Draw(_spriteBatch);

            if (explosion.IsDead)
            {
                explosionsToRemove.Add(explosion);
            }
        }

        foreach (var explosion in explosionsToRemove)
        {
            _explosionList.Remove(explosion);
        }

        _spriteBatch.Draw(_fruitBasketImage, new Vector2(_playerLocation.X, _playerLocation.Y), Color.White);
        var scoreString = _playerScore + "";
        var stringWidth = _text.MeasureString(scoreString);
        _spriteBatch.DrawString(_text, scoreString, new Vector2(WindowWidth / 2f, 10 + stringWidth.Y),
            currentColor, 0.0f, stringWidth / 2f, new Vector2(_scoreScale, _scoreScale), SpriteEffects.None, 0f);

        
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
    
    private Color HsvToRgb(double h, double S, double V)
    {
        double H = h;
        double R;
        double G;
        double B;
        if (V <= 0.0)
        {
            R = (G = (B = 0.0));
        }
        else if (S <= 0.0)
        {
            R = (G = (B = V));
        }
        else
        {
            double num = H / 60.0;
            int i = (int)Math.Floor(num);
            double f = num - (double)i;
            double pv = V * (1.0 - S);
            double qv = V * (1.0 - S * f);
            double tv = V * (1.0 - S * (1.0 - f));
            switch (i)
            {
                case 0:
                    R = V;
                    G = tv;
                    B = pv;
                    break;
                case 1:
                    R = qv;
                    G = V;
                    B = pv;
                    break;
                case 2:
                    R = pv;
                    G = V;
                    B = tv;
                    break;
                case 3:
                    R = pv;
                    G = qv;
                    B = V;
                    break;
                case 4:
                    R = tv;
                    G = pv;
                    B = V;
                    break;
                case 5:
                    R = V;
                    G = pv;
                    B = qv;
                    break;
                case 6:
                    R = V;
                    G = tv;
                    B = pv;
                    break;
                case -1:
                    R = V;
                    G = pv;
                    B = qv;
                    break;
                default:
                    R = (G = (B = V));
                    break;
            }
        }
        return new Color(MathHelper.Clamp((int)(R * 255.0), 0, 255), MathHelper.Clamp((int)(G * 255.0), 0, 255), MathHelper.Clamp((int)(B * 255.0), 0, 255));
    }
}