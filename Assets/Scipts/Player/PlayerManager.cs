using System.Collections;
using UnityEngine;

namespace Platformer
{
    public class PlayerManager : MonoBehaviour, IDamagable, IHealable
    {
        [Header("Health")]
        [SerializeField] private int maxHealth = 5;
        [SerializeField] private int health;
        [SerializeField] public bool PlayerIsDead { get; private set; }

        [Header("I-Frames")]
        [SerializeField] private float iFrameDuration = 1;
        [SerializeField] bool invulnerable = false;
        [Space]
        [SerializeField] AudioClip hurtSound;

        PlayerController2D playerController2D;

        new Renderer renderer;

        private void Awake()
        {
            renderer = GetComponentInChildren<Renderer>();
            playerController2D = GetComponent<PlayerController2D>();
        }

        private void Start()
        {
            if (health == 0)
                health = maxHealth;

            if (UI_Manager.Instance != null)
                UI_Manager.Instance.CreateHealthbar(health, maxHealth);
        }

        public void Damage(DamageInfo damageinfo)
        {
            if (damageinfo.ignoreIFrames) invulnerable = false;

            if (invulnerable)
                return;

            if (damageinfo.damageAmount > 0)
            {
                GetComponent<AudioSource>().pitch = 1.27f;
                GetComponent<AudioSource>().PlayOneShot(hurtSound, 0.5f);
            }

            Debug.Log("Taken Damage " + damageinfo.damageAmount);
            health -= damageinfo.damageAmount;
            StartCoroutine(TriggerInvulnerability());
            UpdateHealth();

            if (damageinfo.knockback != Vector2.zero)
                ApplyKnockback(damageinfo.knockback);
        }

        private void ApplyKnockback(Vector2 knockback) => playerController2D.AddForce(knockback);

        public void Heal(int health)
        {
            this.health += health;
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            health = Mathf.Clamp(health, 0, maxHealth);

            if (UI_Manager.Instance != null)
                UI_Manager.Instance.RefreshHealthbar(health, maxHealth);

            if (health <= 0)
                KillPlayer();
        }

        public int GetHealth() => health;
        public int GetMaxHealth() => maxHealth;

        private void KillPlayer()
        {
            PlayerIsDead = true;
            playerController2D.SetMovementEnabled(false);
            playerController2D.ResetAllForces();
            GetComponent<Rigidbody2D>().simulated = false;
            UI_Manager.Instance.SetUIElementDisplayed(1, true);

            if (transform.GetChild(0).GetComponent<SpriteRenderer>() == true)
                transform.GetChild(0).gameObject.SetActive(false);
        }

        private IEnumerator TriggerInvulnerability()
        {
            renderer.material.SetInt("_IsDithered", 1);
            invulnerable = true;
            yield return new WaitForSeconds(iFrameDuration);
            invulnerable = false;
            renderer.material.SetInt("_IsDithered", 0);
        }

        public void Pause() => UI_Manager.Instance.TogglePause();
    }
}
