﻿namespace Kinect_Flipper
{
    partial class mainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainImage = new System.Windows.Forms.PictureBox();
            this.onOffButton = new System.Windows.Forms.Button();
            this.trackingLabel = new System.Windows.Forms.Label();
            this.swipeLabel = new System.Windows.Forms.Label();
            this.aboutButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mainImage)).BeginInit();
            this.SuspendLayout();
            // 
            // mainImage
            // 
            this.mainImage.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.mainImage.Location = new System.Drawing.Point(12, 12);
            this.mainImage.Name = "mainImage";
            this.mainImage.Size = new System.Drawing.Size(640, 480);
            this.mainImage.TabIndex = 0;
            this.mainImage.TabStop = false;
            // 
            // onOffButton
            // 
            this.onOffButton.Location = new System.Drawing.Point(51, 508);
            this.onOffButton.Name = "onOffButton";
            this.onOffButton.Size = new System.Drawing.Size(75, 23);
            this.onOffButton.TabIndex = 1;
            this.onOffButton.Text = "Turn Off";
            this.onOffButton.UseVisualStyleBackColor = true;
            this.onOffButton.Click += new System.EventHandler(this.onOffButton_Click);
            // 
            // trackingLabel
            // 
            this.trackingLabel.AutoSize = true;
            this.trackingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackingLabel.Location = new System.Drawing.Point(405, 495);
            this.trackingLabel.Name = "trackingLabel";
            this.trackingLabel.Size = new System.Drawing.Size(247, 46);
            this.trackingLabel.TabIndex = 2;
            this.trackingLabel.Text = "Not Tracking";
            // 
            // swipeLabel
            // 
            this.swipeLabel.AutoSize = true;
            this.swipeLabel.Location = new System.Drawing.Point(245, 513);
            this.swipeLabel.Name = "swipeLabel";
            this.swipeLabel.Size = new System.Drawing.Size(62, 13);
            this.swipeLabel.TabIndex = 3;
            this.swipeLabel.Text = "Not Swiped";
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(132, 508);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(75, 23);
            this.aboutButton.TabIndex = 4;
            this.aboutButton.Text = "About";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 543);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.swipeLabel);
            this.Controls.Add(this.trackingLabel);
            this.Controls.Add(this.onOffButton);
            this.Controls.Add(this.mainImage);
            this.Name = "mainForm";
            this.Text = "Book Reader";
            ((System.ComponentModel.ISupportInitialize)(this.mainImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox mainImage;
        private System.Windows.Forms.Button onOffButton;
        private System.Windows.Forms.Label trackingLabel;
        private System.Windows.Forms.Label swipeLabel;
        private System.Windows.Forms.Button aboutButton;
    }
}

