using UnityEngine;

public class Coin : MonoSaveable, ICollectible
{
    [SerializeField] int money = 10;

    public override void OnGameLoad(GameState gameState)
    {
        Init();
    }

    public override void OnGameSave(ref GameState gameState) { }

    public void Pickup()
    {
        State.game.GainMoney(money);
        State.game.AddCollectibleObtained(uuid);
        Destroy(gameObject);
    }

    void Init()
    {
        if (State.game.GetIsCollectibleObtained(uuid))
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Init();
    }
}
