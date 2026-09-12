using UnityEngine;
using Xprees.Variables.Base;

namespace Xprees.Variables.Persistent
{
    [CreateAssetMenu(menuName = "Variables/Persistent/String", fileName = "String_PersistVar")]
    public class PersistentStringVariable : PersistentVariableBaseSO<string>
    {
        protected override string ReadStoredValue(string key, string fallback) => PlayerPrefs.GetString(key, fallback);

        protected override void WriteStoredValue(string key, string value) => PlayerPrefs.SetString(key, value);
    }
}