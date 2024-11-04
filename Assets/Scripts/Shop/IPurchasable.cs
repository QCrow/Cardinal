
// TODO: To be moved to a dedicated decorator class (IPurchasable)
// private void TryPurchase()
// {
//     int playerGold = ShopManager.Instance.Gold;  // Get current player gold

//     if (isSold)
//     {
//         Debug.Log($"{CardName} has already been sold.");
//         return;  // Exit if the card is already sold
//     }

//     if (playerGold >= Price)
//     {
//         ShopManager.Instance.SpendGold(Price);  // Deduct gold
//         Debug.Log($"Purchased {CardName} for {Price} Gold.");

//         // Optionally: Add card to player's deck or inventory
//         CardManager.Instance.AddCardPermanently(ID);

//         isSold = true;
//         isInShop = false;
//         SoldLabel.gameObject.SetActive(true);
//     }
//     else
//     {
//         Debug.Log("Not enough gold to purchase this card.");
//     }
// }


// TODO: To be moved to a dedicated decorator class (IPurchasable)
// public void OnPointerClick(PointerEventData eventData)
// {
//     if (!isInShop) return;
//     TryPurchase();
// }


// Removed: Price and SoldLabel
// These are to be moved to a dedicated decorator class (IPurchasable)