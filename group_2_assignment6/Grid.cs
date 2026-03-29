using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace group_2_assignment6;

public class Grid
{
    private Person[,] _cells;
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

        SeedGrid(initialInfectionRate);
    }

    // Rule 2 - Initial Infection Seeding:
    // Randomly distributes infected individuals across the grid
    // based on the configured infection rate.
    private void SeedGrid(float initialInfectionRate)
    {
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                int health = _random.NextDouble() < initialInfectionRate
                    ? Person.Infected
                    : Person.Healthy;

                _cells[r, c] = new Person(health, 5f);
            }
        }
    }

    // Gets the Person at the given row and column.
    public Person GetCell(int row, int col)
    {
        return _cells[row, col];
    }

    // Rule 1 - Boundary Handling:
    // Returns the list of neighboring Person objects for a given cell.
    // Cells on edges and corners have fewer neighbors (no wrapping).
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
                if (nr >= 0 && nr < _rows && nc >= 0 && nc < _cols)
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
                _cells[r, c].Update(gameTime);
            }
        }
    }
}
