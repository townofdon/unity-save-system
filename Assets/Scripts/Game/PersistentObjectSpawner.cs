using UnityEngine;

namespace RPG.Core
{

    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjects;

        static bool hasSpawned = false;

        void Awake()
        {
            if (hasSpawned) return;
            hasSpawned = true;
            SpawnPersistentObjects();
        }

        void SpawnPersistentObjects()
        {
            GameObject spawned = Instantiate(persistentObjects);
            DontDestroyOnLoad(spawned);
        }
    }
}
