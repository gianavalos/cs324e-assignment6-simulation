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

    private const int CellSize = 10;
    private const int GridRows = 60;
    private const int GridCols = 80;
    private const float InitialInfectionRate = 0.01f;
    private Simulation _simulation;
    private const float InfectionChance = 0.01f;

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

        // Create a 1x1 white pixel texture for drawing cells
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
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
        GraphicsDevice.Clear(Color.DarkBlue);

        _spriteBatch.Begin();

        for (int r = 0; r < _grid.Rows; r++)
        {
            for (int c = 0; c < _grid.Cols; c++)
            {
                // Water cells - draw ocean
                if (!_grid.IsLand(r, c))
                {
                    _spriteBatch.Draw(
                        _pixel,
                        new Rectangle(c * CellSize, r * CellSize, CellSize, CellSize),
                        Color.DarkBlue);
                    continue;
                }

                Person person = _grid.GetCell(r, c);
                Color color = person.Health switch
                {
                    Person.Healthy => Color.Green,
                    Person.Infected => Color.Red,
                    Person.Recovered => Color.CornflowerBlue,
                    _ => Color.White
                };

                // Draw black border first (full cell)
                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle(c * CellSize, r * CellSize, CellSize, CellSize),
                    Color.Black);

                // Draw colored cell inset by 1 pixel on each side
                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle(c * CellSize + 1, r * CellSize + 1, CellSize - 2, CellSize - 2),
                    color);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}