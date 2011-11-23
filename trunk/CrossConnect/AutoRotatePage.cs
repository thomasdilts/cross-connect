#region Header

// <copyright file="AutoRotatePage.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect
{
    using System.Windows;
    using System.Windows.Navigation;

    using Microsoft.Phone.Controls;

    public class AutoRotatePage : PhoneApplicationPage
    {
        #region Fields

        /// <summary>
        ///   This varialb eis needed in order to now what was the previous orientation
        ///   and then choose the proper rotation mode
        /// </summary>
        private PageOrientation? _previousOrientation;

        #endregion Fields

        #region Methods

        /// <summary>
        ///   When leaving the page, clearing the orientation
        /// </summary>
        /// <param name = "e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _previousOrientation = null;
        }

        /// <summary>
        ///   When comming to the page, initiate the orientation
        /// </summary>
        /// <param name = "e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _previousOrientation = Orientation;
        }

        /// <summary>
        ///   When the orientation changes perform a rotate transition
        /// </summary>
        /// <param name = "e"></param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            if (_previousOrientation == null)
                return;

            var transitionElement = new RotateTransition();

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

            var transition =
                transitionElement.GetTransition(
                    (PhoneApplicationPage) (((PhoneApplicationFrame) Application.Current.RootVisual).Content));
            transition.Completed += delegate { transition.Stop(); };
            transition.Begin();

            _previousOrientation = e.Orientation;
        }

        #endregion Methods
    }
}