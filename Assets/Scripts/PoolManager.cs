using UnityEngine;
using System.Collections.Generic;
using System;

public class PoolManager : Manager<PoolManager>
{
    [SerializeField] private List<PoolableObject> poolableObjects;
    private Dictionary<string, PoolableObject> poolDictionary = new Dictionary<string, PoolableObject>();

    private void Awake()
    {
        foreach (PoolableObject poolableObject in poolableObjects)
        {
            poolableObject.Initialize();
            poolDictionary.Add(poolableObject.Name, poolableObject);
        }
    }

    public GameObject Instantiate(GameObject prefab)
    {
        return Instantiate(prefab, Vector3.zero, Quaternion.identity, null);
    }

    public GameObject Instantiate(GameObject prefab, Transform parent)
    {
        return Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
    }

    public GameObject Instantiate(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
    {
        if (instantiateInWorldSpace)
        {
            return Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        }
        else
        {
            return Instantiate(prefab, parent.transform.position, parent.transform.rotation, parent);
        }
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation, null);
    }

    public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        poolDictionary.TryGetValue(prefab.name, out PoolableObject poolableObject);
        return poolableObject.Instantiate(position, rotation, parent);
    }

    public void Destroy(GameObject copy)
    {
        poolDictionary.TryGetValue(copy.name, out PoolableObject poolableObject);
        poolableObject.Destroy(copy);
    }

    public void CheckForDuplicateCopies()
    {
        foreach (PoolableObject poolableObject in poolableObjects)
        {
            poolableObject.CheckForDuplicateCopies();
        }
    }









    [Serializable]
    private class PoolableObject
    {
        public string Name { get; private set; }
        [SerializeField] private GameObject Prefab;
        [SerializeField] private float capacity = 2;
        private LinkedList<GameObject> copies;

        public void Initialize()
        {
            copies = new LinkedList<GameObject>();
            Name = Prefab.name;

            for (int i = 0; i < capacity; i++)
            {
                AddOneCopy();
            }
        }

        private void Expand()
        {
            for (int i = 0; i < capacity; i++)
            {
                AddOneCopy();
            }

            capacity = capacity * 2;
        }

        private void AddOneCopy()
        {
            GameObject copy = UnityEngine.Object.Instantiate(Prefab, Instance.transform.position, Quaternion.identity, Instance.transform);
            copy.name = Name;

            copies.AddFirst(copy);
            copy.SetActive(false);
        }

        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent)
        {
            // If there are no more available copies, expand the list of copies.
            if (copies.Count == 0)
            {
                Expand();
            }

            // Grab a copy.
            GameObject copy = copies.Last.Value;
            copies.RemoveLast();
            copy.SetActive(true);

            // Move the copy.
            copy.transform.position = position;
            copy.transform.rotation = rotation;
            copy.transform.SetParent(parent, true);

            return copy;
        }

        public void Destroy(GameObject copy)
        {
            copies.AddFirst(copy);
            copy.SetActive(false);
        }

        // De-duplicates the pool if needed.
        // This is a slow process that should never need to be called.
        public void CheckForDuplicateCopies()
        {
            foreach (GameObject copy1 in copies)
            {
                int matches = 0;

                foreach (GameObject copy2 in copies)
                {
                    if (copy1 == copy2)
                    {
                        matches++;
                    }
                }

                // Each entry will always match with itself.
                // Therefore, if there is more than that one match, there is a duplicate entry.
                if (matches > 1)
                {
                    Debug.LogError("Duplicate copy of " + copy1.name + " in pool (ID = " + copy1.GetInstanceID() + ") Fixing!");
                    EliminateDuplicateCopies();
                    return;
                }
            }
        }

        // De-duplicates the pool.
        // This is a slow process that should never need to be called.
        private void EliminateDuplicateCopies()
        {
            LinkedList<GameObject> copiesNew = new LinkedList<GameObject>();

            foreach (GameObject copy in copies)
            {
                if (!copiesNew.Contains(copy))
                {
                    copiesNew.AddFirst(copy);
                }
            }

            copies = copiesNew;
        }
    }
}