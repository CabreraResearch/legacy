using System.ComponentModel;
using System.Runtime.Serialization;

namespace NbtWebApp.WebSvc.Logic
{

    /// <summary>
    /// Object sent with request to update a serial balance
    /// </summary>
    [DataContract]
    [Description( "Update data for a serial balance" )]
    public class SerialBalance
    {
        /// <summary>
        /// NBT Name uniquely identifies the balance to users
        /// </summary>
        [DataMember( IsRequired = true )] 
        [Description( "NBT Name uniquely identifies the balance to users" )] 
        public string NbtName = string.Empty;

        /// <summary>
        /// The current weight on the balance
        /// </summary>
        [DataMember( IsRequired = true )] 
        [Description( "The current weight on the balance" )] 
        public double CurrentWeight = double.MinValue;

        /// <summary>
        /// The unit of measurement for the current weight
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "The unit of measurement for the current weight" )]
        public string UnitOfMeasurement = string.Empty;

        /// <summary>
        /// The manufacturer of the balance
        /// </summary>
        [DataMember( IsRequired = true )] 
        [Description( "The manufacturer of the balance" )] 
        public string Manufacturer = string.Empty;

        /// <summary>
        /// The hardware description of the balance
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "The hardware description of the balance" )]
        public string DeviceDescription = string.Empty;

        /// <summary>
        /// Whether the balance is receiving data on requests
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Whether the balance is receiving data on requests" )] 
        public bool Operational = false;

        ///<summary>
        /// What configuration set the balance is using for serial port
        /// </summary>
        [DataMember( IsRequired = true )] 
        [Description( "What configuration the balance is using for serial port" )] 
        public string Configuration = string.Empty;


    }//class SerialBalance




    /// <summary>
    /// Object used to transfer Balance configuration data
    /// </summary>
    [DataContract]
    [Description( "Object used to transfer Balance configuration data" )]
    public class BalanceConfiguration
    {
        /// <summary>
        /// Unique name of this configuration
        /// </summary>
        [DataMember( IsRequired = true )] 
        [Description( "Unique name of this configuration") ]
        public string Name = string.Empty;

        /// <summary>
        /// Request string sent to query balance
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Request string sent to query balance" )]
        public string RequestFormat = string.Empty;

        /// <summary>
        /// Regular expression to parse weights received from balance
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Regular expression to parse weights received from balance" )]
        public string ResponseFormat = string.Empty;

        /// <summary>
        /// Baud Rate of serial port connection
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Baud Rate of serial port connection" )]
        public int BaudRate = int.MinValue;

        /// <summary>
        /// Parity Bit of serial port connection
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Parity Bit of serial port connection" )]
        public string ParityBit  = string.Empty;

        /// <summary>
        /// Data Bits of serial port connection
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Data bits of serial port connection" )]
        public int DataBits  = int.MinValue;

        /// <summary>
        /// Stop Bits of serial port connection
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Stop Bits of serial port connection" )]
        public string StopBits  = string.Empty;

        /// <summary>
        /// Handshake of serial port connection
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Handshake of serial port connection" )]
        public string Handshake = string.Empty;


    }//class BalanceConfiguration

}
