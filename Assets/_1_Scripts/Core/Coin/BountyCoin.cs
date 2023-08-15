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

            if (alreadyCollected)
            {
                return 0;
            }

            alreadyCollected = true;

            SoundManager.Instance.PlayBoosterSfx();
            Destroy(gameObject);

            return coinValue;
        }
    }
}