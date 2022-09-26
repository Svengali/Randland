using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rl; 

internal static class Program
{
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		log.create( $"logs/randland.log", log.Endpoints.Console | log.Endpoints.File );

		log.high( $"Starting up in {Directory.GetCurrentDirectory()}" );

		Application.EnableVisualStyles( );
		Application.SetCompatibleTextRenderingDefault( false );
		Application.Run( new RandlandForm( ) );
	}
}
