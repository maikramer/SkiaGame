namespace SkiaGame.Forms
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.skiaControl = new SkiaSharp.Views.Desktop.SKControl();
            this.SuspendLayout();
            // 
            // skiaControl
            // 
            this.skiaControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skiaControl.Location = new System.Drawing.Point(0, 0);
            this.skiaControl.Name = "skiaControl";
            this.skiaControl.Size = new System.Drawing.Size(832, 553);
            this.skiaControl.TabIndex = 0;
            this.skiaControl.Text = "SkiaGame";
            this.skiaControl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs>(this.skiaControl_PaintSurface);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 553);
            this.Controls.Add(this.skiaControl);
            this.Name = "MainWindow";
            this.Text = "SkiaGame";
            this.Load += new System.EventHandler(this.MainWindowLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private SkiaSharp.Views.Desktop.SKControl skiaControl;
    }
}