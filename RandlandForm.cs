using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rl;

public partial class RandlandForm : Form
{
	public RandlandForm()
	{
		InitializeComponent( );
	}

	private void Main_Load( object sender, EventArgs e )
	{

	}

	private void newMapToolStripMenuItem_Click( object sender, EventArgs e )
	{
		var mapForm = new MapForm();
		mapForm.Show( this );
	}
}
