using PlayableCharacter;
using Skill;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class CharacterDataBase : MonoBehaviour
    {
        public CharacterSO[] characterReferences = new CharacterSO[6];
    }
}
