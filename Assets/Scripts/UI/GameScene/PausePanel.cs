using AILand.GamePlay;
using AILand.System.EventSystem;
using UnityEngine.UI;

namespace AILand.UI
{
    public class PausePanel : BaseUIPanel
    {
        private Button m_btnClose;
        private Button m_btnBackMenu;
        private Button m_btnCloseBg;
        

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        protected override void BindUI()
        {
            m_btnClose = transform.Find("BtnClose").GetComponent<Button>();
            m_btnBackMenu = transform.Find("BtnBackMenu").GetComponent<Button>();
            m_btnCloseBg = transform.Find("BtnCloseBg").GetComponent<Button>();
            
            m_btnClose.onClick.AddListener(Hide);
            m_btnCloseBg.onClick.AddListener(Hide);
        }
        
        protected override void BindListeners()
        {
            EventCenter.AddListener(EventType.PauseGame, Show);
        }

        protected override void UnbindListeners()
        {
            EventCenter.RemoveListener(EventType.PauseGame, Show);
        }

        protected override void Hide()
        {
            base.Hide();
            GameManager.Instance.ResumeGame();
        }
    }

}