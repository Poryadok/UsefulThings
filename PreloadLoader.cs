using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace PM.UsefulThings
{
    public class PreloadLoader : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
#if UNITY_EDITOR
            if (!YandexGame.SDKEnabled)
            {
                Debug.Log("Load preload scene");
                SceneManager.LoadScene("Boot");
            }
#endif
        }
    }
}