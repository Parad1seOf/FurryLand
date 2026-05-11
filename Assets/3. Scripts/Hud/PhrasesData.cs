using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PhrasesData", menuName = "Dialogues/PhrasesData")]
public class PhrasesData : ScriptableObject
{
    [System.Serializable]
    public struct Phrase
    {
        public string id;
        [TextArea] public string text;
    }

    public List<Phrase> phraseslist;

    public string GetPhrase(string id)
    {
        var input = phraseslist.Find(x => x.id == id);
        return !string.IsNullOrEmpty(input.text) ? input.text : string.Empty;
    }

}
