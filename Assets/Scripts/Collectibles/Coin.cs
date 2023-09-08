using UnityEngine;

[RequireComponent(typeof(SaveableEntity))]
public class Coin : MonoBehaviour, ICollectible
{
    [SerializeField] int money = 10;

    SaveableEntity id;

    public void Pickup()
    {
        State.game.GainMoney(money);
        State.game.AddCollectibleObtained(id.uuid);
        Destroy(gameObject);
    }

    void Awake()
    {
        id = GetComponent<SaveableEntity>();
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
