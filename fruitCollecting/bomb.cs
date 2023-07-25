using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace fruitCollecting;

public class bomb
{
    public readonly Texture2D _bombImage;

    public bomb(Texture2D bombImage)
    {
        _bombImage = bombImage;
    }

    

    public void Update()
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_bombImage,new Vector2(0,0),Color.White);
    }
}