using UnityEngine;
using UnityEngine.UI;
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
    public LayerMask placementLayer;

    [Header("Hand/Equip Positions")]
    public Transform torchHandTransform;
    public Transform pickaxeHandTransform;
    public Transform crystalHandTransform;

    [Header("UI")]
    public TextMeshProUGUI pickupMessage;
    private GameObject itemInView;

    [Header("Dialogue References")]
    public Dialogue torchDialogue;

    [Header("Crystal Replacement")]
    public GameObject crystalPrefab;
    private bool crystalEquipped = false;
    private GameObject currentCrystalInstance;

    void Update()
    {
        CheckForPickupItem();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemInView != null)
                TryPickupItem(itemInView);
            else if (crystalEquipped)
                PlaceCrystalAtTarget();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchItem(1);

        HandleEquippedItemInput();
    }

    void CheckForPickupItem()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
        {
            GameObject hitItem = hit.collider.gameObject;
            itemInView = hitItem;

            if (pickupMessage != null)
            {
                pickupMessage.text = "Press E to equip " + itemInView.name;
                pickupMessage.enabled = true;
            }
        }
        else if (Physics.Raycast(ray, out hit, pickupRange, placementLayer) && crystalEquipped)
        {
            itemInView = null;
            if (pickupMessage != null)
            {
                pickupMessage.text = "Press E to place crystal";
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

                if (item.GetComponent<Collider>()) item.GetComponent<Collider>().enabled = false;
                if (item.GetComponent<Rigidbody>()) item.GetComponent<Rigidbody>().isKinematic = true;

                RotateObject rot = item.GetComponent<RotateObject>();
                if (rot != null) rot.StopRotation();

                if (item.CompareTag("Torch")) item.transform.SetParent(torchHandTransform);
                else if (item.CompareTag("Pickaxe")) item.transform.SetParent(pickaxeHandTransform);
                else item.transform.SetParent(torchHandTransform);

                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;

                item.SetActive(false);

                if (pickupMessage != null) pickupMessage.enabled = false;

                Debug.Log(item.name + " picked up into slot " + (i + 1));

                if (item.CompareTag("Torch"))
                {
                    ObjectiveManager.Instance?.SetObjective("OBJECTIVE: Talk to the SUN LORD Statue");
                    if (torchDialogue != null)
                    {
                        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
                        dialogueManager?.StartDialogue(torchDialogue);
                    }
                }

                if (item.CompareTag("Pickaxe"))
                {
                    ObjectiveManager.Instance?.SetObjective("OBJECTIVE: Destroy all 7 demon crystals");
                }

                return;
            }
        }

        Debug.Log("Inventory full!");
    }

    void SwitchItem(int slotIndex)
    {
        if (currentSlot >= 0)
        {
            GameObject currentItem = (currentSlot == 1 && crystalEquipped) ? currentCrystalInstance : inventorySlots[currentSlot];
            if (currentItem != null) currentItem.SetActive(false);
        }

        currentSlot = slotIndex;

        GameObject itemToEquip = null;

        if (currentSlot == 1)
        {
            if (crystalEquipped && currentCrystalInstance != null)
                itemToEquip = currentCrystalInstance;
            else if (inventorySlots[1] != null)
            {
                itemToEquip = inventorySlots[1];
                itemToEquip.transform.SetParent(pickaxeHandTransform);
                itemToEquip.transform.localPosition = Vector3.zero;
                itemToEquip.transform.localRotation = Quaternion.identity;
            }
        }
        else if (inventorySlots[currentSlot] != null)
        {
            itemToEquip = inventorySlots[currentSlot];
        }

        if (itemToEquip != null)
            itemToEquip.SetActive(true);
    }

    void HandleEquippedItemInput()
    {
        if (currentSlot < 0) return;

        GameObject equippedItem = (currentSlot == 1 && crystalEquipped) ? currentCrystalInstance : inventorySlots[currentSlot];
        if (equippedItem == null) return;

        TorchController torch = equippedItem.GetComponent<TorchController>();
        if (torch != null && Input.GetMouseButtonDown(1))
        {
            torch.ToggleTorch();
        }
    }

    public GameObject GetEquippedItem()
    {
        if (currentSlot < 0) return null;
        if (currentSlot == 1)
        {
            return crystalEquipped ? currentCrystalInstance : inventorySlots[1];
        }
        return inventorySlots[currentSlot];
    }

    public void EquipCrystal()
    {
        if (crystalEquipped || crystalPrefab == null || inventorySlots[1] == null) return;

        var renderers = inventorySlots[1].GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
            r.enabled = false;

        currentCrystalInstance = Instantiate(crystalPrefab, crystalHandTransform);
        currentCrystalInstance.transform.localPosition = Vector3.zero;
        currentCrystalInstance.transform.localRotation = Quaternion.identity;
        currentCrystalInstance.SetActive(true);

        crystalEquipped = true;
        currentSlot = 1;

        ObjectiveManager.Instance?.SetObjective("OBJECTIVE: Place the crystal's heart in HOLY FIRE");
    }

    private void PlaceCrystalAtTarget()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, placementLayer))
        {
            CrystalPlacement placement = hit.collider.GetComponent<CrystalPlacement>();
            if (placement != null)
            {
                bool placed = placement.PlaceCrystal(currentCrystalInstance);
                if (placed)
                {
                    crystalEquipped = false;
                    currentCrystalInstance = null;

                    if (inventorySlots[1] != null)
                    {
                        var renderers = inventorySlots[1].GetComponentsInChildren<Renderer>(true);
                        foreach (var r in renderers)
                            r.enabled = true;
                    }

                    currentSlot = 1;
                }
            }
        }
    }
}
