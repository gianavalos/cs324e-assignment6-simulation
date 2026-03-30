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
    private Texture2D _sickSprite;
    private Texture2D _recoveredSprite;
    private SpriteFont _font;
    private Texture2D _waterTexture;

    private const int CellSize = 10;
    private const int GridRows = 60;
    private const int GridCols = 80;
    private const float InitialInfectionRate = 0.02f;
    private Simulation _simulation;
    private const float InfectionChance = 0.02f;

    // UI panel
    private const int PanelWidth = 160;
    private int _gridPixelWidth;

    // Replay button
    private Rectangle _replayButtonRect;
    private MouseState _previousMouse;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _gridPixelWidth = GridCols * CellSize;
        _graphics.PreferredBackBufferWidth = _gridPixelWidth + PanelWidth;
        _graphics.PreferredBackBufferHeight = GridRows * CellSize;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _grid = new Grid(GridRows, GridCols, InitialInfectionRate);
        _simulation = new Simulation(_grid, InfectionChance);
        _replayButtonRect = new Rectangle(_gridPixelWidth + 40, 15, 80, 35);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // Create a 1x1 white pixel texture for drawing cells
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _healthySprite = Content.Load<Texture2D>("imgs/1 idle");
        _sickSprite = Content.Load<Texture2D>("imgs/4 idle");
        _recoveredSprite = Content.Load<Texture2D>("imgs/10 idle");
        _waterTexture = Content.Load<Texture2D>("imgs/1");
        _font = Content.Load<SpriteFont>("DefaultFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Check for replay button click
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed &&
            _previousMouse.LeftButton == ButtonState.Released &&
            _replayButtonRect.Contains(mouse.Position))
        {
            _grid = new Grid(GridRows, GridCols, InitialInfectionRate);
            _simulation = new Simulation(_grid, InfectionChance);
        }
        _previousMouse = mouse;

        _simulation.Update(gameTime);
        _grid.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        // Water background
        _spriteBatch.Draw(_waterTexture, new Rectangle(0, 0, GridCols * CellSize, GridRows * CellSize), Color.White);
        // --- Draw grid ---
        for (int r = 0; r < _grid.Rows; r++)
        {
            for (int c = 0; c < _grid.Cols; c++)
            {
                if (!_grid.IsLand(r, c))
                    continue;

                Person person = _grid.GetCell(r, c);
                if (person == null)
                    continue;

                Texture2D sprite = person.Health switch
                {
                    Person.Healthy => _healthySprite,
                    Person.Infected => _sickSprite,
                    Person.Recovered => _recoveredSprite,
                    _ => _healthySprite
                };

                Rectangle sourceFrame = new Rectangle(0, 0, 16, 16);
                Rectangle destRect = new Rectangle(c * CellSize, r * CellSize, CellSize, CellSize);
                _spriteBatch.Draw(sprite, destRect, sourceFrame, Color.White);
            }
        }

        // --- Draw UI panel ---
        int panelX = _gridPixelWidth;
        int panelH = GridRows * CellSize;

        // Panel background
        _spriteBatch.Draw(_pixel, new Rectangle(panelX, 0, PanelWidth, panelH), new Color(30, 30, 30));

        // Replay button
        Color btnColor = _replayButtonRect.Contains(Mouse.GetState().Position)
            ? new Color(80, 80, 80)
            : new Color(60, 60, 60);
        _spriteBatch.Draw(_pixel, _replayButtonRect, btnColor);

        // Button border
        _spriteBatch.Draw(_pixel, new Rectangle(_replayButtonRect.X, _replayButtonRect.Y, _replayButtonRect.Width, 2), Color.White);
        _spriteBatch.Draw(_pixel, new Rectangle(_replayButtonRect.X, _replayButtonRect.Bottom - 2, _replayButtonRect.Width, 2), Color.White);
        _spriteBatch.Draw(_pixel, new Rectangle(_replayButtonRect.X, _replayButtonRect.Y, 2, _replayButtonRect.Height), Color.White);
        _spriteBatch.Draw(_pixel, new Rectangle(_replayButtonRect.Right - 2, _replayButtonRect.Y, 2, _replayButtonRect.Height), Color.White);

        // Button text
        string replayText = "Replay";
        Vector2 textSize = _font.MeasureString(replayText);
        Vector2 btnTextPos = new Vector2(
            _replayButtonRect.X + (_replayButtonRect.Width - textSize.X) / 2,
            _replayButtonRect.Y + (_replayButtonRect.Height - textSize.Y) / 2);
        _spriteBatch.DrawString(_font, replayText, btnTextPos, Color.White);

        // --- Legend entries ---
        int legendY = 70;
        int spriteDisplaySize = 64;
        int spriteCenterX = panelX + (PanelWidth - spriteDisplaySize) / 2;
        int spacing = 100;

        // Healthy
        _spriteBatch.Draw(_healthySprite, new Rectangle(spriteCenterX, legendY, spriteDisplaySize, spriteDisplaySize), Color.White);
        string healthyLabel = "Healthy";
        Vector2 healthySize = _font.MeasureString(healthyLabel);
        _spriteBatch.DrawString(_font, healthyLabel,
            new Vector2(panelX + (PanelWidth - healthySize.X) / 2, legendY + spriteDisplaySize + 4), Color.Green);

        // Sick
        int sickY = legendY + spacing;
        _spriteBatch.Draw(_sickSprite, new Rectangle(spriteCenterX, sickY, spriteDisplaySize, spriteDisplaySize), Color.White);
        string sickLabel = "Sick";
        Vector2 sickSize = _font.MeasureString(sickLabel);
        _spriteBatch.DrawString(_font, sickLabel,
            new Vector2(panelX + (PanelWidth - sickSize.X) / 2, sickY + spriteDisplaySize + 4), Color.Red);

        // Recovered
        int recY = legendY + spacing * 2;
        _spriteBatch.Draw(_recoveredSprite, new Rectangle(spriteCenterX, recY, spriteDisplaySize, spriteDisplaySize), Color.White);
        string recLabel = "Recovered";
        Vector2 recSize = _font.MeasureString(recLabel);
        _spriteBatch.DrawString(_font, recLabel,
            new Vector2(panelX + (PanelWidth - recSize.X) / 2, recY + spriteDisplaySize + 4), Color.CornflowerBlue);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
