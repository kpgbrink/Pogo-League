using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace AutoLevelMenu.Common
{
    public class SceneChanger : MonoBehaviour
    {
        public GameControl gameControl;
        // Feel free to add more load types
        public enum SceneChangeType
        {
            NormalLoad,
            ProgressLoad,
        }
        public GameObject imgGameObject;
        public Image img;
        public GameObject progressSliderGameObject;
        public Slider progressSlider;
        public TextMeshProUGUI progressText;
        public GameObject levelNamePanel;
        public Image levelNamePanelImage;
        public TextMeshProUGUI worldNameText;
        public TextMeshProUGUI levelNameText;
        public AnimationCurve curve;

        public void ProgressLoad()
        {
            LoadNext(SceneChangeType.ProgressLoad);
        }

        public void ProgressLoad(string scenePath)
        {
            Load(scenePath, SceneChangeType.ProgressLoad);
        }

        public void NormalLoad()
        {
            LoadNext(SceneChangeType.NormalLoad);
        }

        public void NormalLoad(string scenePath)
        {
            Load(scenePath, SceneChangeType.NormalLoad);
        }

        public void LoadNext(SceneChangeType sceneChangeType)
        {
            // Get string of next scene then call Change to
            var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                var nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
                ProgressLoad(nextScenePath);
                return;
            }
        }

        public void Load(string scenePath, SceneChangeType sceneChangeType)
        {
            // Set everything to invisible
            levelNamePanel.SetActive(false);
            progressSliderGameObject.SetActive(false);
            imgGameObject.SetActive(false);

            scenePath = Utils.ToForwardSlash(scenePath);

            switch (sceneChangeType)
            {
                case SceneChangeType.NormalLoad:
                    SceneManager.LoadScene(scenePath);
                    break;
                case SceneChangeType.ProgressLoad:
                    levelNamePanel.SetActive(true);

                    var worldColor = gameControl.levelsData.FindWorldData(scenePath);
                    if (worldColor != null)
                    {
                        worldNameText.text = worldColor.worldName;
                        levelNamePanelImage.color = worldColor.buttonColor;
                    }
                    else
                    {
                        Debug.Log("world color is null");
                        worldNameText.text = "";
                    }

                    levelNameText.text = Utils.GetSceneName(scenePath);
                    StartCoroutine(OnProgressLoading(scenePath));
                    break;
            }
        }

        Color ChangeColorAlpha(Color color, float a)
        {
            var tempColor = color;
            tempColor.a = a;
            return tempColor;
        }

        IEnumerator OnProgressLoading(string scene)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(scene);
            // Make things visible
            progressSliderGameObject.SetActive(true);
            imgGameObject.SetActive(true);
            img.color = ChangeColorAlpha(img.color, 1);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                var progress = Mathf.Clamp01(asyncLoad.progress / .9f);

                progressSlider.value = progress;
                progressText.text = progress * 100f + "%";
                yield return null;
            }
        }

        IEnumerator FadeOut(string scene)
        {
            imgGameObject.SetActive(true);
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * .05f;
                var a = curve.Evaluate(t);
                img.color = ChangeColorAlpha(img.color, a);
                yield return null;
            }
            // Load the scene
            SceneManager.LoadScene(scene);
        }

        IEnumerator FadeIn()
        {
            var t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime * 2f;
                var a = curve.Evaluate(t);
                img.color = ChangeColorAlpha(img.color, a);
                yield return 0;
            }
            imgGameObject.SetActive(false);
            levelNamePanel.SetActive(false);
        }
    }
}
