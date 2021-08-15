using System;
using Managers;
using UnityEngine;

public class Npc : MonoBehaviour
{
    private void Start()
    {
        LevelManager.Instance.RegisterMob(gameObject);
    }

    // TODO: register character death instead of relying on destroy
    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.UnRegisterMob(gameObject);
    }
}
