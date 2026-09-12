using UnityEngine;
using Xprees.Variables.Base;

namespace Xprees.Variables.Persistent
{
    [CreateAssetMenu(menuName = "Variables/Persistent/Int", fileName = "Int_PersistVar")]
    public class PersistentIntVariable : PersistentVariableBaseSO<int>
    {
        protected override int ReadStoredValue(string key, int fallback) => PlayerPrefs.GetInt(key, fallback);

        protected override void WriteStoredValue(string key, int value) => PlayerPrefs.SetInt(key, value);
    }
}