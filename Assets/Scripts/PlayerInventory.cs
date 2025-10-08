using System.Collections;
using UnityEngine;
using TMPro;
using Sun_Temple;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Slots")]
    public GameObject[] inventorySlots = new GameObject[2];
    private int currentSlot = -1;

    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public LayerMask pickupLayer;

    [Header("Hand/Equip Positions")]
    public Transform torchHandTransform;
    public Transform pickaxeHandTransform;

    [Header("UI")]
    public TextMeshProUGUI pickupMessage;
    private GameObject itemInView;

    [Header("Dialogue References")]
    public Dialogue torchDialogue;

    [Header("Torch Settings")]
    public float torchToggleCooldown = 0.3f;
    private bool canToggleTorch = true;

    void Update()
    {
        CheckForPickupItem();

        if (Input.GetKeyDown(KeyCode.E) && itemInView != null)
            TryPickupItem(itemInView);

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchItem(1);

        HandleEquippedItemInput();
    }

    void CheckForPickupItem()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            GameObject hitItem = hit.collider.gameObject;
            if (hitItem == GetEquippedItem())
            {
                itemInView = null;
                if (pickupMessage != null) pickupMessage.enabled = false;
                return;
            }

            itemInView = hitItem;
            if (pickupMessage != null)
            {
                pickupMessage.text = "Press E to equip " + itemInView.name;
                pickupMessage.enabled = true;
            }
        }
        else
        {
            itemInView = null;
            if (pickupMessage != null) pickupMessage.enabled = false;
        }
    }

    void TryPickupItem(GameObject item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = item;

                if (!item.CompareTag("Pickaxe"))
                {
                    if (item.GetComponent<Collider>())
                        item.GetComponent<Collider>().enabled = false;
                    if (item.GetComponent<Rigidbody>())
                        item.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    Rigidbody rb = item.GetComponent<Rigidbody>();
                    if (rb != null) rb.isKinematic = true;
                }

                RotateObject rot = item.GetComponent<RotateObject>();
                if (rot != null) rot.StopRotation();

                if (item.CompareTag("Torch")) item.transform.SetParent(torchHandTransform);
                else if (item.CompareTag("Pickaxe")) item.transform.SetParent(pickaxeHandTransform);

                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                item.SetActive(false);

                if (pickupMessage != null) pickupMessage.enabled = false;

                if (item.CompareTag("Torch"))
                {
                    ObjectiveManager.Instance?.SetObjective("OBJECTIVE: Talk to the SUN LORD Statue");
                    DialogueManager dm = FindObjectOfType<DialogueManager>();
                    dm?.StartDialogue(torchDialogue);
                }
                else if (item.CompareTag("Pickaxe"))
                {
                    ObjectiveManager.Instance?.SetObjective("OBJECTIVE: Destroy all 7 demon crystals");
                }

                return;
            }
        }
    }

    void SwitchItem(int slotIndex)
    {
        if (currentSlot >= 0 && inventorySlots[currentSlot] != null)
            inventorySlots[currentSlot].SetActive(false);

        if (inventorySlots[slotIndex] == null)
        {
            currentSlot = -1;
            return;
        }

        currentSlot = slotIndex;
        inventorySlots[currentSlot].SetActive(true);

        PickaxeController pickaxe = inventorySlots[currentSlot].GetComponent<PickaxeController>();
        if (pickaxe != null && !pickaxe.IsSwinging)
            pickaxe.OnEquip();
    }

    void HandleEquippedItemInput()
    {
        if (currentSlot < 0) return;

        GameObject equippedItem = inventorySlots[currentSlot];
        if (equippedItem == null) return;

        TorchController torch = equippedItem.GetComponent<TorchController>();
        if (torch != null && Input.GetMouseButtonDown(1) && canToggleTorch)
        {
            torch.ToggleTorch();
            StartCoroutine(TorchCooldown());
        }

        if (equippedItem.CompareTag("Pickaxe"))
        {
            PickaxeController pickaxe = equippedItem.GetComponent<PickaxeController>();
            if (pickaxe != null && Input.GetMouseButtonDown(0) && !pickaxe.IsSwinging)
                StartCoroutine(pickaxe.SwingAttack());
        }
    }

    public GameObject GetEquippedItem()
    {
        if (currentSlot < 0) return null;
        return inventorySlots[currentSlot];
    }

    private IEnumerator TorchCooldown()
    {
        canToggleTorch = false;
        yield return new WaitForSeconds(torchToggleCooldown);
        canToggleTorch = true;
    }
}
