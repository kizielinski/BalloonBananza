using UnityEngine;

public class RectangularCycle : MonoBehaviour
{
    [SerializeField] private float x = 4f;
    [SerializeField] private float y = 2f;
    [SerializeField] private float speedScale = 2f;

    private Rigidbody2D rb;
    private Vector2[] corners;
    private int targetCornerIndex;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 startPos = rb != null ? rb.position : (Vector2)transform.position;

        // Rectangle path anchored at the initial position (bottom-left -> clockwise).
        corners = new Vector2[]
        {
            startPos,
            startPos + new Vector2(x, 0f),
            startPos + new Vector2(x, y),
            startPos + new Vector2(0f, y)
        };

        targetCornerIndex = 1;
    }

    void FixedUpdate()
    {
        if (rb == null || corners == null || corners.Length == 0)
        {
            return;
        }

        int fromIndex = (targetCornerIndex - 1 + corners.Length) % corners.Length;
        Vector2 from = corners[fromIndex];
        Vector2 target = corners[targetCornerIndex];
        Vector2 current = rb.position;
        Vector2 next = current;
        float step = speedScale * Time.fixedDeltaTime;

        // Enforce axis-aligned movement per edge to avoid tiny diagonal drift.
        if (Mathf.Approximately(from.y, target.y))
        {
            next.x = Mathf.MoveTowards(current.x, target.x, step);
            next.y = from.y;
        }
        else
        {
            next.x = from.x;
            next.y = Mathf.MoveTowards(current.y, target.y, step);
        }

        rb.MovePosition(next);

        const float reachEpsilon = 0.01f;
        if ((target - next).sqrMagnitude <= reachEpsilon * reachEpsilon)
        {
            rb.MovePosition(target);
            targetCornerIndex = (targetCornerIndex + 1) % corners.Length;
        }
    }
}
