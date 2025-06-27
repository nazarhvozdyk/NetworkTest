using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    private NetworkVariable<byte> networkCurrentHealth = new NetworkVariable<byte>();
    [SerializeField] private int startHealth = 100;
    public int currentHealth { get; private set; }
    public delegate void OnHealthChanged(int currentValue);
    public event OnHealthChanged onHealthChanged;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            currentHealth = startHealth;
            networkCurrentHealth.Value = (byte)startHealth;
            onHealthChanged?.Invoke(currentHealth);
        }
        else
        {
            currentHealth = networkCurrentHealth.Value;
            onHealthChanged?.Invoke(currentHealth);
            networkCurrentHealth.OnValueChanged += OnValueChanged;
        }
    }

    private void OnValueChanged(byte previousValue, byte newValue)
    {
        currentHealth = newValue;
        onHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        networkCurrentHealth.Value = (byte)currentHealth;
        onHealthChanged?.Invoke(currentHealth);
    }
}