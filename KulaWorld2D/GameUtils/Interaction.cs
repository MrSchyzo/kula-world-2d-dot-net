using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameUtils
{
    public static class Interaction
    {
        /// <summary>
        /// Questo metodo crea un dialog che riceve in input due stringhe.
        /// </summary>
        /// <param name="title">Titolo del dialog</param>
        /// <param name="prompt1">Testo da scrivere sopra la prima casella</param>
        /// <param name="prompt2">Testo da scrivere sopra la seconda casella</param>
        /// <param name="val1">Primo valore che verrà modificato dal dialog</param>
        /// <param name="val2">Secondo valore che verrà modificato dal dialog</param>
        /// <returns></returns>
        public static DialogResult DoubleTextInputBox(string title, string prompt1, string prompt2, ref string val1, ref string val2)
        {
            Form form = new Form();
            Label label1 = new Label();
            Label label2 = new Label();
            TextBox textBox1 = new TextBox();
            TextBox textBox2 = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label1.Text = prompt1;
            label2.Text = prompt2;
            textBox1.Text = val1;
            textBox2.Text = val2;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            label1.SetBounds(9, 20, 120, 13);
            textBox1.SetBounds(12, 36, 100, 20);
            label2.SetBounds(19 + 120, 20, 120, 13);
            textBox2.SetBounds(22 + 120, 36, 100, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            //label1.AutoSize = true;
            //label2.AutoSize = true;

            form.ClientSize = new Size(389, 107);
            form.Controls.AddRange(new Control[] { label1, textBox1, label2, textBox2, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label2.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            val1 = textBox1.Text;
            val2 = textBox2.Text;
            return dialogResult;
        }

        /// <summary>
        /// Questo metodo crea un dialog che riceve in input una stringa.
        /// </summary>
        /// <param name="title">Titolo del dialog</param>
        /// <param name="promptText">Testo da scrivere sopra la casella</param>
        /// <param name="value">Valore che verrà modificato dal dialog</param>
        /// <returns></returns>
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
    
}
