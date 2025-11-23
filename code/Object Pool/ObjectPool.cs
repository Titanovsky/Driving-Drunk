public class ObjectPool
{
    private List<GameObject> _objects = new();

    private GameObject _cachePrefab;
    private Vector3 _cachePos;
    private Rotation _cacheRot;

    public ObjectPool(int capacity)
    {
        _objects.Capacity = capacity;
    }

    public GameObject Get(GameObject prefab, Vector3 pos, Rotation rot)
    {
        var obj = IsFull() ? GetFirstReleased() : Clone(prefab, pos, rot);

        _cachePrefab = prefab;
        _cachePos = pos;
        _cacheRot = rot;

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

        return _objects.First();
    }

    private GameObject Clone(GameObject prefab, Vector3 pos, Rotation rot)
    {
        var obj = prefab.Clone(pos, rot);
        _objects.Add(obj);

        return obj;
    }
}