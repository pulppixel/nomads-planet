namespace NomadsPlanet
{
    public class BountyCoin : Coin
    {
        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (AlreadyCollected)
            {
                return 0;
            }

            AlreadyCollected = true;

            SoundManager.Instance.PlayBoosterSfx();
            Destroy(gameObject);

            return CoinValue;
        }
    }
}