using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Soul Hunter/Character")]
public class CharacterData : ScriptableObject
{
    public string name;
    public Sprite portrait;
    public Color nameColor = Color.white;
    public Color dialogueColor = Color.white;
}
