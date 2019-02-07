using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROFLPlayer
{
    public partial class ExecAddForm : Form
    {
        public ExecAddForm()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            this.ExecNameTextBox.AutoSize = false;
            this.ExecNameTextBox.Size = new Size(227, 23);

            this.ExecTargetTextBox.AutoSize = false;
            this.ExecTargetTextBox.Size = new Size(227, 23);

            this.ExecStartTextBox.AutoSize = false;
            this.ExecStartTextBox.Size = new Size(227, 23);
        }
    }
}
