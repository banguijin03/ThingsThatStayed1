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
        targetInventory = FindAnyObjectByType<Inventory>();
        player = FindAnyObjectByType<PlayerController>();

        if (player != null) playerCollider = player.GetComponent<Collider2D>();
    }

    public void Initialize(ItemContainer data, int count)
    {
        itemData = data;
        amount = count;
    }

    private void Update()
    {
        if (playerCollider == null) return;

        Vector2 playerPosition = playerCollider.bounds.center;

        float distance = Vector2.Distance( transform.position, playerPosition );

        if (reachedPlayer)
        {
            if (isPlayerTouching)
            {
                TryPickup();
                return;
            }
            if (distance > pickupRange)
            {
                reachedPlayer = false;
            }
            return;
        }

        if (distance <= pickupRange)
        {
            isMoving = true;
        }

        if (isMoving)
        {
            if (distance > followLimit)
            {
                isMoving = false;
                return;
            }

            float distanceSpeed = Mathf.Lerp(
                2f,
                moveSpeed,
                1f - Mathf.Clamp01(distance / pickupRange)
            );

            transform.position = Vector2.MoveTowards(
                transform.position,
                playerPosition,
                distanceSpeed * Time.deltaTime
            );
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

        // 다른 아이템 데이터면 합치지 않음
        if (otherItem.itemData != itemData) return;

        // 한쪽만 합치기를 실행
        if (GetInstanceID() > otherItem.GetInstanceID())
            return;

        amount += otherItem.amount;

        Destroy(otherItem.gameObject);
    }
}