namespace PangordsEngine
{
    class Transform : Component
    {
        public Matrix4x4 matrix = new Matrix4x4(1.0f);

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                matrix = Mathf.Translate(matrix, position);
            }
        }
        private Vector3 position;

        public Vector3 EulerAngles
        {
            get
            {
                return eulerAngles;
            }
            set
            {
                eulerAngles = value;
                matrix = Mathf.Rotate(matrix, Mathf.DegreesToRadians(eulerAngles.x), Vector3.Right);
                matrix = Mathf.Rotate(matrix, Mathf.DegreesToRadians(eulerAngles.y), Vector3.Up);
                matrix = Mathf.Rotate(matrix, Mathf.DegreesToRadians(eulerAngles.z), Vector3.Forward);
            }
        }
        private Vector3 eulerAngles;

        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                matrix = Mathf.Scale(matrix, scale);
            }
        }
        private Vector3 scale;

        public Vector3 Front
        {
            get
            {
                Vector3 front;
                front.x = Mathf.Cos(Mathf.DegreesToRadians(EulerAngles.x)) * Mathf.Cos(Mathf.DegreesToRadians(EulerAngles.y));
                front.y = Mathf.Sin(Mathf.DegreesToRadians(EulerAngles.y));
                front.z = Mathf.Sin(Mathf.DegreesToRadians(EulerAngles.x)) * Mathf.Cos(Mathf.DegreesToRadians(EulerAngles.y));
                return Mathf.Normalize(front);
            }
        }
    }
}