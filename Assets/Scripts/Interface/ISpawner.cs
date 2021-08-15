using System.Collections.Generic;

namespace Interface
{
    public interface ISpawner
    {
        /// <summary>
        /// Wave that will spawn a mob.
        /// </summary>
        IEnumerable<int> Waves { get; }
        
        /// <summary>
        /// Spawn mobs.
        /// </summary>
        /// <param name="currentWave"></param>
        /// <returns>Number of mobs spawned by this spawner on this wave. Used to calculate wave progress.</returns>
        int Spawn(int currentWave);
    }
}
