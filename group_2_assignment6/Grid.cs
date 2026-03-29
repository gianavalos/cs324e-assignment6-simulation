using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace group_2_assignment6;

public class Grid
{
    private Person[,] _cells;
    private bool[,] _landMap;
    private int _rows;
    private int _cols;
    private Random _random;

    public int Rows => _rows;
    public int Cols => _cols;

    public Grid(int rows, int cols, float initialInfectionRate)
    {
        _rows = rows;
        _cols = cols;
        _random = new Random();
        _cells = new Person[rows, cols];
        _landMap = GenerateMap();

        SeedGrid(initialInfectionRate);
    }

    // Rule 2 - Initial Infection Seeding:
    // Randomly distributes infected individuals across land cells
    // based on the configured infection rate.
    private void SeedGrid(float initialInfectionRate)
    {
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                if (!_landMap[r, c])
                {
                    _cells[r, c] = null;
                    continue;
                }

                int health = _random.NextDouble() < initialInfectionRate
                    ? Person.Infected
                    : Person.Healthy;

                _cells[r, c] = new Person(health, 3f);
            }
        }
    }

    // Returns true if the cell at (row, col) is land.
    public bool IsLand(int row, int col)
    {
        return _landMap[row, col];
    }

    // Gets the Person at the given row and column. Returns null for water cells.
    public Person GetCell(int row, int col)
    {
        return _cells[row, col];
    }

    // Rule 1 - Boundary Handling:
    // Returns the list of neighboring Person objects for a given cell.
    // Cells on edges and corners have fewer neighbors (no wrapping).
    // Water cells are skipped - only land neighbors are returned.
    // Checks all 8 surrounding directions (Moore neighborhood).
    public List<Person> GetNeighbors(int row, int col)
    {
        List<Person> neighbors = new List<Person>();

        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                // Skip the cell itself
                if (dr == 0 && dc == 0)
                    continue;

                int nr = row + dr;
                int nc = col + dc;

                // Boundary check - edges have fewer neighbors
                if (nr >= 0 && nr < _rows && nc >= 0 && nc < _cols
                    && _cells[nr, nc] != null)
                {
                    neighbors.Add(_cells[nr, nc]);
                }
            }
        }

        return neighbors;
    }

    // Updates every Person in the grid each time step.
    public void Update(GameTime gameTime)
    {
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                _cells[r, c]?.Update(gameTime);
            }
        }
    }

    // Generates a predefined island map with jagged coastlines and choke points.
    // 5 islands connected through a central hub via narrow bridges.
    // Uses a seeded hash for deterministic jagged edges (same map every run).
    private bool[,] GenerateMap()
    {
        bool[,] map = new bool[_rows, _cols];

        // Island centers (row, col) and base radii
        // Top-Left, Top-Right, Bottom-Left, Bottom-Right, Center
        int[][] islands = new int[][]
        {
            new int[] { 14, 16, 12 },   // Top-Left
            new int[] { 14, 63, 12 },   // Top-Right
            new int[] { 45, 16, 12 },   // Bottom-Left
            new int[] { 45, 63, 12 },   // Bottom-Right
            new int[] { 30, 40, 8 },    // Center
        };

        // Fill jagged islands
        foreach (var island in islands)
        {
            FillJaggedIsland(map, island[0], island[1], island[2]);
        }

        // Bridges connecting all 4 corners to center (2-cell wide, with slight jag)
        // Top-Left to Center
        FillBridge(map, 14, 16, 30, 40, 2);
        // Top-Right to Center
        FillBridge(map, 14, 63, 30, 40, 2);
        // Bottom-Left to Center
        FillBridge(map, 45, 16, 30, 40, 2);
        // Bottom-Right to Center
        FillBridge(map, 45, 63, 30, 40, 2);

        return map;
    }

    // Creates a jagged island by checking distance from center with noise.
    // Uses a deterministic hash so the shape is the same every run.
    private void FillJaggedIsland(bool[,] map, int centerRow, int centerCol, int radius)
    {
        for (int r = centerRow - radius - 2; r <= centerRow + radius + 2; r++)
        {
            for (int c = centerCol - radius - 2; c <= centerCol + radius + 2; c++)
            {
                if (r < 0 || r >= _rows || c < 0 || c >= _cols)
                    continue;

                double dx = c - centerCol;
                double dy = r - centerRow;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                // Use angle-based noise for jagged coastline
                double angle = Math.Atan2(dy, dx);
                double noise = CoastNoise(centerRow + centerCol, angle);

                // Cells within the noisy radius are land
                if (dist < radius + noise)
                {
                    map[r, c] = true;
                }
            }
        }
    }

    // Deterministic noise function that varies by angle to create jagged coastlines.
    // seed keeps each island's shape unique.
    private double CoastNoise(int seed, double angle)
    {
        // Layer multiple sine waves at different frequencies for natural-looking edges
        double n = 0;
        n += 2.5 * Math.Sin(angle * 3 + seed * 1.7);
        n += 1.5 * Math.Sin(angle * 7 + seed * 3.1);
        n += 1.0 * Math.Sin(angle * 13 + seed * 5.3);
        return n;
    }

    // Draws a narrow bridge between two points (r1,c1) -> (r2,c2).
    // width controls how many cells wide the bridge is.
    private void FillBridge(bool[,] map, int r1, int c1, int r2, int c2, int width)
    {
        int steps = Math.Max(Math.Abs(r2 - r1), Math.Abs(c2 - c1));
        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            int r = (int)(r1 + (r2 - r1) * t);
            int c = (int)(c1 + (c2 - c1) * t);

            // Fill cells in a small cross pattern for the bridge width
            for (int dr = -width / 2; dr <= width / 2; dr++)
            {
                for (int dc = -width / 2; dc <= width / 2; dc++)
                {
                    int nr = r + dr;
                    int nc = c + dc;
                    if (nr >= 0 && nr < _rows && nc >= 0 && nc < _cols)
                    {
                        map[nr, nc] = true;
                    }
                }
            }
        }
    }
}
