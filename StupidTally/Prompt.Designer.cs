namespace StupidTally
{
	partial class Prompt
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		public static string ShowDialog(string text, string caption) {
			Prompt prompt = new Prompt() {
				Width = 329,
				Height = 125,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				Text = caption,
				StartPosition = FormStartPosition.CenterScreen
			};
			prompt.textLabel.Text = text;
			prompt.textBox.KeyDown += (sender, e) => { if (e.KeyCode == Keys.Enter) { prompt.DialogResult = DialogResult.OK; prompt.Close(); }};
			prompt.okButton.Click += (sender, e) => { prompt.Close(); };
			prompt.cancelButton.Click += (sender, e) => { prompt.Close(); };

			return prompt.ShowDialog() == DialogResult.OK ? prompt.textBox.Text : "";
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.textLabel = new System.Windows.Forms.Label();
			this.textBox = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textLabel
			// 
			this.textLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textLabel.AutoSize = true;
			this.textLabel.Location = new System.Drawing.Point(12, 9);
			this.textLabel.Name = "textLabel";
			this.textLabel.Size = new System.Drawing.Size(77, 15);
			this.textLabel.TabIndex = 0;
			this.textLabel.Text = "Enter a value:";
			this.textLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(12, 27);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(289, 23);
			this.textBox.TabIndex = 1;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(145, 56);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 3;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(226, 56);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// Prompt
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(313, 86);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.textLabel);
			this.Name = "Prompt";
			this.Text = "Prompt";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public Label textLabel;
		public TextBox textBox;
		public Button okButton;
		public Button cancelButton;
	}
}