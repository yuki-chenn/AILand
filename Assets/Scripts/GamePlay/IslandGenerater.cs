using System.Collections;
using System.Collections.Generic;
using AILand.GamePlay;
using AILand.System.EventSystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

public class IslandGenerater : MonoBehaviour, IInteractable
{
    public void OnFocus()
    {
        
    }

    public void OnLostFocus()
    {
        
    }

    public void Interact()
    {
        Debug.Log("Interact with IslandGenerater");
        EventCenter.Broadcast(EventType.ShowDrawIslandPanelUI);
    }
}
