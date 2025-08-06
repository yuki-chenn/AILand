using System.Collections;
using AILand.GamePlay.World;
using UnityEngine;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace AILand.UI
{
    public class ElementEnergyUI : MonoBehaviour
    {
        private Text[] m_txtElementCount;
        private Text[] m_txtElementAddCount;


        private void Awake()
        {
            m_txtElementCount = new Text[5];
            m_txtElementAddCount = new Text[5];
            for (int i = 0; i < 5; i++)
            {
                m_txtElementCount[i] = transform.GetChild(i).Find("ElementCount").GetComponent<Text>();
                m_txtElementAddCount[i] = transform.GetChild(i).Find("ElementAddCount").GetComponent<Text>();
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
            for(int i = 0; i < 5; ++i)
            {
                if(delta[i] == 0)
                {
                    m_txtElementAddCount[i].text = "";
                }
                else
                {
                    m_txtElementAddCount[i].text = (delta[i] > 0 ? "+" : "-") + delta[i].ToString();
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
    }
}
