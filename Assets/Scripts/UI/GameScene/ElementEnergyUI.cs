using System.Collections;
using AILand.GamePlay;
using AILand.GamePlay.World;
using UnityEngine;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace AILand.UI
{
    public class ElementEnergyUI : MonoBehaviour
    {
        private Transform[] m_transElementEnergy;
        
        private Text[] m_txtElementCount;
        private Text[] m_txtElementAddCount;


        private void Awake()
        {
            m_transElementEnergy = new Transform[5];
            m_txtElementCount = new Text[5];
            m_txtElementAddCount = new Text[5];
            for (int i = 0; i < 5; i++)
            {
                m_transElementEnergy[i] = transform.GetChild(i);
                m_txtElementCount[i] = m_transElementEnergy[i].Find("ElementCount").GetComponent<Text>();
                m_txtElementAddCount[i] = m_transElementEnergy[i].Find("ElementAddCount").GetComponent<Text>();
            }
            ClearHideAddCount();
        }


        public void UpdateEnergy(NormalElement element)
        {
            for(int i = 0; i < 5; ++i)
            {
                m_txtElementCount[i].text = element[i].ToString();
            }
        }

        public void UpdateEnergyAdd(int[] delta)
        {
            // 取消之前的
            CancelInvoke("ClearHideAddCount");
            
            for(int i = 0; i < 5; ++i)
            {
                if(delta[i] == 0)
                {
                    m_txtElementAddCount[i].text = "";
                }
                else
                {
                    m_txtElementAddCount[i].text = (delta[i] > 0 ? "+" : "") + delta[i].ToString();
                }
                
            }
            Invoke("ClearHideAddCount", 1f);
        }

        private void ClearHideAddCount()
        {
            for(int i = 0; i < 5; ++i)
            {
                m_txtElementAddCount[i].text = "";
            }
        }

        public void UpdateSelectElement()
        {
            for(int i = 0; i < 5; ++i)
            {
                m_transElementEnergy[i].GetComponent<Image>().enabled =
                    i == GameManager.Instance.CurSelectedElementIndex;
            }
        }
    }
}
