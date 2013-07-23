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
        /// The configured request string for the balance
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "The configured request string for the balance" )]
        public string RequestFormat = string.Empty;

        /// <summary>
        /// The configured response string for the balance
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "The configured response string for the balance" )]
        public string ResponseFormat = string.Empty;

        /// <summary>
        /// Whether the balance is receiving data on requests
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Whether the balance is receiving data on requests" )] 
        public bool Operational = false;










    }

}
