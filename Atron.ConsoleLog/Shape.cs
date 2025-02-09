namespace Atron.ConsoleLog
{
    public class Shape2D
    {
        public Shape2D()
        {

        }

        // Marcar com virtual pra fazer override
        public virtual float GetArea()
        {
            return 0.0f;
        }

        public override string ToString() => $"This is object is a '{GetType()}' ";
       
    }

    public class Circle : Shape2D
    {
        public const float PI = 3.14f;

        public Circle(int r)
        {
            radius = r;
        }

        public override float GetArea()
        {
            return (PI * (radius * radius));
        }

        int radius;
    }

    public class Rectangle : Shape2D
    {
        public Rectangle(int w, int h)
        {
            width = w; height = h;
        }

        public Rectangle(int side)
        {
            width = height = side;
        }

        public override float GetArea()
        {
            return width * height;
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Height", "must be >= 0");
                } 

                height = value;
            }
        }

        public int BorderSize { get; set; } = 1;

        int width;
        int height;
    }


    public class Square : Rectangle
    {
        public Square (int side) : base(side, side) { }
    }
}
