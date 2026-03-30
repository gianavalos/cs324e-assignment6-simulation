using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_2_assignment6;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Grid _grid;
    private Texture2D _pixel;
    private Texture2D _healthySprite;
    private Texture2D _infectedSprite;
    private Texture2D _recoveredSprite;
    private Texture2D _waterTexture;

    private const int CellSize = 16;
    private const int GridRows = 60;
    private const int GridCols = 80;
    private const float InitialInfectionRate = 0.05f;
    private Simulation _simulation;
    private const float InfectionChance = 0.3f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = GridCols * CellSize;
        _graphics.PreferredBackBufferHeight = GridRows * CellSize;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _grid = new Grid(GridRows, GridCols, InitialInfectionRate);
        _simulation = new Simulation(_grid, InfectionChance);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _waterTexture = Content.Load<Texture2D>("imgs/1");
        // Create a 1x1 white pixel texture for drawing cells
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        
        _healthySprite = Content.Load<Texture2D>("imgs/1 idle");   // first character
        _infectedSprite = Content.Load<Texture2D>("imgs/4 idle");  // green alien
        _recoveredSprite = Content.Load<Texture2D>("imgs/10 idle"); // recovered character
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _simulation.Update(gameTime);
        _grid.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_waterTexture, new Rectangle(0, 0, GridCols * CellSize, GridRows * CellSize), Color.White);
        _spriteBatch.End();

        _spriteBatch.Begin();

        for (int r = 0; r < _grid.Rows; r++)
        {
            for (int c = 0; c < _grid.Cols; c++)
            {
                Person person = _grid.GetCell(r, c);
                
                // Skip water cells
                if (person == null)
                    continue;
                
                Texture2D sprite = person.Health switch
                {
                    Person.Healthy => _healthySprite,
                    Person.Infected => _infectedSprite,
                    Person.Recovered => _recoveredSprite,
                    _ => _healthySprite
                };

                Rectangle sourceFrame = new Rectangle(0, 0, 16, 16);
                Rectangle destRect = new Rectangle(c * CellSize, r * CellSize, CellSize, CellSize);
                _spriteBatch.Draw(sprite, destRect, sourceFrame, Color.White);
            }
        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}