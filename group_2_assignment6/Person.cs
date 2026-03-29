using Microsoft.Xna.Framework;

namespace group_2_assignment6;

public class Person
{
    // Health states
    public const int Healthy = 0;
    public const int Infected = 1;
    public const int Recovered = 2;

    public int Health { get; set; }
    public float ElapsedTime { get; set; }
    public float MaxInfectedTime { get; set; }

    public Person(int health, float maxInfectedTime)
    {
        Health = health;
        ElapsedTime = 0f;
        MaxInfectedTime = maxInfectedTime;
    }

    public void Update(GameTime gameTime)
    {
        // RULE(Disease Progression): An infected person remains infected for a fixed number of time steps
        if (Health == Infected)
        {
            ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // RULE(Recovery): After the infection period ends, the infected person becomes recovered and can
            // no longer spread or catch the disease
            if (ElapsedTime >= MaxInfectedTime)
            {
                Health = Recovered;
                ElapsedTime = 0f;
            }
        }
    }
    
    // Spread a disease and let a person be infected
    public void SpreadDisease()
    {
        if (Health == Healthy)
        {
            Health = Infected;
            ElapsedTime = 0f;
        }
    }
}
