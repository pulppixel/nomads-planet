using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

namespace NomadsPlanet
{
    public class BoosterZone : NetworkBehaviour
    {
        [SerializeField] private Light[] pointLight;

        private const float MaxBoostPower = 10f;
        private const float BoosterCooldown = 60f;
        private const float BoosterTickRate = 5f;

        private float _remainingCooldown;
        private float _tickTimer;

        private readonly List<DrivingPlayer> _playersInZone = new();

        private readonly NetworkVariable<float> _boostPower = new();

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _boostPower.OnValueChanged += HandleBoosterPowerChanged;
                HandleBoosterPowerChanged(0, _boostPower.Value);
            }

            if (IsServer)
            {
                _boostPower.Value = MaxBoostPower;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _boostPower.OnValueChanged -= HandleBoosterPowerChanged;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
            {
                return;
            }

            if (!other.attachedRigidbody.TryGetComponent(out DrivingPlayer player))
            {
                return;
            }

            if (_playersInZone.Contains(player))
            {
                return;
            }

            _playersInZone.Add(player);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsServer)
            {
                return;
            }

            if (!other.attachedRigidbody.TryGetComponent(out DrivingPlayer player))
            {
                return;
            }

            if (!_playersInZone.Contains(player))
            {
                return;
            }

            _playersInZone.Remove(player);
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }

            if (_remainingCooldown > 0f)
            {
                _remainingCooldown -= Time.deltaTime;

                if (_remainingCooldown <= 0f)
                {
                    _boostPower.Value = MaxBoostPower;
                }
                else
                {
                    return;
                }
            }

            _tickTimer += Time.deltaTime;
            if (_tickTimer >= 1 / BoosterTickRate)
            {
                foreach (DrivingPlayer player in _playersInZone)
                {
                    if (_boostPower.Value == 0)
                    {
                        break;
                    }

                    player.CarController.StartBoost();
                    _boostPower.Value -= 1;

                    if (_boostPower.Value == 0)
                    {
                        _remainingCooldown = BoosterCooldown;
                    }
                }

                _tickTimer %= (1 / BoosterTickRate);
            }
        }

        private void HandleBoosterPowerChanged(float oldBoostPower, float newBoostPower)
        {
            float difference = Mathf.Clamp01(newBoostPower - oldBoostPower);
            Color targetColor = Color.Lerp(Color.black, new Color(0.33f, 1f, 0.9f), difference);

            _SetAllLightColors(targetColor);
        }

        private void _SetAllLightColors(Color targetColor)
        {
            foreach (var light1 in pointLight)
            {
                DOTween.Kill(light1);
                light1.DOColor(targetColor, 1f);
            }
        }
    }
}