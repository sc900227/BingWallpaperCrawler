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
using System.Windows.Media.Media3D;
using System.IO;
using System.Windows.Media.Animation;

namespace BingWallpaperCrawler.Control
{
    /// <summary>
    /// Viewport3DControl.xaml 的交互逻辑
    /// </summary>
    public partial class Viewport3DControl : UserControl
    {
        public Viewport3DControl()
        {
            InitializeComponent();
        }
        
        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        //这里是向右滑动
        public void MoveToRight()
        {
            GeometryModel3D current = this.contentView;
            var transform3DGroup = current.Transform as Transform3DGroup;
            var rotate = (transform3DGroup.Children[0] as RotateTransform3D).Rotation as AxisAngleRotation3D;
            AnimationVisualElement(rotate, 45);

        }

        //这里是向左滑动
        public void MoveToLeft()
        {
            GeometryModel3D current = this.contentView;
            var transform3DGroup = current.Transform as Transform3DGroup;
            var rotate = (transform3DGroup.Children[0] as RotateTransform3D).Rotation as AxisAngleRotation3D;
            AnimationVisualElement(rotate, -45);

        }
        //回到中间
        public void MoveToCenter()
        {
            GeometryModel3D current = this.contentView;
            var transform3DGroup = current.Transform as Transform3DGroup;
            var rotate = (transform3DGroup.Children[0] as RotateTransform3D).Rotation as AxisAngleRotation3D;
            AnimationVisualElement(rotate, 0);

        }
        //动画变换
        public void AnimationTransformTo(double angel)
        {
            GeometryModel3D current = this.contentView;
            var transform3DGroup = current.Transform as Transform3DGroup;
            var scale = transform3DGroup.Children[1] as ScaleTransform3D;
            var rotate = (transform3DGroup.Children[0] as RotateTransform3D).Rotation as AxisAngleRotation3D;
            AnimationVisualElement(rotate, angel);

        }
        //动画变换
        /// <summary>
        /// 全动画效果，较低性能
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="angel"></param>
        public void AnimationTransformTo(double scaleX, double scaleY, double scaleZ, double angel)
        {
            GeometryModel3D current = this.contentView;
            var transform3DGroup = current.Transform as Transform3DGroup;

            var scale = transform3DGroup.Children[1] as ScaleTransform3D;
            var rotate = (transform3DGroup.Children[0] as RotateTransform3D).Rotation as AxisAngleRotation3D;
            if (angel > 0)
                rotate.Axis = (new Vector3D(0, 300, 0));
            else if (angel > 0)
                rotate.Axis = (new Vector3D(0, 1, 0));
            else
                rotate.Axis = (new Vector3D(0, 150, 0));

            AnimationVisualElement(rotate, scale, angel, scaleX, scaleY, scaleZ);
        }
        //动画变换
        /// <summary>
        /// 只有旋转动画效果，缩放无动画效果，性能较好
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="angel"></param>
        public void AnimationRotateTo(double scaleX, double scaleY, double scaleZ, double angel)
        {
            GeometryModel3D current = this.contentView;
            var transform3DGroup = current.Transform as Transform3DGroup;

            var scale = transform3DGroup.Children[1] as ScaleTransform3D;
            var rotate = (transform3DGroup.Children[0] as RotateTransform3D).Rotation as AxisAngleRotation3D;
            if (angel > 0)
                rotate.Axis = (new Vector3D(0, 300, 0));
            else if (angel > 0)
                rotate.Axis = (new Vector3D(0, 1, 0));
            else
                rotate.Axis = (new Vector3D(0, 150, 0));

            scale.ScaleX = scaleX;
            scale.ScaleY = scaleY;
            scale.ScaleZ = scaleZ;
            AnimationVisualElement(rotate, angel);
        }
        //无动画变换
        /// <summary>
        /// 无动画效果，性能最好
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <param name="angel"></param>
        public void StaticTransformTo(double scaleX, double scaleY, double scaleZ, double angel)
        {
            GeometryModel3D current = this.contentView;
            var transform3DGroup = current.Transform as Transform3DGroup;

            var scale = transform3DGroup.Children[1] as ScaleTransform3D;
            var rotate = (transform3DGroup.Children[0] as RotateTransform3D).Rotation as AxisAngleRotation3D;
            rotate.Angle = angel;
            scale.ScaleX = scaleX;
            scale.ScaleY = scaleY;
            scale.ScaleZ = scaleZ;
        }
        private void AnimationVisualElement(AxisAngleRotation3D rotate, double angel)
        {
            Duration duration = new Duration(TimeSpan.FromSeconds(.1));
            //对AxisAngleRotation3D的Angle属性应用动画
            DoubleAnimation animationAngel = new DoubleAnimation();
            animationAngel.To = angel;
            animationAngel.AccelerationRatio = 0.3;
            animationAngel.DecelerationRatio = 0.7;
            animationAngel.Duration = duration;
            rotate.BeginAnimation(AxisAngleRotation3D.AngleProperty, animationAngel);
        }
        private void AnimationVisualElement(AxisAngleRotation3D rotate, ScaleTransform3D scale, double angel, double scaleX, double scaleY, double scaleZ)
        {
            Duration duration = new Duration(TimeSpan.FromSeconds(.1));
            //对AxisAngleRotation3D的Angle属性应用动画
            DoubleAnimation animationAngel = new DoubleAnimation();
            animationAngel.To = angel;
            animationAngel.AccelerationRatio = 0.3;
            animationAngel.DecelerationRatio = 0.7;
            animationAngel.Duration = duration;
            rotate.BeginAnimation(AxisAngleRotation3D.AngleProperty, animationAngel);

            //对ScaleTransform3D的scaleX属性应用动画
            DoubleAnimation animationScaleX = new DoubleAnimation();
            animationScaleX.To = scaleX;
            animationScaleX.AccelerationRatio = 0.3;
            animationScaleX.DecelerationRatio = 0.7;
            animationScaleX.Duration = duration;
            scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, animationScaleX);

            DoubleAnimation animationScaleY = new DoubleAnimation();
            animationScaleY.To = scaleY;
            animationScaleY.AccelerationRatio = 0.3;
            animationScaleY.DecelerationRatio = 0.7;
            animationScaleY.Duration = duration;
            scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, animationScaleY);

            scale.ScaleZ = scaleZ;
            /*
          //对ScaleTransform3D的scaleZ属性应用动画
          DoubleAnimation animationScaleZ = new DoubleAnimation();
          animationScaleZ.To = scaleZ;
          animationScaleZ.AccelerationRatio = 0.3;
          animationScaleZ.DecelerationRatio = 0.7;
          animationScaleZ.Duration = duration;
          scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, animationScaleZ);
             */
        }
        public void SetImageChickState() {
            bdBack.BorderThickness = new Thickness(5, 5, 5, 5);
            bdBack.BorderBrush = new SolidColorBrush(Colors.SkyBlue);
        }
        public void RemoveImageChickState()
        {
            bdBack.BorderThickness = new Thickness(0, 0, 0, 0);
            bdBack.BorderBrush = new SolidColorBrush(Colors.White);
        }
        public void SetImageSource(string uri)
        {
            GeometryModel3D current = this.contentView;
            DiffuseMaterial mater = new DiffuseMaterial();
            ImageBrush brush = new ImageBrush(new BitmapImage(new Uri(uri)));
            // ImageBrush brush = new ImageBrush(new BitmapImage(new Uri(uri, UriKind.Absolute)));
            brush.Stretch = Stretch.Uniform;
            mater.Brush = brush;
            current.Material = mater;
           
        }
        public void SetImageSource(ref ImageSource image)
        {
            GeometryModel3D current = this.contentView;
            DiffuseMaterial mater = new DiffuseMaterial();
            ImageBrush brush = new ImageBrush(image);
            brush.Stretch = Stretch.Uniform;
            mater.Brush = brush;
            current.Material = mater;
            
        }
        public void SetImageSource(Stream streamSource)
        {
            GeometryModel3D current = this.contentView;
            DiffuseMaterial mater = new DiffuseMaterial();
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.None;
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.StreamSource = streamSource;
            image.EndInit();

            ImageBrush brush = new ImageBrush(image);
            brush.Stretch = Stretch.Uniform;
            mater.Brush = brush;
            current.Material = mater;
            
        }
    }
}
