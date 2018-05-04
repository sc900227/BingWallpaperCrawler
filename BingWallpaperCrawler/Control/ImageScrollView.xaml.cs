using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BingWallpaperCrawler.Control
{
    /// <summary>
    /// ImageScrollView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageScrollView : UserControl
    {
       

        public delegate void TouchDownHander(UIElement view, int index);
        public event TouchDownHander OnTouchDownEvent;

        private static double OPACITY_DOWN_FACTOR = 0.1;    // 图像之间的透明度差

        private double _xCenter;
        private double _yCenter;

        private static double SPRINESS = 0.05;		    // 弹性运动参数
        private static double CRITICAL_POINT = 0.001;
        private static double MOVE_DISTANCE = 25; //移动距离后转换
        private double _touch_move_distance = 0;
        private double _target = 1;		// 目标位置
        private double _current = 0;	// 当前位置
        private int _chickCurrent = -1;
        public double CurrentIndex
        {
            get { return _current; }
        }
        public double TargetIndex
        {
            get { return _target; }
        }
        public int ChickCurrentIndex
        {
            get { return _chickCurrent; }
        }
        private static double childViewWidth = 300;
        public double ChildViewWidth
        {
            get { return childViewWidth; }
            set { childViewWidth = value; }
        }

        private double childViewHeight = 300;

        public double ChildViewHeight
        {
            get { return childViewHeight; }
            set { childViewHeight = value; }
        }
        private double spaceWidth = 300;

        public double SpaceWidth
        {
            get { return spaceWidth; }
            set { spaceWidth = value; }
        }
        private bool IsPressed = false;
        private bool LockMoveEvent = false; //锁住滑动事件
        private bool enableslide = true; //是否允许滑行
        public bool Enableslide
        {
            get { return enableslide; }
            set { enableslide = value; }
        }
        private List<Viewport3DControl> _images = new List<Viewport3DControl>();
        private DispatcherTimer _timer = new DispatcherTimer();

        public ImageScrollView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ImageScrollView_Loaded);
            Start();
        }

        void ImageScrollView_Loaded(object sender, RoutedEventArgs e)
        {
            _xCenter = LayoutRoot.Width / 2 - SpaceWidth / 2 - 30;
            _yCenter = LayoutRoot.Height / 2 - ChildViewHeight / 2;

        }

        void _timer_Tick(object sender, EventArgs e)
        {
            //还原位置
            if (IsPressed == false && _touch_move_distance != 0)
            {
                //回弹
                _touch_move_distance += (-_touch_move_distance) * SPRINESS;
            }
            for (int i = 0; i < _images.Count; i++)
            {
                Viewport3DControl image = _images[i];
                posImage(image, i);
            }

            if (Math.Abs(_target - _current) < CRITICAL_POINT && IsPressed == false) return;
            _current += (_target - _current) * SPRINESS;
        }

        public void AddImages(string[] imagesUri)
        {
            for (int i = 0; i < imagesUri.Length; i++)
            {
                string url = imagesUri[i];
                Viewport3DControl image = new Viewport3DControl();
                image.SetImageSource(url);
                image.Index = i;
                image.Width = ChildViewWidth;
                image.Height = ChildViewHeight;
                image.MouseUp += new MouseButtonEventHandler(image_MouseUp);
                image.MouseDown += new MouseButtonEventHandler(image_MouseDown);
                LayoutRoot.Children.Add(image);
                posImage(image, i);
                _images.Add(image);
            }
        }
        public void AddImage(string imagesrc)
        {
            Viewport3DControl image = new Viewport3DControl();
            image.SetImageSource(imagesrc);
            image.Index = _images.Count;
            image.Width = ChildViewWidth;
            image.Height = ChildViewHeight;
            image.MouseUp += new MouseButtonEventHandler(image_MouseUp);
            image.MouseDown += new MouseButtonEventHandler(image_MouseDown);
            LayoutRoot.Children.Add(image);

            posImage(image, _images.Count);
            _images.Add(image);
        }
        public void AddImage(ImageSource bitmapImage, int index)
        {
            Viewport3DControl image = new Viewport3DControl();
            image.SetImageSource(ref bitmapImage);
            image.Index = index;
            image.Width = ChildViewWidth;
            image.Height = ChildViewHeight;
            image.MouseUp += new MouseButtonEventHandler(image_MouseUp);
            image.MouseDown += new MouseButtonEventHandler(image_MouseDown);
            LayoutRoot.Children.Add(image);
            posImage(image, _images.Count);
            _images.Add(image);
        }
        private double mouseHaseMove = 0;
        void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseHaseMove = e.GetPosition(LayoutRoot).X;
            Viewport3DControl view = (Viewport3DControl)sender;
            //view.SetImageChickState();
            ChangeImageChickStyle(view.Index);
            _chickCurrent = view.Index;
            //ChangeImageChickStyle(Convert.ToInt32(CurrentIndex));
        }

        void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Math.Abs(e.GetPosition(LayoutRoot).X - mouseHaseMove) < 3)
            {
                Viewport3DControl view = (Viewport3DControl)sender;
                //OnTouchDownEvent(view, view.Index);
            }
        }

        private void ChangeImageChickStyle(int index) {
            if (_images!=null&&_images.Count>0)
            {
                foreach (var item in _images)
                {
                    item.RemoveImageChickState();
                }
                _images[index].SetImageChickState();
            }
        }
        private void posImage(Viewport3DControl image, int index)
        {
            //  LayoutRoot.Children.Add(image);

            double diffFactor = index - _current;

            double left = _xCenter + diffFactor * SpaceWidth - _touch_move_distance;
            double top = 0.0;
            double Zindex = -Math.Abs(diffFactor) * 100;
            image.SetValue(Canvas.ZIndexProperty, (int)Zindex);
            double scalez = 1 - Math.Abs(diffFactor) * 0.07;

            // image.Width = ChildViewWidth;
            // image.Height = ChildViewHeight;

            if (index == _target)
            {
                double angle = (left - _xCenter) * 0.05;
                image.AnimationTransformTo(scalez, scalez, 1, angle);
                left += 30;
            }
            else if (index > _target)
            {
                image.AnimationTransformTo(scalez, scalez, 1, 8);
                left += 60;
            }
            else if (index < _target)
            {
                image.AnimationTransformTo(scalez, scalez, 1, -8); //-10
            }
            image.Opacity = 1 - Math.Abs(diffFactor) * OPACITY_DOWN_FACTOR;
            image.SetValue(Canvas.LeftProperty, left);
            image.SetValue(Canvas.TopProperty, top);


        }
        private void moveIndex(int value)
        {
            _target += value;
            _target = Math.Max(0, _target);
            _target = Math.Min(_images.Count - 1, _target);
        }

        public void MoveLeft()
        {
            moveIndex(-1);
        }

        public void MoveRight()
        {
            moveIndex(1);
        }

        private void Start()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        Double tempi = 0;
        Point tempp;
        bool hasrun = false;

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (SlideAffect == SlideAffectEnum.OrderSlide)
                OrderSlideAffect(sender, e);
            else
                JumpSlideAffect(sender, e);
        }

        //效果1，释放滑动后才根据滑动跟离跳转到相关项(可直接跳几项)
        private void JumpSlideAffect(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsPressed = true;
                if (tempp.X > e.GetPosition(LayoutRoot).X)
                {
                    _touch_move_distance += 1;
                }
                else if (tempp.X < e.GetPosition(LayoutRoot).X)
                {
                    _touch_move_distance -= 1;
                }
                tempp = e.GetPosition(LayoutRoot);
                hasrun = false;
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (hasrun == false)
                {
                    Double moveindexcount = Math.Round(_touch_move_distance / SpaceWidth, 0);
                    if (_touch_move_distance > MOVE_DISTANCE)
                        moveindexcount += 1;
                    else if (_touch_move_distance < -MOVE_DISTANCE)
                        moveindexcount -= 1;

                    moveIndex((int)moveindexcount);
                    hasrun = true;
                }
                IsPressed = false;
                tempp = new Point(0, 0);
            }
            else
            {
                tempi = 0;
                tempp = new Point(0, 0);
            }
        }
        //效果2，滑动到一定距离后自动跳转到下一项(当Enableslide == false时，每次只能跳一项)
        private void OrderSlideAffect(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsPressed = true;
                tempi += 1;
                if (tempp.X > e.GetPosition(LayoutRoot).X)
                {
                    if (tempi > MOVE_DISTANCE)
                    {
                        if (LockMoveEvent == false || Enableslide == true)
                        {
                            LockMoveEvent = true;
                            MoveRight();
                        }
                        tempi = 0;
                    }
                    _touch_move_distance += 1;
                }
                else if (tempp.X < e.GetPosition(LayoutRoot).X)
                {
                    if (tempi > MOVE_DISTANCE)
                    {
                        if (LockMoveEvent == false || Enableslide == true)
                        {
                            LockMoveEvent = true;
                            MoveLeft();
                        }
                        tempi = 0;
                    }
                    _touch_move_distance -= 1;

                }
                tempp = e.GetPosition(LayoutRoot);

            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                LockMoveEvent = false;
                IsPressed = false;
                tempi = 0;
                tempp = new Point(0, 0);
            }
            else
            {
                LockMoveEvent = false;
                tempi = 0;
                tempp = new Point(0, 0);
            }
        }

        public enum SlideAffectEnum
        {
            JumpSlide,
            OrderSlide
        }

        private SlideAffectEnum slideAffect;

        public SlideAffectEnum SlideAffect
        {
            get { return slideAffect; }
            set { slideAffect = value; }
        }
    }
}
