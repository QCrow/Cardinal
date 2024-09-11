using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevTasks
{
    /// <summary>
    /// Sets the resource value for the specified resource type to the given value.
    /// Usage: sr <type> <amt> 
    /// Allowed flags: none
    /// Alias: sr
    /// </summary>
    /// <param name="resourceType">The type of the resource (Energy, Food, Morale).</param>
    /// <param name="value">The value to set for the resource.</param>
    [DevCommand("\nsr\tSet a resouce to certain value", "Usage: sr <type> <amt>", new string[] {}, "sr")]
    public static void SetResourceValue(ResourceType resourceType, int value)
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.SetResourceCurrentValue(resourceType, value);
            Debug.Log($"Set {resourceType} to {value}");
        }
        else
        {
            Debug.LogWarning("ResourceManager instance not found.");
        }
    }

    /// <summary>
    /// Adds certain amount of cards to the hand by CardID, if no amount is provided, add one
    /// Alias: ac
    /// Allowed flags: none
    /// Usage: ac <id> [<amt>]
    /// </summary>
    /// <param name="cardID">The CardID.</param>
    /// <param name="amount">The amount of card to add.</param>
    [DevCommand("\nac\tAdd a card by ID","Usage: ac <id> [<amt>]", new string[] {}, "ac")]
    public static void AddCardToHand(int cardID, int amount = 1)
    {
        if (Hand.Instance.AddCardByID(cardID, amount, true))
        {
            Debug.Log($"Card with ID {cardID} added to hand.");
        }
        else
        {
            Debug.LogWarning($"Failed to add card with ID {cardID}.");
        }
    }

    /// <summary>
    /// Destroys a card from the hand by position (index), if flag -a is provided, destroy all cards in hand.
    /// Alias: rmh
    /// Allowed flags: -a
    /// Usage: rmh <index> [-a]
    /// </summary>
    /// <param name="arguments">Array containing only the position index.</param>
    /// <param name="flags">Array containing flags, such as `-a` to destroy all cards in hand.</param>
    [DevCommand("\nrmh\tRemove a card from hand by index or with flags","Usage: rmh <index> [-a]", new string[] { "-a" }, "rmh")]
    public static void DestroyCardFromHand(string[] arguments, TerminalFlag[] flags)
    {
        bool destroyAll = flags.Any(f => f.Is("-a"));

        // If the -a flag is present, destroy all cards
        if (destroyAll)
        {
            Hand.Instance.DestroyAllCards(true);
            Debug.Log("All cards removed from hand.");
            return;
        }

        // Handle the case where an index is provided
        if (arguments.Length > 0)
        {
            if (int.TryParse(arguments[0], out int index))
            {
                // Remove the card at the specified index
                if (Hand.Instance.DestroyCard(index, true))
                {
                    Debug.Log($"Card at index {index} removed from hand.");
                }
                else
                {
                    Debug.LogWarning($"Failed to remove card at index {index}.");
                }
            }
            else
            {
                string warningMessage = "Invalid index provided: " + arguments[0];
                Debug.LogError(warningMessage);
                DevTerminal.Instance?.LogToTerminal(warningMessage);
            }
        }
        else
        {
            Debug.LogError("You must provide either an index or a valid flag.");
        }
    }

    /// <summary>
    /// Destroy a card in board given its row and column, if flag -a is provided, destroy all cards in board
    /// Alias: rmb
    /// Allowed flags: -a
    /// Usage: rmb <row> <col> [-a]
    /// </summary>
    /// <param name="arguments">Array containing the row and column values as strings.</param>
    /// <param name="flags">Array containing flags, such as `-a` to remove all cards.</param>
    [DevCommand("\nrmb\tRemove a card from the board at a specific row and column", "Usage: rmb <row> <col> [-a]", new string[] { "-a" }, "rmb")]
    public static void RemoveCardFromBoard(string[] arguments, TerminalFlag[] flags)
    {
        bool destroyAll = flags.Any(f => f.Is("-a"));

        if (destroyAll)
        {
            List<List<Slot>> slots = Board.Instance.GetAllSlots();

            // Loop through each row and column
            for (int i = 0; i < slots.Count; i++)
            {
                for (int j = 0; j < slots[i].Count; j++)
                {
                    Slot s = slots[i][j];
                    RemoveCardFromSlot(s, i, j); // Call the helper method for each slot
                }
            }

            return;  // Exit after removing all cards
        }

        if (arguments.Length != 2)
        {
            string errorMessage = "Invalid number of arguments. Usage: rmb <row> <col> [-a]";
            Debug.LogError(errorMessage);
            DevTerminal.Instance?.LogToTerminal(errorMessage);
            return;
        }

        // Parse the arguments to row and column
        if (!int.TryParse(arguments[0], out int row) || !int.TryParse(arguments[1], out int column))
        {
            string errorMessage = "Invalid arguments. Both row and column must be integers.";
            Debug.LogError(errorMessage);
            DevTerminal.Instance?.LogToTerminal(errorMessage);
            return;
        }

        // Remove a card at the specific position if destroyAll is not set
        Slot? slot = Board.Instance.GetSlotAtPosition(row, column);
        RemoveCardFromSlot(slot, row, column);
    }

    /// <summary>
    /// Helper method to remove a card from a slot and handle logging.
    /// </summary>
    /// <param name="slot">The slot to remove the card from.</param>
    /// <param name="row">The row of the slot.</param>
    /// <param name="col">The column of the slot.</param>
    private static void RemoveCardFromSlot(Slot? slot, int row, int col)
    {
        if (slot == null)
        {
            string errorMessage = $"Invalid slot at position ({row}, {col}).";
            Debug.LogError(errorMessage);
            DevTerminal.Instance?.LogToTerminal(errorMessage);
        }
        else if (slot.Card == null)
        {
            string warningMessage = $"No card found in slot at position ({row}, {col}).";
            Debug.LogWarning(warningMessage);
            DevTerminal.Instance?.LogToTerminal(warningMessage);
        }
        else
        {
            // Force remove the card from the slot
            slot.Card.ForceRemove();
            string successMessage = $"Card removed from slot at position ({row}, {col}).";
            Debug.Log(successMessage);
            DevTerminal.Instance?.LogToTerminal(successMessage);
        }
    }

}