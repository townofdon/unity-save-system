
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoSaveable
{
    [SerializeField] UnityEvent OnCheckpointReached;
    [SerializeField] new Collider2D collider;
    [SerializeField] Transform spawnPoint;

    bool isTriggered = false;

    public override void OnGameLoad(GameState gameState)
    {
        Init();
    }

    public override void OnGameSave(ref GameState gameState) { }

    void Init()
    {
        if (State.game.GetIsCheckpointReached(uuid))
        {
            TriggerCheckpointReached();
        }
    }

    void Start()
    {
        Init();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;

        if (other == null) return;
        if (string.IsNullOrEmpty(other.tag)) return;
        if (!other.CompareTag("Player")) return;

        TriggerCheckpointReached();

        State.game.AddCheckpointReached(uuid);
        State.game.SetPlayerSpawnPosition(spawnPoint != null ? spawnPoint.position : transform.position);
        GlobalEvents.OnCheckpointReached?.Invoke();
    }

    void TriggerCheckpointReached()
    {
        if (isTriggered) return;

        isTriggered = true;
        collider.enabled = false;
        OnCheckpointReached.Invoke();
    }
}
