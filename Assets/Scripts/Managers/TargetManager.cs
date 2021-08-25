using System.Collections.Generic;
using Interface;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// List all targetables and provide lookup functions.
    /// maybe moved at entity level in the future with trigger overlap  as a register/unregister method
    /// </summary>
    public class TargetManager : MonoBehaviour
    {
        private readonly List<int> _teams = new List<int>();
        private readonly Dictionary<int, List<ITargetable>> _targetables = new Dictionary<int, List<ITargetable>>();

        /// <summary>
        /// Register a new targetable.
        /// characters register on spawn to be looked up by other.
        /// </summary>
        /// <param name="targetable">registering targetable</param>
        public void Register(ITargetable targetable)
        {
            // Register new team id. team id are never unregistered.
            if (!_targetables.ContainsKey(targetable.Team))
            {
                _teams.Add(targetable.Team);
                _targetables[targetable.Team] = new List<ITargetable>();
            }
            
            _targetables[targetable.Team].Add(targetable);
        }

        /// <summary>
        /// Unregister a targetable
        /// characters unregister on OnDestroy (or when dying).
        /// </summary>
        /// <param name="targetable"></param>
        public void UnRegister(ITargetable targetable)
        {
            _targetables[targetable.Team].Remove(targetable);
        }

        /// <summary>
        /// Return closer enemy, to a position.
        /// no linq, no foreach for fast GC free calls.
        /// </summary>
        /// <param name="sourcePosition">world position</param>
        /// <param name="sourceTeam">caller team, every other teams are included for lookup</param>
        /// <param name="range">only consider potential targets below this range</param>
        /// <returns>closest enemy or null</returns>
        public ITargetable GetCloserEnemy(Vector3 sourcePosition, int sourceTeam, float range = float.MaxValue)
        {
            var targetDistance = float.PositiveInfinity;
            ITargetable closeEnemy = null;

            // loop over each registered team
            for (int i = 0; i < _teams.Count; i++)
            {
                int team = _teams[i];
                if (team == sourceTeam)
                    continue;
                
                // loop over each targetable if not in source team
                List<ITargetable> teamList = _targetables[team];
                for (int j = 0; j < teamList.Count; j++)
                {
                    ITargetable target = teamList[j];
                    float distance = (target.Position - sourcePosition).sqrMagnitude;
                    if (distance < targetDistance)
                    {
                        targetDistance = distance;
                        closeEnemy = target;
                    }
                }
            }

            return targetDistance <= range ? closeEnemy : null;
        }
    }
}
