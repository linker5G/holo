﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

using HoloToolkit.Examples.SharingWithUNET;
public class SessionListButton : MonoBehaviour, IInputClickHandler {

    public NetworkDiscoveryWithAnchors.SessionInfo SessionInfo { get; private set; }
    int textColorId;
    TextMesh textMesh;
    Material textMaterial;
    ScrollingSessionListUIController scrollingUIController;                                  

    void OnEnable()
    {
        textMesh = gameObject.GetComponentInChildren<TextMesh>();
        textMaterial = textMesh.GetComponent<MeshRenderer>().material;
        textColorId = Shader.PropertyToID("_Color");
        scrollingUIController = ScrollingSessionListUIController.Instance;
        if (scrollingUIController == null)
            Debug.Log("sad");
    }

    public void SetSessionInfo(NetworkDiscoveryWithAnchors.SessionInfo sessionInfo )
    {
        SessionInfo = sessionInfo;
        if (SessionInfo != null)
        {
            textMesh.text = string.Format("{0}\n{1}", SessionInfo.SessionName, SessionInfo.SessionIp);
            if (SessionInfo == scrollingUIController.SelectedSession)
            {
                textMaterial.SetColor(textColorId, new Color(0f, 0.90f, 0.88f));

                textMesh.color = new Color(0f, 0.90f, 0.88f);
            }
            else
            {
                textMaterial.SetColor(textColorId, Color.white);
                textMesh.color = Color.white;
            }
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        scrollingUIController.SetSelectedSession(SessionInfo);
    }
}
