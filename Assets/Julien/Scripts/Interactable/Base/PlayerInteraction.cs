using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField] private float interactionDistance;
    [SerializeField] private KeyCode interactionKeyCode = KeyCode.E;
    [SerializeField] private GameObject canvas;

    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private Camera cam;

    private void Start()
    {
        if (IsLocalPlayer) canvas.SetActive(true);
    }

    void Update()
    {
        if (!IsLocalPlayer) return;
        
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        bool didHit = false;
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit, interactionDistance))
        {
            if (raycastHit.collider.TryGetComponent(out Interactable interactable))
            {
                HandleInteraction(interactable);
                didHit = true;
                interactionText.text = interactable.GetDescription();
            }
        }

        if (!didHit)
        {
            interactionText.text = "";
        }
    }

    void HandleInteraction(Interactable interactable)
    {
        switch (interactable.interactionType)
        {
            case Interactable.InteractionType.Click:
                if (Input.GetKeyDown(interactionKeyCode)) interactable.Interact(this);
                break;
            case Interactable.InteractionType.Hold:
                if (Input.GetKey(interactionKeyCode))
                {
                    interactable.IncreaseHoldTime();
                    if (interactable.GetCurrentHoldTime() > interactable.GetRequiredHoldTime())
                    {
                        interactable.Interact(this);
                        interactable.ResetHoldTime();
                    }
                }
                else
                {
                    interactable.ResetHoldTime();
                }
                break;
            default:
                throw new Exception("Interaction inconnue !");
        }
    }
}