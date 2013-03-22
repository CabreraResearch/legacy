using System;
using System.Runtime.Serialization;

namespace ChemSW.Nbt.csw.Mobile
{
    #region Data Contract

    [DataContract]
    public class RapidLoaderData
    {
        [DataContract]
        public class RapidLoaderDataRequest
        {
            [DataMember]
            public String CSVData = String.Empty;
        }
    }

    #endregion Data Contract

    public class CswNbtMobileRapidLoader
    {        
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;

        public CswNbtMobileRapidLoader( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Properties and ctor

        public void saveRapidLoaderData( RapidLoaderData.RapidLoaderDataRequest Request )
        {
            //TODO: 
            //check email - throw error if empty, else:
            //convert csv string to excel format and save file in temp directory (naming it accordingly)
            //send email with a link to the user (and support)
        }
    }
}