using UnityEngine;
using System.Collections;

public static class CoroutineUtils
    {
        public static IEnumerator WaitWhileUnpaused(float seconds)
        {
            float elapsed = 0f;

            while (elapsed < seconds)
            {
                if (!PauseManager.isGameLogicPaused)
                    elapsed += Time.unscaledDeltaTime;

                yield return null;
            }
        }
    }


