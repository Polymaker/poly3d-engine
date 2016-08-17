namespace Poly3D.Test
{
    partial class Form2
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
            this.engineControl1 = new Poly3D.Platform.EngineControl();
            this.engineControl2 = new Poly3D.Platform.EngineControl();
            this.SuspendLayout();
            // 
            // engineControl1
            // 
            this.engineControl1.BackColor = System.Drawing.Color.Black;
            this.engineControl1.Location = new System.Drawing.Point(12, 12);
            this.engineControl1.Name = "engineControl1";
            this.engineControl1.Size = new System.Drawing.Size(150, 150);
            this.engineControl1.TabIndex = 0;
            this.engineControl1.VSync = false;
            // 
            // engineControl2
            // 
            this.engineControl2.BackColor = System.Drawing.Color.Black;
            this.engineControl2.Location = new System.Drawing.Point(168, 12);
            this.engineControl2.Name = "engineControl2";
            this.engineControl2.Size = new System.Drawing.Size(150, 150);
            this.engineControl2.TabIndex = 1;
            this.engineControl2.VSync = false;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 261);
            this.Controls.Add(this.engineControl2);
            this.Controls.Add(this.engineControl1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);

        }

        #endregion

        private Platform.EngineControl engineControl1;
        private Platform.EngineControl engineControl2;
    }
}