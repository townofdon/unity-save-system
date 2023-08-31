using UnityEngine;

[RequireComponent(typeof(SaveIdentifier))]
public class Coin : MonoBehaviour, ICollectible
{
    [SerializeField] int money = 10;

    SaveIdentifier id;

    public void Pickup()
    {
        State.game.GainMoney(money);
        State.game.AddCollectibleObtained(id.uuid);
        Destroy(gameObject);
    }

    void Awake()
    {
        id = GetComponent<SaveIdentifier>();
    }

    void Start()
    {
        if (State.game.GetIsCollectibleObtained(id.uuid))
        {
            Destroy(gameObject);
            return;
        }
    }
}
