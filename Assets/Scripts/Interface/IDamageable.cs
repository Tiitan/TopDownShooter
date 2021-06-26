using System.ComponentModel;
using UnityEngine;

namespace Interface
{
    public interface IDamageable: INotifyPropertyChanged
    {
        public float HealthPoint { get; }
        
        public float MaxHealthPoint { get; }
        
        Vector3 Position { get; }
    }
}
