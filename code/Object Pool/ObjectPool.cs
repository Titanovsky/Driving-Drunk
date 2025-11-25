public class ObjectPool
{
    private List<GameObject> _objects = new();

    public ObjectPool(int capacity)
    {
        _objects.Capacity = capacity;
    }

    public GameObject Get(GameObject prefab, Vector3 pos, Rotation rot)
    {
        var obj = GetFirstReleased() ?? Clone(prefab, pos, rot);

        return obj;
    }

    public bool Has(GameObject obj) => _objects.Contains(obj);
    public bool IsFull() => _objects.Count == _objects.Capacity;

    private bool IsReleased(GameObject obj) => obj.Enabled == false;

    private GameObject GetFirstReleased()
    {
        foreach (GameObject go in _objects)
        {
            if (!go.IsValid())
            {
                Log.Warning("[Object Pool] Deleted GameObject was found");

                _objects.Remove(go);

                continue;
            }

            if (IsReleased(go))
            {
                go.Enabled = true;

                return go;
            }
        }

        return IsFull() ? _objects.First() : null;
    }

    private GameObject Clone(GameObject prefab, Vector3 pos, Rotation rot)
    {
        var obj = prefab.Clone(pos, rot);
        _objects.Add(obj);

        return obj;
    }
}