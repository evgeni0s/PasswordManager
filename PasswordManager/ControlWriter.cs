using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI
{
    public class ControlWriter : TextWriter
    {
        private TextBox textBox;

        public ControlWriter(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public override void Write(char value)
        {
            textBox.AppendText(value.ToString());
        }

        public override void Write(string value)
        {
            textBox.AppendText(value);
        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.ASCII;
            }
        }

    }
}
