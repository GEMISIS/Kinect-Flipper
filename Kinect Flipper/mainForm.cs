using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.Kinect;

namespace Kinect_Flipper
{
    public partial class mainForm : Form
    {
        /// <summary>
        /// The Kinect sensor that is in use.
        /// </summary>
        private KinectSensor kinect;

        /// <summary>
        /// The left key stroke to send.
        /// </summary>
        private string leftKey = "{LEFT}";
        /// <summary>
        /// The right key stroke to send.
        /// </summary>
        private string rightKey = "{RIGHT}";

        /// <summary>
        /// This event executs when the form is closing.  Turns of the Kinect device
        /// and gets rid of its resources.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the Kinect camera.  If it has already stopped, then nothing will happen.
            kinect.Stop();
            // Dispose of the Kinect camera to free up resources (just to be safe).
            kinect.Dispose();
        }

        /// <summary>
        /// The main form constructor.  Initializes the Kinect Sensor, and sets
        /// the event handlers for when a color frame and skeleton frame are recieved, as well
        /// as sets up the form closing event.
        /// </summary>
        public mainForm()
        {
            // Initializes the components.
            InitializeComponent();

            // Check if there are Kinect Sensors available.
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // Sets the Kinect sensor to the default sensor.
                kinect = KinectSensor.KinectSensors[0];

                // Sets up the color frame event.
                kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinect_ColorFrameReady);
                // Sets up the Skeleton frame event.
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);

                // Enable the color stream at 640x480 resolution and 30 frames per second.
                kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                // Set the tracking to be enabled in near range by default.
                kinect.SkeletonStream.EnableTrackingInNearRange = true;
                // Set the skeleton tracking to full body.  While the legs aren't
                // used, it is useful for PowerPoint presentations.
                kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                // Don't let the app chose what skeletons to track.
                kinect.SkeletonStream.AppChoosesSkeletons = false;
                // Enable Skeleton tracking.
                kinect.SkeletonStream.Enable();

                // Start the Kinect camera.
                kinect.Start();

                // Setup the form closing event to disconnect the sensor and cleanup.
                this.FormClosing += new FormClosingEventHandler(mainForm_FormClosing);
            }
            else
            {
                // If there are no Kinect Sensors, then we set it to null so the application
                // knows there is no Kinect currently.
                kinect = null;
            }
        }

        /// <summary>
        /// Keep a list of skeletons available.  This helps speed things up rather
        /// than creating a new array each time.
        /// </summary>
        Skeleton[] skeletons = null;
        
        /// <summary>
        /// The old skeleton frame.  Kept for keeping track of when to update frames via its timestamp.
        /// </summary>
        SkeletonFrame oldFrame = null;

        // This section keeps track of the various old skeleton points for tracking.
        #region oldSkeletonPoints
        /// <summary>
        /// The old right hand.
        /// </summary>
        SkeletonPoint oldSkeletonHandRight;
        /// <summary>
        /// <summary>
        /// The old right shoulder.
        /// </summary>
        SkeletonPoint oldSkeletonShoulderRight;
        /// <summary>
        /// The old left hand.
        /// </summary>
        SkeletonPoint oldSkeletonHandLeft;
        /// <summary>
        /// The old left shoulder.
        /// </summary>
        SkeletonPoint oldSkeletonShoulderLeft;
        #endregion

        /// <summary>
        /// Updates the tracking status label, as well as the controls
        /// for the application.  This is done here since we recieve the
        /// skeleton frame here initially.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">The skeleton frame.</param>
        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // By default we set the tracking label text to not tracking.
            trackingLabel.Text = "Not Tracking";

            // Get the current skeleton frame.
            SkeletonFrame frame = e.OpenSkeletonFrame();

            // Check that there is an actual frame.
            if (frame != null)
            {
                // Check that there are actually skeletons in the frame.
                if (frame.SkeletonArrayLength > 0)
                {
                    // Check if the skeletons array is null.
                    // We only need to initialize it once, after that, it can hold the correct
                    // number of skeletons each time.
                    if (skeletons == null)
                    {
                        // Initialize the skeletons array.
                        skeletons = new Skeleton[frame.SkeletonArrayLength];
                    }
                    // Then we copy the data to the skeletons array.
                    frame.CopySkeletonDataTo(skeletons);

                    // Go through each skeleton in the array.
                    foreach (Skeleton s in skeletons)
                    {
                        // Check that the skeleton is being tracked.
                        if (s.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // Update the tracking label text that
                            // the application is tracking a skeleton.
                            trackingLabel.Text = "Tracking!";

                            // Check if the left hand is being tracked.
                            if (s.Joints[JointType.HandLeft].TrackingState != JointTrackingState.NotTracked)
                            {
                                // Then check if the left shoulder is being tracked.
                                if (s.Joints[JointType.ShoulderLeft].TrackingState != JointTrackingState.NotTracked)
                                {
                                    // Next check that we have old skeleton hand data.
                                    if (oldSkeletonHandLeft != null)
                                    {
                                        // Then check the position of the old skeleton hand.
                                        if (oldSkeletonHandLeft.X < oldSkeletonShoulderLeft.X)
                                        {
                                            // Then we track the new data.
                                            if (s.Joints[JointType.HandLeft].Position.X > s.Joints[JointType.ShoulderLeft].Position.X)
                                            {
                                                // Next we get distance between the hand and the shoulder's X positions.
                                                if (Math.Abs(s.Joints[JointType.HandLeft].Position.X - s.Joints[JointType.ShoulderLeft].Position.X) > 0.10)
                                                {
                                                    // Optionally, we could also make sure the hand is at a specific location on the Y axis.
                                                    //if (s.Joints[JointType.HandLeft].Position.Y < s.Joints[JointType.HipCenter].Position.Y)
                                                    {
                                                        // Update the swipe label text.
                                                        swipeLabel.Text = "Swiped right";
                                                        // Send the correct keystroke.
                                                        SendKeys.Send(leftKey);

                                                        // Update the old frame.
                                                        oldFrame = frame;
                                                        // Update the old skeleton right hand.
                                                        oldSkeletonHandRight = s.Joints[JointType.HandRight].Position;
                                                        // Update the old skeleton right elbow.
                                                        oldSkeletonShoulderRight = s.Joints[JointType.ShoulderRight].Position;
                                                        // Update the old skeleton left hand.
                                                        oldSkeletonHandLeft = s.Joints[JointType.HandLeft].Position;
                                                        // Update the old skeleton left shoulder.
                                                        oldSkeletonShoulderLeft = s.Joints[JointType.ShoulderLeft].Position;
                                                        //updatedLabel.Text = "Updated";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // Check if the right hand is being tracked.
                            if (s.Joints[JointType.HandRight].TrackingState != JointTrackingState.NotTracked)
                            {
                                // Then check if the right shoulder is being tracked.
                                if (s.Joints[JointType.ShoulderRight].TrackingState != JointTrackingState.NotTracked)
                                {
                                    // Next check that we have old skeleton hand data.
                                    if (oldSkeletonHandRight != null)
                                    {
                                        // Then check the position of the old skeleton hand.
                                        if (oldSkeletonHandRight.X > oldSkeletonShoulderRight.X)
                                        {
                                            // Then we track the new data.
                                            if (s.Joints[JointType.HandRight].Position.X < s.Joints[JointType.ShoulderRight].Position.X)
                                            {
                                                // Next we get distance between the hand and the shoulder's X positions.
                                                if (Math.Abs(s.Joints[JointType.HandRight].Position.X - s.Joints[JointType.ShoulderRight].Position.X) > 0.10)
                                                {
                                                    // Optionally, we could also make sure the hand is at a specific location on the Y axis.
                                                    //if (s.Joints[JointType.HandRight].Position.Y < s.Joints[JointType.HipCenter].Position.Y)
                                                    {
                                                        // Update the swipe label text.
                                                        swipeLabel.Text = "Swiped left";
                                                        // Send the correct keystroke.
                                                        SendKeys.Send(rightKey);

                                                        // Update the old frame.
                                                        oldFrame = frame;
                                                        // Update the old skeleton right hand.
                                                        oldSkeletonHandRight = s.Joints[JointType.HandRight].Position;
                                                        // Update the old skeleton right elbow.
                                                        oldSkeletonShoulderRight = s.Joints[JointType.ShoulderRight].Position;
                                                        // Update the old skeleton left hand.
                                                        oldSkeletonHandLeft = s.Joints[JointType.HandLeft].Position;
                                                        // Update the old skeleton left shoulder.
                                                        oldSkeletonShoulderLeft = s.Joints[JointType.ShoulderLeft].Position;
                                                        //updatedLabel.Text = "Updated";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // Check if the old frame is null.
                            if (oldFrame != null)
                            {
                                // If it's not null, then we check if the time difference
                                // between the old frame and new frame is great than 750 ms.
                                if (Math.Abs(frame.Timestamp - oldFrame.Timestamp) > 750)
                                {
                                    // Update the old frame.
                                    oldFrame = frame;
                                    // Update the old skeleton right hand.
                                    oldSkeletonHandRight = s.Joints[JointType.HandRight].Position;
                                    // Update the old skeleton right elbow.
                                    oldSkeletonShoulderRight = s.Joints[JointType.ShoulderRight].Position;
                                    // Update the old skeleton left hand.
                                    oldSkeletonHandLeft = s.Joints[JointType.HandLeft].Position;
                                    // Update the old skeleton left shoulder.
                                    oldSkeletonShoulderLeft = s.Joints[JointType.ShoulderLeft].Position;
                                    //updatedLabel.Text = "Updated";
                                }
                            }
                            else
                            {
                                // Update the old frame.
                                oldFrame = frame;
                            }
                            // Break from the loop since we found the skeleton to use.
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When a color frame is recieved, it takes the frame, resizes it, and then
        /// sets it to the displayed image on the application for user feedback.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">The color frame recieved.</param>
        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // Create a color frame to hold the color frame that was recieved.
            ColorImageFrame frame = e.OpenColorImageFrame();
            // Check that the frame is not empty.
            if (frame != null)
            {
                // Create an array of bytes to hold the data.
                byte[] pixelData = new byte[frame.PixelDataLength];
                // Copy the pixel data to the array.
                frame.CopyPixelDataTo(pixelData);

                // Create a bmp to hold the actual image.
                Bitmap bmp = new Bitmap(frame.Width, frame.Height, PixelFormat.Format32bppRgb);
                // Create the bitmap data to hold the data for the image.
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                // Create the pointer to the bmp data.
                IntPtr ptr = bmpData.Scan0;
                // Then copy the actual data to the bmp.
                System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, ptr, frame.PixelDataLength);
                // Unlock the bmp data bits so they can be used.
                bmp.UnlockBits(bmpData);
                // Set the color image to the bmp image.
                this.mainImage.Image = bmp;

                // Dispose of the frame.
                frame.Dispose();
            }
        }

        /// <summary>
        /// Used for when the on/off button is clicked.  Simply starts or stops
        /// the device based on whether it is already running.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void onOffButton_Click(object sender, EventArgs e)
        {
            // Check if the Kinect sensor is running.
            if (kinect.IsRunning)
            {
                // Stop the sensor.
                kinect.Stop();
                // Change the button text to "Turn On".
                onOffButton.Text = "Turn On";
            }
            else
            {
                // Start the sensor.
                kinect.Start();
                // Change the button text to "Turn Off".
                onOffButton.Text = "Turn Off";
            }
        }

        /// <summary>
        /// Display information about the program.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void aboutButton_Click(object sender, EventArgs e)
        {
            // Display a message box showing the creator of the program.
            MessageBox.Show("Created by: Gerald McAlister", "About");
        }

        /// <summary>
        /// The left key label's click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leftKeyLabel_Click(object sender, EventArgs e)
        {
            // Set the preview key event.
            leftKeyLabel.PreviewKeyDown += new PreviewKeyDownEventHandler(leftKeyLabel_PreviewKeyDown);
            // Set the lost focus event.
            leftKeyLabel.LostFocus += new EventHandler(leftKeyLabel_LostFocus);
            // Focus on the label.
            leftKeyLabel.Focus();
        }

        /// <summary>
        /// The right key label's click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rightKeyLabel_Click(object sender, EventArgs e)
        {
            // Set the preview key event.
            rightKeyLabel.PreviewKeyDown += new PreviewKeyDownEventHandler(rightKeyLabel_PreviewKeyDown);
            // Set the lost focus event.
            rightKeyLabel.LostFocus += new EventHandler(rightKeyLabel_LostFocus);
            // Focus on the label.
            rightKeyLabel.Focus();
        }

        /// <summary>
        /// The left key label's key preview event.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">The key arguments.  This is used to get the key.</param>
        void leftKeyLabel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Check if the key code is not the escape key.
            if (!e.KeyCode.Equals(Keys.Escape))
            {
                // If it is not the escape key, set the left key value.
                // Start by checking if it is a letter.
                if (e.KeyValue >= 'A' && e.KeyValue <= 'z')
                {
                    // If so, set it to lower case by default.
                    leftKey = Convert.ToChar(e.KeyValue).ToString().ToLower();
                    // Then check if caps lock is enabled.
                    if (IsKeyLocked(Keys.CapsLock))
                    {
                        // If so, set the letter to be upper case.
                        leftKey = leftKey.ToUpper();
                    }
                    // Tell the user the left key has been changed.
                    MessageBox.Show("Set the left key to " + leftKey + "!\n");
                    // Update the text lable for the left key.
                    leftKeyLabel.Text = "Left Key: " + leftKey;
                }
                // Otherwise check if the key is an arrow key.
                else if (e.KeyCode.Equals(Keys.Left) || e.KeyCode.Equals(Keys.Right) || e.KeyCode.Equals(Keys.Up) || e.KeyCode.Equals(Keys.Down))
                {
                    // If it is not the escape key, set the left key value.
                    leftKey = "{" + new KeysConverter().ConvertToString(e.KeyCode).ToUpper() + "}";
                    // Tell the user the left key has been changed.
                    MessageBox.Show("Set the left key to " + new KeysConverter().ConvertToString(e.KeyCode) + "!\n");
                    // Update the text lable for the left key.
                    leftKeyLabel.Text = "Left Key: " + new KeysConverter().ConvertToString(e.KeyCode);
                }
            }
            // Remove the key preview event.
            leftKeyLabel.PreviewKeyDown -= new PreviewKeyDownEventHandler(leftKeyLabel_PreviewKeyDown);
            // Focus on the on/off button.
            this.onOffButton.Focus();
        }

        /// <summary>
        /// The right key label's key preview event.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">The key arguments.  This is used to get the key.</param>
        void rightKeyLabel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Check if the key code is not the escape key.
            if (!e.KeyCode.Equals(Keys.Escape))
            {
                // If it is not the escape key, set the right key value.
                // Start by checking if it is a letter.
                if (e.KeyValue >= 'A' && e.KeyValue <= 'z')
                {
                    // If so, set it to lower case by default.
                    rightKey = Convert.ToChar(e.KeyValue).ToString().ToLower();
                    // Then check if caps lock is enabled.
                    if (IsKeyLocked(Keys.CapsLock))
                    {
                        // If so, set the letter to be upper case.
                        rightKey = rightKey.ToUpper();
                    }
                    // Tell the user the right key has been changed.
                    MessageBox.Show("Set the right key to letter " + rightKey + "!\n");
                    // Update the text lable for the right key.
                    rightKeyLabel.Text = "Right Key: " + rightKey;
                }
                // Otherwise check if the key is an arrow key.
                else if (e.KeyCode.Equals(Keys.Left) || e.KeyCode.Equals(Keys.Right) || e.KeyCode.Equals(Keys.Up) || e.KeyCode.Equals(Keys.Down))
                {
                    // If it is not the escape key, set the right key value.
                    rightKey = "{" + new KeysConverter().ConvertToString(e.KeyCode).ToUpper() + "}";
                    // Tell the user the right key has been changed.
                    MessageBox.Show("Set the right key to " + new KeysConverter().ConvertToString(e.KeyCode) + "!\n");
                    // Update the text lable for the right key.
                    rightKeyLabel.Text = "Right Key: " + new KeysConverter().ConvertToString(e.KeyCode);
                }
            }
            // Remove the key preview event.
            rightKeyLabel.PreviewKeyDown -= new PreviewKeyDownEventHandler(rightKeyLabel_PreviewKeyDown);
            // Focus on the on/off button.
            this.onOffButton.Focus();
        }

        /// <summary>
        /// The event for when the right key label loses focus.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        void rightKeyLabel_LostFocus(object sender, EventArgs e)
        {
            // Remove the key preview event.
            rightKeyLabel.PreviewKeyDown -= new PreviewKeyDownEventHandler(rightKeyLabel_PreviewKeyDown);
        }

        /// <summary>
        /// The event for when the left key label loses focus.
        /// </summary>
        /// <param name="sender">Not used.</param>
        /// <param name="e">Not used.</param>
        void leftKeyLabel_LostFocus(object sender, EventArgs e)
        {
            // Remove the key preview event.
            leftKeyLabel.PreviewKeyDown -= new PreviewKeyDownEventHandler(leftKeyLabel_PreviewKeyDown);
        }
    }
}
