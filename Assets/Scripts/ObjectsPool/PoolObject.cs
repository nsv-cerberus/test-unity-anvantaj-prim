using System;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour, IDisposable
{
    protected abstract void ReturnToObjectsPool();

    public void Dispose()
    {
        // здесь нужно освободить ресурсы, если это необходимо
    }
}