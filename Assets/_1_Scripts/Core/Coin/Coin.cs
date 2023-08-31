using Unity.Netcode;

namespace NomadsPlanet
{
    public abstract class Coin : NetworkBehaviour
    {
        protected int CoinValue = 10;
        protected bool AlreadyCollected;

        public abstract int Collect();

        public void SetValue(int value)
        {
            CoinValue = value;
        }

        protected void Show(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}