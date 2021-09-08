using UnityEngine;

namespace Assets.Sandbox.MouseActions
{
    public class OrbitalMouseController : MouseActionBase
    {
        public float xSpeed = 1.0f;
        public float ySpeed = 1.0f;
        public Transform target;
        public float distance = 400.0f;
        protected float x = 0.0f;
        protected float y = 0.0f;
        private bool resetPosition = false;
        private float _sensitivityFactor = 0.01f;
        private bool setDefultRotation = false;

        // Use this for initialization
        protected override void Start()
        {
            MouseButton = 0;
            base.Start();
            Vector3 angles = transform.eulerAngles;
            if (!setDefultRotation)
            {
                x = angles.y - 180;
                y = -1 * angles.z;
            }
        }

        internal void ResetPosition()
        {
            resetPosition = true;
        }
        protected override void LateUpdate()
        {
            if (resetPosition)
            {
                transform.LookAt(target, transform.up);
            }
            base.LateUpdate();
            if (!IsActive)
            {
                return;
            }
            if (Input.GetMouseButton(MouseButton))
            {
                if (IsMouseInViewArea())
                {
                    if (target)
                    {
                        //  get the distance the mouse moved in the respective direction
                        x += Input.GetAxis("Mouse X") * xSpeed * distance * _sensitivityFactor;
                        y -= Input.GetAxis("Mouse Y") * ySpeed * distance * _sensitivityFactor;
                        // when mouse moves left and right we actually rotate around local y axis	
                        transform.RotateAround(target.position, transform.up, x);
                        // when mouse moves up and down we actually rotate around the local x axis	
                        transform.RotateAround(target.position, transform.right, y);
                        // reset back to 0 so it doesn't continue to rotate while holding the button
                        x = 0;
                        y = 0;
                        //CopySyncDataFromCamera();
                    }
                }
            }
        }

        //public override void CopySyncDataFromCamera()
        //{
        //    var synchronizer = GetSynchronizer();
        //    if (synchronizer != null)
        //    {
        //        synchronizer.Position = transform.position;
        //        synchronizer.Rotation = transform.rotation;
        //    }
        //}

        public void SetDefault()
        {
            setDefultRotation = true;
        }
    }
}
