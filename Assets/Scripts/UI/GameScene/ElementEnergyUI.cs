using AILand.GamePlay.World;
using UnityEngine;
using UnityEngine.UI;

namespace AILand.UI
{
    public class ElementEnergyUI : MonoBehaviour
    {
        private Text[] m_txtElementCount;


        private void Awake()
        {
            m_txtElementCount = new Text[5];
            for (int i = 0; i < 5; i++)
            {
                m_txtElementCount[i] = transform.GetChild(i).Find("ElementCount").GetComponent<Text>();
            }
        }


        public void UpdateEnergy(NormalElement element)
        {
            for(int i = 0; i < 5; ++i)
            {
                m_txtElementCount[i].text = element[i].ToString();
            }
        }

    }
}
