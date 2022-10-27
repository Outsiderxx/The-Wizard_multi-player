using UnityEngine;

public class CharacterEventHandler : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsItem;

    private CharacterState state;
    private CharacterEffect effect;

    public System.Action<Item> OnObtainItem;
    public System.Action OnTriggerPortal;

    private void Awake()
    {
        this.state = this.GetComponentInParent<CharacterState>();
        this.effect = this.GetComponentInParent<CharacterEffect>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 12)
        {
            this.OnObtainItem.Invoke(other.GetComponent<Item>());
        }

        if (other.gameObject.tag == "Portal")
        {
            this.OnTriggerPortal.Invoke();
            Destroy(other.gameObject);
        }
    }
}
