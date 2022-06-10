using System;

namespace PangordsEngine
{
    // Defines several possible options for camera movement. Used as abstraction to stay away from window-system specific input methods
    public enum CameraMovement
    {
        Forward,
        Backward,
        Left,
        Right
    };

    class Camera
    {
        // Default camera values
        public float YAW = -90.0f;
        public float PITCH = 0.0f;
        public float SPEED = 2.5f;
        public float SENSITIVITY = 0.1f;
        public float FIELDOFVIEW = 45.0f;

        public Transform transform = new Transform();

        // camera Attributes
        public Vector3 Position;
        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;
        public Vector3 WorldUp;
        // camera options
        public float MovementSpeed;
        public float MouseSensitivity;
        public float FieldOfView;

        public Camera(Vector3 position)
        {
            transform.position = position;
            transform.eulerAngles = new Vector3(YAW, PITCH, transform.eulerAngles.z);

            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVITY;
            FieldOfView = FIELDOFVIEW;

            Front = Vector3.Forward;

            UpdateCameraVectors();
        }

        // returns the view matrix calculated using Euler Angles and the LookAt Matrix
        public Matrix4x4 GetViewMatrix()
        {
            return Mathf.LookAt(transform.position, transform.position + Front, Up);
        }

        // processes input received from any keyboard-like input system. Accepts input parameter in the form of camera defined ENUM (to abstract it from windowing systems)
        public void ProcessKeyboard(CameraMovement direction, float deltaTime)
        {
            float velocity = MovementSpeed * deltaTime;
            if (direction == CameraMovement.Forward)
                transform.position += Front * velocity;
            if (direction == CameraMovement.Backward)
                transform.position -= Front * velocity;
            if (direction == CameraMovement.Left)
                transform.position -= Right * velocity;
            if (direction == CameraMovement.Right)
                transform.position += Right * velocity;
        }

        // processes input received from a mouse input system. Expects the offset value in both the x and y direction.
        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            transform.eulerAngles.x += xoffset;
            transform.eulerAngles.y += yoffset;

            // make sure that when pitch is out of bounds, screen doesn't get flipped
            if (constrainPitch)
            {
                if (transform.eulerAngles.y > 89.0f)
                    transform.eulerAngles.y = 89.0f;
                if (transform.eulerAngles.y < -89.0f)
                    transform.eulerAngles.y = -89.0f;
            }

            // update Front, Right and Up Vectors using the updated Euler angles
            UpdateCameraVectors();
        }

        // processes input received from a mouse scroll-wheel event. Only requires input on the vertical wheel-axis
        public void ProcessMouseScroll(float yoffset)
        {
            FieldOfView -= yoffset;
            if (FieldOfView < 1.0f)
                FieldOfView = 1.0f;
            if (FieldOfView > 45.0f)
                FieldOfView = 45.0f;
        }

        // calculates the front vector from the Camera's (updated) Euler Angles
        public void UpdateCameraVectors()
        {
            // calculate the new Front vector
            Vector3 front;
            front.x = Mathf.Cos(Mathf.DegreesToRadians(transform.eulerAngles.x)) * Mathf.Cos(Mathf.DegreesToRadians(transform.eulerAngles.y));
            front.y = Mathf.Sin(Mathf.DegreesToRadians(transform.eulerAngles.y));
            front.z = Mathf.Sin(Mathf.DegreesToRadians(transform.eulerAngles.x)) * Mathf.Cos(Mathf.DegreesToRadians(transform.eulerAngles.y));
            Front = Mathf.Normalize(front);
            // also re-calculate the Right and Up vector
            Right = Mathf.Normalize(Mathf.Cross(Front, Vector3.Up));  // normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            Up = Mathf.Normalize(Mathf.Cross(Right, Front));
        }
    }
}
