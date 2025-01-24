// using System;
// using System.Linq;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using System.Runtime.InteropServices.WindowsRuntime;

// public enum TargetRangeType
// {
//     None, // Only used for default value
//     Adjacent,
//     Row,
//     Column,
//     Front,
//     Middle,
//     Back,
//     Center,
//     Diagonal,
//     Corner,
//     Edge,
//     Random,
//     All,
//     Others
// }

// [Serializable]
// public class Target
// {
//     public TargetRangeType TargetRange;
//     [InlineProperty]
//     public Filter TargetFilter;

//     public Target(TargetRangeType targetRange, Filter targetFilter)
//     {
//         TargetRange = targetRange;
//         TargetFilter = targetFilter;
//     }

//     private List<CardInstance> TargetSlotsToCards(List<Slot> slots)
//     {
//         return slots.ConvertAll(slot => slot.Content as CardInstance).Where(card => card != null).ToList();
//     }

//     public List<CardInstance> GetAvailableCardTargets(CardInstance src)
//     {
//         List<CardInstance> targets = new();
//         List<Slot> slots = new();
//         switch (TargetRange)
//         {
//             case TargetRangeType.All:
//                 slots = Board.Instance.GetAllSlots().SelectMany(slotList => slotList).ToList();
//                 break;
//             case TargetRangeType.Others:
//                 slots = Board.Instance.GetAllSlots().SelectMany(slotList => slotList).ToList();
//                 slots.Remove(src.CurrentSlot);
//                 break;
//             case TargetRangeType.Adjacent:
//                 slots = src.CurrentSlot.Neighbors;
//                 break;
//             case TargetRangeType.Row:
//                 slots = Board.Instance.GetRow(src.CurrentSlot.Row);
//                 break;
//             case TargetRangeType.Column:
//                 slots = Board.Instance.GetColumn(src.CurrentSlot.Col);
//                 break;
//             case TargetRangeType.Front:
//                 slots = Board.Instance.GetColumn(0);
//                 break;
//             case TargetRangeType.Middle:
//                 slots = Board.Instance.GetColumn(1);
//                 break;
//             case TargetRangeType.Back:
//                 slots = Board.Instance.GetColumn(2);
//                 break;
//             case TargetRangeType.Center:
//                 slots = Board.Instance.GetRow(1).FindAll(slot => slot.Col == 1);
//                 break;
//             default:
//                 break;
//         }
//         targets = TargetSlotsToCards(slots);

//         if (TargetFilter != null)
//         {
//             targets = targets.FindAll(card => TargetFilter.IsMatch(card));
//         }

//         return targets;
//     }

//     public List<Slot> GetAvailableSlotTargets(CardInstance src)
//     {
//         List<Slot> slots = new();
//         switch (TargetRange)
//         {
//             case TargetRangeType.Adjacent:
//                 slots = src.CurrentSlot.Neighbors;
//                 break;
//             case TargetRangeType.Row:
//                 slots = Board.Instance.GetRow(src.CurrentSlot.Row);
//                 break;
//             case TargetRangeType.Column:
//                 slots = Board.Instance.GetColumn(src.CurrentSlot.Col);
//                 break;
//             case TargetRangeType.Front:
//                 slots = Board.Instance.GetColumn(0);
//                 break;
//             case TargetRangeType.Middle:
//                 slots = Board.Instance.GetColumn(1);
//                 break;
//             case TargetRangeType.Back:
//                 slots = Board.Instance.GetColumn(2);
//                 break;
//             case TargetRangeType.Center:
//                 slots = Board.Instance.GetRow(1).FindAll(slot => slot.Col == 1);
//                 break;
//             default:
//                 break;
//         }
//         return slots;
//     }
// }