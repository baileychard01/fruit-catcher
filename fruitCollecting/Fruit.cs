using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace fruitCollecting;


public class Fruit
{
    public int Score;
    private readonly float _rotationSpeed;
    public  Vector2 Position;

    private readonly Texture2D _fruitTexture;
    private readonly Rectangle _rectangle;
    private readonly float _dropSpeed;
    private float _currentRotation;
    
    public Fruit(Texture2D fruitTexture, Rectangle rectangle,Vector2 position,float dropSpeed,int score, float rotationSpeed)
    {
        Score = score;
        _rotationSpeed = rotationSpeed;
        _fruitTexture = fruitTexture;
        _rectangle = rectangle;
        Position = position;
        _dropSpeed = dropSpeed;
    }

    private float GetScale()
    {
        return 5f / Score + 1;
    }
    public Rectangle GetRectangle()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)(16 * GetScale()), (int)(16 * GetScale()));
    }
    
    public void Update()
    {
        Position.Y += _dropSpeed;
        _currentRotation += _rotationSpeed;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // At half screen the fruits are full visibility
        var fadeLevel = MathHelper.Lerp(0.5f, 1.0f, Position.Y / ( Game1.WindowHeight / 2.0f ));
        var invisibility = (float) Score / Game1.MaxScore;
        // Better score fruits are more invisible
        var finalColor = Color.Lerp(Color.White, Color.White * 0.25f, invisibility);
        
        spriteBatch.Draw(
            _fruitTexture,Position,
            _rectangle,
            finalColor * fadeLevel, 
            _currentRotation, 
            new Vector2(16, 16) / 2f, 
            new Vector2(GetScale(), GetScale()), // Better score fruits are smaller
            SpriteEffects.None, 
            0
        );
    }

    
}

