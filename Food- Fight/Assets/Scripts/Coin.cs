﻿using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;
    
    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!pickedUp && collider.CompareTag("Player")) {

            BankAccountManager wallet = collider.GetComponent<BankAccountManager>();

            if (wallet == null) {

                Debug.Log("Player missing BankAccountManager script!");

            } else if (wallet.CanDeposit(value)) {

                pickedUp = true;
                wallet.Deposit(value);
                Debug.Log("Added " + value + " coins to your wallet!!");
                Destroy(this.gameObject, 0.1f);

            } else {
                Debug.Log("Wallet is full! Cannot add more!!!");
            }
        }
    }

}
