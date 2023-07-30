namespace NomadsPlanet
{
    public class RespawningCoin : Coin
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

            return coinValue;
        }
    }
}