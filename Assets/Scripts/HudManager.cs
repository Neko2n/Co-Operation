using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    [field: SerializeField] public GameManager Game { get; private set; }

    [Header("Widgets")]
    [SerializeField]
    private TextMeshProUGUI timer;

    void Update()
    {
        this.timer.SetText("{}", math.ceil(this.Game.SecondsRemaining));
    }
}
