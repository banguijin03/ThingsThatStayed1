using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] float pickupRange = 1.5f;
    [SerializeField] float followLimit = 5f;
    [SerializeField] float moveSpeed = 8f;

    private ItemContainer itemData;
    private int amount;

    private Inventory targetInventory;
    private PlayerController player;
    private Collider2D playerCollider;

    private bool isMoving;
    private bool reachedPlayer;
    private bool isPlayerTouching;

    private void Awake()
    {
        ConnectPlayer();
    }

    private void ConnectPlayer()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.Player == null) return;

        player = GameManager.Instance.Player.GetComponent<PlayerController>();

        if (player == null) return;

        playerCollider = player.GetComponent<Collider2D>();
        targetInventory = player.GetComponent<Inventory>();
    }

    public void Initialize(ItemContainer data, int count)
    {
        itemData = data;
        amount = count;
    }

    private void Update()
    {
        if (player == null || targetInventory == null || playerCollider == null)
        {
            ConnectPlayer();

            if (player == null || targetInventory == null || playerCollider == null) return;
        }

        Vector2 playerPosition = playerCollider.bounds.center;
        float distance = Vector2.Distance(transform.position, playerPosition);

        if (reachedPlayer)
        {
            if (isPlayerTouching)
            {
                TryPickup();
                return;
            }

            if (distance > pickupRange)
                reachedPlayer = false;

            return;
        }

        if (distance <= pickupRange) isMoving = true;

        if (isMoving)
        {
            if (distance > followLimit)
            {
                isMoving = false;
                return;
            }

            float distanceSpeed = Mathf.Lerp( 2f, moveSpeed, 1f - Mathf.Clamp01(distance / pickupRange));

            transform.position = Vector2.MoveTowards(transform.position, playerPosition, distanceSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out WorldItem otherItem))
        {
            MergeItem(otherItem);
            return;
        }

        if (other != playerCollider) return;

        isPlayerTouching = true;
        TryPickup();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != playerCollider) return;

        isPlayerTouching = false;
    }

    private void TryPickup()
    {
        if (targetInventory == null) return;
        if (itemData == null) return;
        if (amount <= 0) return;

        int remaining = targetInventory.AddItem(itemData, amount);

        if (remaining <= 0)
        {
            Destroy(gameObject);
            return;
        }

        amount = remaining;
        isMoving = false;
        reachedPlayer = true;
    }

    private void MergeItem(WorldItem otherItem)
    {
        if (otherItem == null) return;
        if (otherItem == this) return;
        if (otherItem.itemData != itemData) return;

        if (GetInstanceID() > otherItem.GetInstanceID()) return;

        amount += otherItem.amount;
        Destroy(otherItem.gameObject);
    }
}