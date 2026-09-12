using UnityEngine;
using Xprees.Variables.Base;

namespace Xprees.Variables.Persistent
{
    [CreateAssetMenu(menuName = "Variables/Persistent/Bool", fileName = "Bool_PersistVar")]
    public class PersistentBoolVariable : PersistentVariableBaseSO<bool>
    {
        protected override bool ReadStoredValue(string key, bool fallback) => PlayerPrefs.GetInt(key, fallback ? 1 : 0) != 0;

        protected override void WriteStoredValue(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}