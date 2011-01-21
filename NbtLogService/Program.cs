using System.ServiceProcess;

namespace CswLogServiceNbt
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ServiceMain() 
			};
            ServiceBase.Run( ServicesToRun );
        }
    }
}
