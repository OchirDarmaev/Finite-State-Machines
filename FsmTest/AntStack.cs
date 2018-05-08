using System;
using System.Drawing;

namespace FsmTest
{
    public class AntStack
    {
        private readonly int MouseThreatRadius = 5;
        private readonly int MaxVelocity = 1;
        private readonly int Distanse = 1;
        private int _width;
        private int _heigth;

        public FsmStack Brain { get; private set; }
        public Point Position { get; private set; }
        public Point Velocity { get; private set; }
        public Point Leaf { get; private set; }
        public Point Home { get; private set; }
        public float Rotation { get; private set; }
        public Point Mouse { get;  set; }
        public Action Paint ;

        public AntStack(int width, int heigth, Action paint)
        {
            _width = width;
            _heigth = heigth;
            var rand = new Random();
            Position = new Point(rand.Next(width), rand.Next(heigth));
            Leaf =  new Point(rand.Next(width), rand.Next(heigth));
            Home =  new Point(rand.Next(width), rand.Next(heigth));
            Mouse = new Point(-width, -heigth);
            Velocity = new Point(-width, -heigth);
            Brain = new FsmStack();
            Paint = paint;
            Brain.PushState(FindLeaf);
            Paint.Invoke();
        }

        /// <summary>
        /// Состояние "findLeaf".
        /// Заставляет муравья искать листья.
        /// </summary>
        public void FindLeaf()
        {
            Velocity = new Point(Leaf.X - Position.X, Leaf.Y - Position.Y);

            // Муравей тольо что подобрал листок
            if (Distance(Leaf, Position) <= Distanse)
            {
                var r = new Random();
                Leaf = new Point(r.Next(_width), r.Next(_heigth));
                Brain.PopState();
                Brain.PushState(GoHome);
            }
            if (Distance(Mouse, Position) < MouseThreatRadius)
            {
                Brain.PushState(RunAway);
            }
        }

        /// <summary>
        ///  Состояние "goHome".
        /// Заставляет муравья идти в муравейник.
        /// </summary>
        public void GoHome()
        {
            Velocity = new Point(Home.X - Position.X, Home.Y - Position.Y);

            if (Distance(Home, Position) <= Distanse)
            {
                Brain.PopState();
                // Муравей уже дома.Пора искать новый лист.
                Brain.PushState(FindLeaf);
            }
            if (Distance(Mouse, Position) < MouseThreatRadius)
            {
                Brain.PushState(RunAway);
            }
        }

        /// <summary>
        /// Состояние "runAway".
        /// Заставляет муравья убегать от курсора мыши.
        /// </summary>
        public void RunAway()
        {
            // Перемещаем муравья подальше от курсора
            Velocity = new Point(Position.X - Mouse.X, Position.Y - Mouse.Y);

            // Если курсор отстался рядом
            if (Distance(Mouse, Position) > MouseThreatRadius)
            {
                Brain.PopState();
            }
        }

        /// <summary>
        /// Обновление конечного автомата. Эта функция будет вызывать
        /// функцию активного состояния: findLeaf(), goHome() или runAway().
        /// </summary>
        public void Update()
        {
            Brain.Update();
            Paint.Invoke();
            // Примените вектор скорости к положению, сделав движение муравья.
            MoveBasedOnVelocity();
        }

        public string GetStateName() => Brain.GetStateName();

        private void MoveBasedOnVelocity()
        {
            // Truncate
            double scalar;
            scalar = MaxVelocity / LengthVector(Velocity);
            scalar = scalar < 1.0 ? scalar : 1.0;

            // ScaleBy
            var x = Convert.ToInt32(Velocity.X * scalar);
            var y = Convert.ToInt32(Velocity.Y * scalar);
            Velocity = new Point(x, y);
            
            // Vector Add
            Position = new Point(Position.X + Velocity.X, Position.Y + Velocity.Y);

            Rotation = (float)(90 + (180 * GetAngle(Velocity)) / Math.PI);
        }

        private double GetAngle(Point p) => Math.Atan2(p.Y, p.X);

        private double LengthVector(Point p) => Math.Sqrt(p.X * p.X + p.Y * p.Y);

        private int Distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            int res = Convert.ToInt32(Math.Sqrt(dx * dx + dy * dy));
            return res;
        }
    }
}
