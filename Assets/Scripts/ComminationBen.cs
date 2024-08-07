using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewComminationBen", menuName = "Game/Create ComminationBen")]
public class ComminationBen : ScriptableObject
{
    public string[] writeValues;
    public string[] values;
    public string[] wrongValues;
    public bool isWriting;
    public string id;
    public string functionID;
    public bool isRandom;
    public string nameID;
    public AudioClip[] benSounds;
    public bool isSound;
}
