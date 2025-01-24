// using System.Collections.Generic;
// using UnityEngine;

// public abstract class Effect
// {
//     protected CardInstance _card;

//     public Effect(CardInstance card)
//     {
//         _card = card;
//     }

//     public abstract void Apply();
//     public virtual void Apply(int amount)
//     {
//         for (int i = 0; i < amount; i++)
//         {
//             Apply();
//         }
//     }
//     public abstract void Revert();
//     public virtual void Revert(int amount)
//     {
//         for (int i = 0; i < amount; i++)
//         {
//             Revert();
//         }
//     }
//     public virtual void ModifyPotency(int amount) { }
// }


// public class AddCardModifierEffect : Effect
// {
//     private readonly CardModifierType _modifierType;
//     private EffectValue _value;

//     private readonly bool _isTargeted;
//     private readonly Target _target;

//     public AddCardModifierEffect(CardInstance card, CardModifierType modifierType, EffectValue value, bool isTargeted = false, Target target = null) : base(card)
//     {
//         if (modifierType is CardModifierType.None)
//         {
//             throw new System.ArgumentException("ModifierType cannot be None.");
//         }
//         _modifierType = modifierType;
//         _value = value;
//         _isTargeted = isTargeted;
//         _target = target;
//     }

//     public override void Apply()
//     {
//         if (_isTargeted)
//         {
//             List<CardInstance> cards = _target.GetAvailableCardTargets(_card);
//             Debug.Log(cards.Count);
//             cards.ForEach(target => target.AddModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType));
//         }
//         else
//         {
//             _card.AddModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType);
//         }
//     }

//     public override void Revert()
//     {
//         if (_isTargeted)
//         {
//             _target.GetAvailableCardTargets(_card).ForEach(target => target.RemoveModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType));
//         }
//         else
//         {
//             _card.RemoveModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType);
//         }
//     }

//     public override void ModifyPotency(int amount)
//     {
//         _value.BaseValue += amount;
//     }
// }

// public class AddSlotModifierEffect : Effect
// {
//     private readonly SlotModifierType _modifierType;
//     private EffectValue _value;

//     private readonly bool _isTargeted;
//     private readonly Target _target;

//     public AddSlotModifierEffect(CardInstance card, SlotModifierType modifierType, EffectValue value, bool isTargeted = false, Target target = null) : base(card)
//     {
//         if (modifierType is SlotModifierType.None)
//         {
//             throw new System.ArgumentException("ModifierType cannot be None.");
//         }
//         _modifierType = modifierType;
//         _value = value;
//         _isTargeted = isTargeted;
//         _target = target;
//     }

//     public override void Apply()
//     {
//         if (_isTargeted)
//         {
//             _target.GetAvailableSlotTargets(_card).ForEach(target =>
//             {
//                 target.AddModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType);
//             });
//         }
//         else
//         {
//             _card.CurrentSlot.AddModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType);
//         }
//     }

//     public override void Revert()
//     {
//         if (_isTargeted)
//         {
//             _target.GetAvailableSlotTargets(_card).ForEach(target =>
//             {
//                 target.RemoveModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType);
//             });
//         }
//         else
//         {
//             _card.CurrentSlot.RemoveModifier(_modifierType, _value.GetValue(_card), _value.PersistenceType);
//         }
//     }

//     public override void ModifyPotency(int amount)
//     {
//         _value.BaseValue += amount;
//     }
// }

// public class RemoveEffect : Effect
// {
//     private readonly bool _isTargeted;
//     private readonly Target _target;
//     private readonly bool _isPermanent;

//     public RemoveEffect(CardInstance card, EffectValue effectValue, bool isTargeted = false, Target target = null) : base(card)
//     {
//         _isTargeted = isTargeted;
//         _target = target;
//         _isPermanent = effectValue.IsPermanent;
//     }

//     public override void Apply()
//     {
//         if (_isTargeted)
//         {
//             _target.GetAvailableCardTargets(_card).ForEach(target => CardSystem.Instance.DeckManager.RemoveCard(target, _isPermanent));
//         }
//         else
//         {
//             CardSystem.Instance.DeckManager.RemoveCard(_card, _isPermanent);
//         }
//     }

//     public override void Revert()
//     {
//         throw new System.NotImplementedException("Currently, DestroyEffect cannot be reverted.");
//     }
// }

// public class AddCardEffect : Effect
// {
//     private readonly int _cardID;
//     private readonly bool _isPermanent;

//     public AddCardEffect(CardInstance card, EffectValue effectValue) : base(card)
//     {
//         _cardID = effectValue.GetValue(card);
//         _isPermanent = effectValue.IsPermanent;
//     }

//     public override void Apply()
//     {
//         CardSystem.Instance.DeckManager.AddCard(_cardID, _isPermanent);
//     }

//     public override void Revert()
//     {
//         throw new System.NotImplementedException("Currently, AddCardEffect cannot be reverted.");
//     }
// }

// public class TransformEffect : Effect
// {
//     private readonly int _cardID;
//     private readonly bool _isPermanent;
//     private readonly bool _isTargeted;
//     private readonly Target _target;

//     public TransformEffect(CardInstance card, EffectValue effectValue, bool isTargeted, Target target) : base(card)
//     {
//         _cardID = effectValue.GetValue(card);
//         _isPermanent = effectValue.IsPermanent;
//         _isTargeted = isTargeted;
//         _target = target;
//     }

//     public override void Apply()
//     {
//         if (_isTargeted)
//         {
//             _target.GetAvailableCardTargets(_card).ForEach(target =>
//             {
//                 CardInstance newCard = CardSystem.Instance.DeckManager.TransformCard(target, _cardID, _isPermanent);
//                 newCard.ApplyEffect(TriggerType.WhileInPlay);
//             }
//         );
//         }
//         else
//         {
//             CardInstance newCard = CardSystem.Instance.DeckManager.TransformCard(_card, _cardID, _isPermanent);
//             newCard.ApplyEffect(TriggerType.WhileInPlay);
//         }
//     }

//     public override void Revert()
//     {
//         throw new System.NotImplementedException();
//     }
// }

// public class GainPermanentDamageAndResetEffect : Effect
// {
//     private readonly int _value;
//     private int _count = 0;
//     private readonly Target _target;

//     public GainPermanentDamageAndResetEffect(CardInstance card, EffectValue effectValue, Target target) : base(card)
//     {
//         _value = effectValue.GetValue(card);
//         _target = target;
//     }

//     public override void Apply()
//     {
//         bool exists = _target.GetAvailableCardTargets(_card).Count > 0;
//         if (exists)
//         {
//             _count++;
//             _card.AddModifier(CardModifierType.Strength, _value, ModifierPersistenceType.Permanent);
//         }
//         else
//         {
//             _card.RemoveModifier(CardModifierType.Strength, _value * _count, ModifierPersistenceType.Permanent);
//             _count = 0;
//         }
//     }

//     public override void Revert()
//     {
//         throw new System.NotImplementedException("Currently, GainPermanentDamageAndResetEffect cannot be reverted.");
//     }
// }

// public class MonoEffect : Effect
// {
//     public MonoEffect(CardInstance card) : base(card)
//     { }

//     public override void Apply()
//     {
//         Board.Instance.DeployedCards.ForEach(card =>
//         {
//             if (card == _card) return;
//             Slot slot = card.CurrentSlot;
//             CardSystem.Instance.DeckManager.RemoveCard(card, false);
//             CardInstance voidCard = CardSystem.Instance.DeckManager.AddCard(0, false);
//             voidCard.BindToSlot(slot);
//         });

//         Board.Instance.UpdateDeployedCards();

//         _card.AddModifier(CardModifierType.Strength, BattleManager.Instance.LastDealtDamage - _card.TotalAttack, ModifierPersistenceType.Battle);
//         _card.UpdateAttackValue();
//     }

//     public override void Revert()
//     {
//         throw new System.NotImplementedException("Currently, GainPermanentDamageAndResetEffect cannot be reverted.");
//     }
// }