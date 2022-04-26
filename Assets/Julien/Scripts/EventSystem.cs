using UnityEngine;
using UnityEngine.Events;

public static class EventSystem
{

    public static event UnityAction OnShootEvent;
    public static void ShootEvent() => OnShootEvent?.Invoke();

}
