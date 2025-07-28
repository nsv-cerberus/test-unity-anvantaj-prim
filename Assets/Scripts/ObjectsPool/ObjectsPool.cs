using System.Collections.Generic;

public abstract class ObjectsPool<T>
    where T : PoolObject
{
    private Stack<T> _objects = new Stack<T>();

    public T Get()
    {
        if (_objects.Count > 0)
        {
            var obj = _objects.Pop();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            return null;
        }
    }

    public void Add(T obj) {
        obj.gameObject.SetActive(false);
        _objects.Push(obj);
    }
    public void Clear() => _objects.Clear();
}
