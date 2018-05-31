
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace AirMonitor.Controls
{
    /// <summary>
    /// 3D图形变化控制。
    /// </summary>
    public class Trackball3D
    {
        public static ModelVisual3D GetTransformModel(DependencyObject obj)
        {
            return (ModelVisual3D)obj.GetValue(TransformModelProperty);
        }

        public static void SetTransformModel(DependencyObject obj, ModelVisual3D value)
        {
            obj.SetValue(TransformModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for TransformModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransformModelProperty =
            DependencyProperty.RegisterAttached("TransformModel", typeof(ModelVisual3D), typeof(Trackball3D), new PropertyMetadata(null, new PropertyChangedCallback(OnTransformModelChanged)));

        public static Trackball3DContext GetTrackball3DContext(DependencyObject obj)
        {
            return (Trackball3DContext)obj.GetValue(Trackball3DContextProperty);
        }

        public static void SetTrackball3DContext(DependencyObject obj, Trackball3DContext value)
        {
            obj.SetValue(Trackball3DContextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Trackball3DContext.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Trackball3DContextProperty =
            DependencyProperty.RegisterAttached("Trackball3DContext", typeof(Trackball3DContext), typeof(Trackball3D), new PropertyMetadata(new Trackball3DContext()));


        private static void OnTransformModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IsViewport3D(d, (view, context) =>
             {
                 view.MouseDown -= View_MouseDown;
                 view.MouseUp -= View_MouseUp;
                 view.MouseWheel -= View_MouseWheel;
                 view.MouseMove -= View_MouseMove;

                 if (e.OldValue is ModelVisual3D oldModel)
                 {
                     oldModel.Transform = null;
                 }
                 if (e.NewValue is ModelVisual3D newModel)
                 {
                     var group = new Transform3DGroup();
                     context.Scale = new ScaleTransform3D(1, 1, 1);
                     context.Rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
                     context.Translate = new TranslateTransform3D(0, 0, 0);
                     group.Children.Add(context.Scale);
                     group.Children.Add(context.Rotate);
                     group.Children.Add(context.Translate);
                     newModel.Transform = group;
                     view.MouseDown += View_MouseDown;
                     view.MouseUp += View_MouseUp;
                     view.MouseWheel += View_MouseWheel;
                     view.MouseMove += View_MouseMove;
                 }
             });
        }

        private static void View_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            IsViewport3D(sender, (view, context) =>
            {
                e.Handled = true;
                var d = context.Scale.ScaleX + e.Delta / 1000d;
                if (d < 0.8 || d > 5)
                {
                    return;
                }
                context.Scale.ScaleX = d;
                context.Scale.ScaleY = d;
                context.Scale.ScaleZ = d;
            });
        }

        private static void IsViewport3D(object sender, Action<Viewport3D, Trackball3DContext> act)
        {
            if (sender is Viewport3D view)
            {
                act(view, GetTrackball3DContext(view));
            }
        }

        private static void View_MouseMove(object sender, MouseEventArgs e)
        {
            IsViewport3D(sender, (view, context) =>
            {
                if (view.IsMouseCaptured)
                {
                    var delta = context.MouseDownPoint - e.MouseDevice.GetPosition(view);
                    delta /= 2;
                    if (e.LeftButton == MouseButtonState.Pressed) //tranlate
                    {
                        //delta /= 20;
                        context.Translate.OffsetX = context.MouseDownTranslate.OffsetX - delta.X;
                        context.Translate.OffsetY = context.MouseDownTranslate.OffsetY + delta.Y;
                    }
                    else if (e.RightButton == MouseButtonState.Pressed) //rotate
                    {
                        if (delta.X == 0 && delta.Y == 0) return;
                        if (context.Rotate.Rotation is AxisAngleRotation3D a)
                        {
                            var a1 = context.MouseDownRotate.Rotation as AxisAngleRotation3D;
                            var lockx = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                            var locky = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                            var mouse = new Vector3D(lockx ? 0 : -delta.Y, locky ? 1 : -delta.X, 1);
                            //var axis = Vector3D.CrossProduct(mouse, a1.Axis);
                            //double len = axis.Length;
                            var q = new Quaternion(mouse, mouse.Length) * new Quaternion(a1.Axis, a1.Angle);
                            //a.Axis = q.Axis;
                            a.Axis = q.Axis;
                            a.Angle = q.Angle;// (a1.Angle - delta.X) % 360;
                            //view.Info("x={0},y={1},z={2},l={4},a={3}", a.Axis.X, a.Axis.Y, a.Axis.Z, a.Angle, a.Axis.Length);
                        }
                    }
                }
            });
        }

        private static void View_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsViewport3D(sender, (view, context) =>
            {
                view.ReleaseMouseCapture();
            });
        }

        private static void View_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsViewport3D(sender, (view, context) =>
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    context.Rotate.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
                }
                else
                {
                    context.MouseDownPoint = e.MouseDevice.GetPosition(view);
                    context.MouseDownTranslate = context.Translate.Clone();
                    context.MouseDownRotate = context.Rotate.Clone();
                    view.CaptureMouse();
                }
            });
        }
    }
}

