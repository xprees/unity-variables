using UnityEngine;
using Xprees.Variables.Base;

namespace Xprees.Variables.Persistent
{
    [CreateAssetMenu(menuName = "Variables/Persistent/Float", fileName = "Float_PersistVar")]
    public class PersistentFloatVariable : PersistentVariableBaseSO<float>
    {
        protected override float ReadStoredValue(string key, float fallback) => PlayerPrefs.GetFloat(key, fallback);

        protected override void WriteStoredValue(string key, float value) => PlayerPrefs.SetFloat(key, value);
    }
}