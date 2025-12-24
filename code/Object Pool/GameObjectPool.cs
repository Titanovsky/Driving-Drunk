public class GameObjectPool
{
    private List<GameObject> _objects = new();
    private bool _isNetwork = false;

    public GameObjectPool(int capacity, bool isNetwork)
    {
        _objects.Capacity = capacity;
        _isNetwork = isNetwork;
    }

    public GameObject Get(GameObject prefab, CloneConfig config)
    {
        var obj = GetFirstReleased() ?? Clone(prefab, config);

        return obj;
    }

    public bool Has(GameObject obj) => _objects.Contains(obj);
    public bool IsFull() => _objects.Count == _objects.Capacity;

    private bool IsReleased(GameObject obj) => obj.Enabled == false;

    private GameObject GetFirstReleased()
    {
        // идём с конца, чтобы RemoveAt не сдвигал ещё не просмотренные элементы
        for (int i = _objects.Count - 1; i >= 0; i--)
        {
            var go = _objects[i];

            if (!go.IsValid())
            {
                Log.Warning("[Object Pool] Deleted GameObject was found");
                _objects.RemoveAt(i);

                continue;
            }

            if (IsReleased(go))
            {
                go.Enabled = true;

                return go;
            }
        }

        return IsFull() ? _objects.FirstOrDefault() : null;
    }

    private GameObject Clone(GameObject prefab, CloneConfig config)
    {
        var obj = prefab.Clone(config);
        if (_isNetwork)
            obj.NetworkSpawn(Connection.Host);

        _objects.Add(obj);

        return obj;
    }
}