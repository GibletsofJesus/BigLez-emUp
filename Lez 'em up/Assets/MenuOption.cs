using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class MenuOption : ScriptableObject {
    public string title;
    public bool unlocked;
    public Behaviour action;
}
