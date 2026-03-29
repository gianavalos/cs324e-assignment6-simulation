using System;
using Microsoft.Xna.Framework;

namespace group_2_assignment6;

public class Simulation
{
    private Grid _grid;
    private float _infectionChance;
    private Random _random;
    private float _timeSinceLastStep;
    private const float StepInterval = 0.5f;

    public Simulation(Grid grid, float infectionChance)
    {
        _grid = grid;
        _infectionChance = infectionChance;
        _random = new Random();
        _timeSinceLastStep = 0f;
    }
    
    // Advance simulation one time step
    // Loops through grid and applies the infection spread rules
    // based on the health status of each person's neighbors
    public void Update(GameTime gameTime)
    {
        // Only advance the simulation every StepInterval seconds
        _timeSinceLastStep += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeSinceLastStep < StepInterval)
            return;
        _timeSinceLastStep = 0f;
        
        // First collect infections then apply
        // Cells update based on current state
        bool [,] newlyInfected = new bool[_grid.Rows, _grid.Cols];

        for (int r = 0; r < _grid.Rows; r++)
        {
            for (int c = 0; c < _grid.Cols; c++)
            {
                Person person = _grid.GetCell(r, c);
                
                // Rule: Only healthy people can become infected
                if (person.Health != Person.Healthy)
                    continue;
                
                // Check if any neighbor is infected
                foreach (Person neighbor in _grid.GetNeighbors(r, c) )
                {
                    if (neighbor.Health == Person.Infected)
                    {
                        // Apply the infection change
                        if (_random.NextDouble() < _infectionChance)
                        {
                            newlyInfected[r, c] = true;
                        }

                        break; // Only need one infected neighbor to trigger
                    }
                }
            }
            
        }
        // Apply new infections
        for (int r = 0; r < _grid.Rows; r++)
        {
            for (int c = 0; c < _grid.Cols; c++)
            {
                if (newlyInfected[r, c])
                {
                    _grid.GetCell(r, c).SpreadDisease(); // cleaner than setting Health directly
                }
            }
        }
    }
    // No draw, delegate to the grid only
    public void Draw()
    {
        _grid.Update(new GameTime());
    }
}