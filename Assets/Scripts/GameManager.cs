#nullable enable
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class GameManager : MonoBehaviour
{
    private const float DAMAGE_FORGIVENESS_RADIUS = 0.25f;

    public List<ShapeBehaviour> shapes = new();

    [Tooltip("The sprite to draw lines between shape nodes with.")]
    public SpriteRenderer? brush = null;

    [Tooltip("The number of seconds before you lose the game.")]
    [field: SerializeField] public float TotalSeconds { get; private set; }

    public float SecondsRemaining { get; private set; }

    public bool GameStarted { get; private set; }

    private readonly List<SpriteRenderer> strokes = new();

    private readonly HashSet<ShapeBehaviour> completedShapes = new();

    // Used to "forgive" bad inputs which were too close to the previous bad input
    private Vector2? forgivePosition = null;

    void Start()
    {
        this.SecondsRemaining = this.TotalSeconds;
        this.GameStarted = true;
    }

    void Update()
    {
        if (!this.GameStarted)
            return;

        if (this.shapes.Count == this.completedShapes.Count)
        {
            this.EndGame(true);
            return;
        }

        this.SecondsRemaining -= Time.deltaTime;
        if (this.SecondsRemaining <= 0)
        {
            this.Rewind();
            return;
        }

        Mouse? mouse = Mouse.current;
        if (mouse == null)
            return;
        if (mouse.leftButton.isPressed)
        {
            this.DrawAt(Camera.main.ScreenToWorldPoint(mouse.position.ReadValue()));
        }
        else
        {
            this.forgivePosition = null;
        }
        this.UpdateGhosts(mouse);

        // Debug the timer
        if (mouse.rightButton.isPressed)
        {
            Debug.Log("Seconds remaining: " + this.SecondsRemaining);
        }
    }

    private void DrawAt(in Vector2 position)
    {
        bool success = false;
        foreach (ShapeBehaviour shape in this.shapes)
        {
            if (shape.IsCompleted)
                continue;
            ShapeBehaviour.ShapeNode? node = shape.Click(position);
            if (node == null)
                continue;
            success = true;
            if (shape.IsCompleted)
            {
                this.completedShapes.Add(shape);
                this.forgivePosition = position;
            }
            if (this.brush != null)
            {
                SpriteRenderer stroke = GameObject.Instantiate<SpriteRenderer>(this.brush);
                stroke.transform.position = node.TrySnapToSegment(position) ?? position;
                this.strokes.Add(stroke);
            }
        }
        if (!success)
        {
            // Take damage or something idk yet
            bool takeDamage = true;
            if (this.forgivePosition != null)
            {
                float distance = ((Vector2)this.forgivePosition - position).sqrMagnitude;
                if (distance <= DAMAGE_FORGIVENESS_RADIUS * DAMAGE_FORGIVENESS_RADIUS)
                {
                    takeDamage = false;
                }
            }
            if (takeDamage)
            {
                this.forgivePosition = position;
                Debug.Log("Ouch!");
            }
        }
    }

    private void ClearStrokes()
    {
        foreach (SpriteRenderer stroke in this.strokes)
        {
            GameObject.Destroy(stroke);
        }
        this.strokes.Clear();
    }

    public void EndGame(in bool win)
    {
        this.GameStarted = false;
        this.ClearStrokes();
        Debug.Log("Game " + (win ? "won" : "lost"));
        this.GameEnded?.Invoke(win);
    }
}
