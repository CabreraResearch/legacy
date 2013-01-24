using System.Runtime.Serialization;
using ChemSW.Nbt.Actions;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceRegulatoryReporting
    {
        #region DataContract

        /// <summary>
        /// Return Object for HMIS Data
        /// </summary>
        [DataContract]
        public class HMISDataReturn : CswWebSvcReturn
        {
            public HMISDataReturn()
            {
                Data = new HMISData();
            }
            [DataMember]
            public HMISData Data;
        }

        /// <summary>
        /// Return Object for TierII Data
        /// </summary>
        [DataContract]
        public class TierIIDataReturn : CswWebSvcReturn
        {
            public TierIIDataReturn()
            {
                Data = new TierIIData();
            }
            [DataMember]
            public TierIIData Data;
        }

        #endregion DataContract

        #region Public

        /// <summary>
        /// Gets all reportable hazardous Materials and their total quantities in a given Control Zone
        /// </summary>
        public static void getHMISData( ICswResources CswResources, HMISDataReturn Return, HMISData.HMISDataRequest Request )
        {
            CswNbtActHMISReporting _CswNbtActHMISReporting = new CswNbtActHMISReporting( (CswNbtResources) CswResources );
            Return.Data = _CswNbtActHMISReporting.getHMISData( Request );
        }

        /// <summary>
        /// Gets all reportable Materials and their max and average quantities in a given Location for the given timeframe
        /// </summary>
        public static void getTierIIData( ICswResources CswResources, TierIIDataReturn Return, TierIIData.TierIIDataRequest Request )
        {
            CswNbtActTierIIReporting _CswNbtActTierIIReporting = new CswNbtActTierIIReporting( (CswNbtResources) CswResources );
            Return.Data = _CswNbtActTierIIReporting.getTierIIData( Request );
        }

        #endregion Public
    }
}
