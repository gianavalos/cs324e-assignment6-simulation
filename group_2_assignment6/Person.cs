using Microsoft.Xna.Framework;

namespace group_2_assignment6;

// Stub class for Person - Jinsoo will expand this.
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
        if (Health == Infected)
        {
            ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ElapsedTime >= MaxInfectedTime)
            {
                Health = Recovered;
                ElapsedTime = 0f;
            }
        }
    }
}
