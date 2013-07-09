using System.ComponentModel;
using System.Runtime.Serialization;

namespace NbtWebApp.WebSvc.Logic.Labels
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

    }


}
