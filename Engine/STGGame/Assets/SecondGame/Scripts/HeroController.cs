using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCGame
{
    public class HeroController : MonoBehaviour
    {
        public new Renderer renderer;
        public Animator animator;

        public float moveSpeed;
        public float runSpeed;

        private float _curSpeed;
        private bool _jumping;
        private bool _running;

        void Start()
        {
            animator = animator ??  GetComponent<Animator>();
            renderer = renderer ?? GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            UpdateState();

            UpdatePosition();
            UpdateScale();
            UpdateRenderer();
            UpdateAniation();

            ClearState();
        }

        void UpdatePosition()
        {
            transform.Translate(new Vector3(_curSpeed * Time.deltaTime, 0, 0));

        }
        void UpdateScale()
        {
            if (_curSpeed < 0f)
            {
                var oldLocalScale = transform.localScale;
                oldLocalScale.x = -Mathf.Abs(oldLocalScale.x);
                transform.localScale = oldLocalScale;
            } 
            else if (_curSpeed > 0f)
            {
                var oldLocalScale = transform.localScale;
                oldLocalScale.x = Mathf.Abs(oldLocalScale.x);
                transform.localScale = oldLocalScale;
            }

        }

        void UpdateState()
        {
            int moveDir = 0;
            if (Input.GetKey(KeyCode.A))
            {
                moveDir = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveDir = 1;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _curSpeed = runSpeed * moveDir;
                _running = true;
            }
            else
            {
                _curSpeed = moveSpeed * moveDir;
                _running = false;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                _jumping = true;
            }
            else
            {
                //如果再次回到平台上,表示跳跃结束
            }

        }
       
        void UpdateRenderer()
        {
            if (renderer == null)
                return;

        }

        void UpdateAniation()
        {
            if (animator == null)
                return;

            if (Mathf.Approximately(_curSpeed, 0f))
            {
                animator.Play("Idle");
            }
            else
            {
                if (_running)
                {
                    animator.Play("Run");
                }
                else
                {
                    animator.Play("Walk");
                }
            }
        }


        void ClearState()
        {

        }
    }

}

