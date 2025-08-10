using System;
using System.Collections;
using System.Collections.Generic;
using AILand.System.EventSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.UI
{
    public class SceneLoadPanel : BaseUIPanel
    {
        private Text m_txtLoadingPercent;
        private Text m_txtLoading;
        private Slider m_sliderLoadingBar;

        protected override void Awake()
        {
            base.Awake();
            Hide();
        }

        protected override void Start()
        {
            m_txtLoading.text = "Loading...";
        }

        private void StartGame()
        {
            Show();
            StartCoroutine(LoadingScene("GameScene"));
        }
        
        IEnumerator LoadingScene(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            if (op != null)
            {
                op.allowSceneActivation = false;
                while (!op.isDone)
                {
                    m_sliderLoadingBar.value = op.progress;
                    m_txtLoadingPercent.text = $"{(op.progress * 100):F0}%";

                    if (op.progress >= 0.9f)
                    {
                        m_sliderLoadingBar.value = 1;
                        m_txtLoadingPercent.text = "100%";

                        m_txtLoading.text = "Press any key to start ...";
                        if (Input.anyKeyDown)
                        {
                            op.allowSceneActivation = true;
                        }
                    }

                    yield return null;
                }
            }
        }

        
        protected override void BindUI()
        {
            m_txtLoadingPercent = transform.Find("Loading/TxtLoadingPercent").GetComponent<Text>();
            m_txtLoading = transform.Find("Loading/TxtLoading").GetComponent<Text>();
            m_sliderLoadingBar = transform.Find("Loading/SliderLoadingBar").GetComponent<Slider>();
        }
        
        protected override void BindListeners()
        {
            EventCenter.AddListener(EventType.StartGame,StartGame);
        }
        
        protected override void UnbindListeners()
        {
            EventCenter.RemoveListener(EventType.StartGame, StartGame);
        }
        
    }
}
