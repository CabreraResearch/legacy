using System.Runtime.Serialization;
using NbtWebApp.WebSvc.Logic.Labels;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Serial Balance Return Object
    /// </summary>
    [DataContract]
    public class CswNbtBalanceReturn : CswWebSvcReturn
    {
        
    }




    public class CswNbtWebServiceSerialBalance
    {

        public static void UpdateBalanceData( ICswResources CswResources, CswNbtBalanceReturn Return, SerialBalance Request )
        {
            
        }

    }
}