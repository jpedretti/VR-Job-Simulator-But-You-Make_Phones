using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace com.NW84P
{
    public class SceneLoader : MonoBehaviour
    {
        private const float _SCENE_LOAD_FADE_DURATION = 0.3f;

        [SerializeField]
        protected Image _fadeSceneLoadImage;

        protected IEnumerator FadeOut()
        {
            _fadeSceneLoadImage.transform.parent.gameObject.SetActive(true);
            var fadeImageColor = _fadeSceneLoadImage.color;
            fadeImageColor.a = 1;
            _fadeSceneLoadImage.color = fadeImageColor;
            var fadeTimer = 0f;
            yield return null;

            while (_fadeSceneLoadImage.color.a > 0)
            {
                fadeTimer += Time.deltaTime;
                fadeImageColor.a = 1 - (fadeTimer / _SCENE_LOAD_FADE_DURATION);
                _fadeSceneLoadImage.color = fadeImageColor;
                yield return null;
            }

            _fadeSceneLoadImage.transform.parent.gameObject.SetActive(false);
        }

        protected IEnumerator FadeAndLoadScene(int sceneIndex)
        {
            _fadeSceneLoadImage.transform.parent.gameObject.SetActive(true);
            var fadeImageColor = _fadeSceneLoadImage.color;
            var fadeTimer = 0f;
            yield return null;

            while (_fadeSceneLoadImage.color.a < 1)
            {
                fadeTimer += Time.deltaTime;
                fadeImageColor.a = fadeTimer / _SCENE_LOAD_FADE_DURATION;
                _fadeSceneLoadImage.color = fadeImageColor;
                yield return null;
            }

            // load scene
            SceneManager.LoadScene(sceneIndex);
        }

#if UNITY_EDITOR

        public virtual void OnValidate() => Debug.Assert(_fadeSceneLoadImage != null, "Fade Scene Load Image is null");

#endif
    }
}
