using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace fruitCollecting;

public class Explosion
{
    private readonly Texture2D _explosionTexture;
    private readonly Vector2 _position;
    private readonly Rectangle[] _explosionRectangles;
    private int _currentFrame;
    private float _currentFrameTime;
    private float _frameDuration = .01f;
    
    public Explosion(Texture2D explosionTexture, Vector2 position, Rectangle[] explosionRectangles)
    {
        _explosionTexture = explosionTexture;
        _position = position;
        _explosionRectangles = explosionRectangles;
    }

    public void Update(GameTime gameTime)
    {
        if (IsDead)
        {
            return;
        }
        
        _currentFrameTime += (float) gameTime.ElapsedGameTime.TotalSeconds;

        if (_currentFrameTime >= _frameDuration)
        {
            _currentFrame++;
            _currentFrameTime = 0f;
        }

        if (_currentFrame >= _explosionRectangles.Length - 1)
        {
            IsDead = true;
        }
    }

    public bool IsDead { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsDead)
        {
            return;
        }

        var explosionSize = new Vector2(GetCurrentFrame().Width, GetCurrentFrame().Height);
        spriteBatch.Draw(_explosionTexture, _position, GetCurrentFrame(), Color.White, 0.0f, explosionSize / 2.0f, Vector2.One, SpriteEffects.None, 0);
    }

    public Rectangle GetCurrentFrame()
    {
        return _explosionRectangles[_currentFrame];
    }
}