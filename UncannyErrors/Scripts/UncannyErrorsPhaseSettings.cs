using UnityEngine;

namespace Dan.UncannyErrors.Scripts
{
    public class UncannyErrorsPhaseSettings : ScriptableObject
    {
        [SerializeField] private Phase[] _phases;
        
        internal Phase GetPhaseAtErrorCount()
        {
            var errorCount = UncannyErrorsLogListener.errorList.Count;
            for (int i = 0; i < _phases.Length; i++)
            {
                if (errorCount <= _phases[i].minErrorCount)
                    return _phases[i];
            }

            return _phases[0];
        }
    }
}