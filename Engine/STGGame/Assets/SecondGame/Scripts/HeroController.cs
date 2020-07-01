using UnityEngine;

namespace UCGame
{
    public class HeroController : MonoBehaviour
    {
        public new Renderer renderer;
        public Animator animator;
        public new Rigidbody rigidbody;

        public float moveSpeed; //移动速度
        public float runSpeed;  //奔跑速度
        public float jumpForce; //跳跃力

        private Vector3 _curSpeed;
        private bool _jumping;
        private bool _running;

        void Start()
        {
            animator = animator ??  GetComponent<Animator>();
            renderer = renderer ?? GetComponent<Renderer>();
            rigidbody = rigidbody ?? GetComponent<Rigidbody>();
        }

        void Update()
        {
            UpdateState();

            UpdatePosition();
            UpdateScale();

            UpdateRigidbody();
            UpdateRenderer();
            UpdateAniation();

            ClearState();
        }

        void UpdatePosition()
        {
            var newSpeed = _curSpeed * Time.deltaTime;
            transform.Translate(newSpeed);

        }

        void UpdateScale()
        {
            if (_curSpeed.x < 0f)
            {
                var oldLocalScale = transform.localScale;
                oldLocalScale.x = -Mathf.Abs(oldLocalScale.x);
                transform.localScale = oldLocalScale;
            } 
            else if (_curSpeed.x > 0f)
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
                _curSpeed.x = runSpeed * moveDir;
                _running = true;
            }
            else
            {
                _curSpeed.x = moveSpeed * moveDir;
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

        void UpdateRigidbody()
        {
            if (rigidbody == null)
                return;


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

            if (Mathf.Approximately(_curSpeed.x, 0f))
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

