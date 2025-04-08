using UnityEngine;
using UnityEngine.UI;

namespace InteractiveFloorProjection
{
    [RequireComponent(typeof(Animator))]
    public class NotificationMessage : MonoBehaviour
    {
        public Text messageText;
        public float appearDuration;
        float timer;
        Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Show()
        {
            timer = 0f;
            animator.SetBool("visible", true);
        }
        public void Hide()
        {
            animator.SetBool("visible", false);
            timer = 0f;
        }
        void Update()
        {
            if (animator.GetBool("visible"))
            {
                timer += Time.deltaTime;
                if (timer >= appearDuration)
                {
                    Hide();
                }
            }
        }
    }
}