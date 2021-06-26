using Managers;
using UnityEngine;

namespace Interface
{
    public interface ITargetable
    {
        int Team { get; }
        
        Vector3 Position { get; }
    }
}
