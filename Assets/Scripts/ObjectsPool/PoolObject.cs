using System;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour, IDisposable
{
    protected abstract void ReturnToObjectsPool();

    public void Dispose()
    {
        // ����� ����� ���������� �������, ���� ��� ����������
    }
}