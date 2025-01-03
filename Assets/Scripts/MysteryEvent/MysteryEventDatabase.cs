using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MysteryEventDatabase", menuName = "MyGame/MysteryEventDatabase")]
public class MysteryEventDatabase : ScriptableObject
{
    public List<MysteryEvent> events;
}

