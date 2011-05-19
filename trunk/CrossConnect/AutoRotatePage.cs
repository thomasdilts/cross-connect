namespace CrossConnect
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using Microsoft.Phone.Controls;

    public class AutoRotatePage : PhoneApplicationPage
    {
        #region Fields

        /// <summary>
        /// This varialb eis needed in order to now what was the previous orientation
        /// and then choose the proper rotation mode
        /// </summary>
        private Nullable<PageOrientation> _previousOrientation = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// When leaving the page, clearing the orientation
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _previousOrientation = null;
        }

        /// <summary>
        /// When comming to the page, initiate the orientation
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _previousOrientation = Orientation;
        }

        /// <summary>
        /// When the orientation changes perform a rotate transition
        /// </summary>
        /// <param name="e"></param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            if (_previousOrientation == null)
                return;

            RotateTransition transitionElement = new RotateTransition();

            // counter clockwise rotation
            if (_previousOrientation == PageOrientation.LandscapeRight &&
                    e.Orientation == PageOrientation.PortraitUp ||
                _previousOrientation == PageOrientation.PortraitUp &&
                    e.Orientation == PageOrientation.LandscapeLeft ||
                _previousOrientation == PageOrientation.LandscapeLeft &&
                    e.Orientation == PageOrientation.PortraitDown ||
                _previousOrientation == PageOrientation.PortraitDown &&
                    e.Orientation == PageOrientation.LandscapeRight)
                transitionElement.Mode = RotateTransitionMode.In90Clockwise;
            // clockwise rotation
            else if (_previousOrientation == PageOrientation.LandscapeLeft &&
                        e.Orientation == PageOrientation.PortraitUp ||
                     _previousOrientation == PageOrientation.PortraitDown &&
                        e.Orientation == PageOrientation.LandscapeLeft ||
                     _previousOrientation == PageOrientation.LandscapeRight &&
                        e.Orientation == PageOrientation.PortraitDown ||
                     _previousOrientation == PageOrientation.PortraitUp &&
                        e.Orientation == PageOrientation.LandscapeRight)
                transitionElement.Mode = RotateTransitionMode.In90Counterclockwise;
            // 180 rotation
            else
                transitionElement.Mode = RotateTransitionMode.In180Clockwise;

            var transition = transitionElement.GetTransition((PhoneApplicationPage)(((PhoneApplicationFrame)Application.Current.RootVisual).Content));
            transition.Completed += delegate { transition.Stop(); };
            transition.Begin();

            _previousOrientation = e.Orientation;
        }

        #endregion Methods
    }
}