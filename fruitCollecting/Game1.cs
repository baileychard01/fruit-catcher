using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace fruitCollecting;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D backgroundImage;
    Texture2D fruitBasketimage;
    new Vector2 playerLocation;
    public int speed;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 928;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = 793;   // set this value to the desired height of your window
        _graphics.ApplyChanges();
        
        playerLocation.X = 100;
        playerLocation.Y = 643;
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        speed = 5;
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        backgroundImage = Content.Load<Texture2D>("Background");
        fruitBasketimage = Content.Load<Texture2D>("gamePlayer");


    }

    protected override void Update(GameTime gameTime)
    {
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            playerLocation.X += speed;
        }
        
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            playerLocation.X -= speed;
        }

        
        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        
        _spriteBatch.Draw(backgroundImage, new Vector2(0, 0), Color.White);
        _spriteBatch.Draw(fruitBasketimage,new Vector2(playerLocation.X,playerLocation.Y),Color.White);
        _spriteBatch.End();
        

        base.Draw(gameTime);
    }
}