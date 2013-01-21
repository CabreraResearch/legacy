using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Conversion;

namespace ChemSW.Nbt.Actions
{
    #region DataContract

    [DataContract]
    public class TierIIData
    {
        public TierIIData()
        {
            Materials = new Collection<TierIIMaterial>();
        }

        [DataMember]
        public Collection<TierIIMaterial> Materials;

        [DataContract]
        public class TierIIMaterial
        {
            //Internal
            [DataMember]
            public String MaterialId = String.Empty;
            //Chemical Description
            [DataMember]
            public String TradeName = String.Empty;
            [DataMember]
            public String CASNo = String.Empty;
            [DataMember]
            public String MaterialType = String.Empty;//Pure,Mixture
            [DataMember]
            public String PhysicalState = String.Empty;//Solid,Liquid,Gas
            [DataMember]
            public String SpecialFlags = String.Empty;//EHS,Not Reportable (?)
            //Physical and Health Hazards
            [DataMember]
            public String HazardCategories = String.Empty;//Fire,Pressure,Reactive,Immediate,Delayed
            //Inventory
            [DataMember]
            public Double MaxQty = 0.0;
            [DataMember]
            public Double AverageQty = 0.0;
            [DataMember]
            public Int32 DaysOnSite = 0;
            //Storage Codes and Locations
            [DataMember]
            public String UseTypes = String.Empty;//Storage,Closed,Open
            [DataMember]
            public String Pressures = String.Empty;//Atmos,Subatmos,Pressurized
            [DataMember]
            public String Temperatures = String.Empty;//RT,>RT,<RT,Cryogenic
            [DataMember]
            public String Locations = String.Empty;
        }

        [DataContract]
        public class TierIIDataRequest
        {
            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public bool IncludeChildLocations = false;
            [DataMember]
            public String StartDate = String.Empty;
            [DataMember]
            public String EndDate = String.Empty;
        }

    } // HMISData

    #endregion DataContract

    public class CswNbtActTierIIReporting
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private TierIIData Data;
        private CswPrimaryKey LocationId;

        public CswNbtActTierIIReporting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new TierIIData();
        }

        #endregion Properties and ctor

        #region Public Methods

        public TierIIData getTierIIData( TierIIData.TierIIDataRequest Request )
        {
            //TODO -
            /// grab all the rows in the tierII table for the given location (and possibly child locations?) and start/end date
            /// for each unique material in resulting dataset:
            /// -get the material type, physical state, specialflags, and hazard categories (instance the node or direct query?)
            /// -calculate the max and avg qty from the dataset rows, along with number of days on site 
            ///  (i.e. - number of rows in dataset for the given location ) - this may be more challenging if child locations can be included
            /// -get container storage data and locations respectively by querying the audit table
            /// -add all of this data to a TierIIMaterial object and add that to the return Data
            LocationId = CswConvert.ToPrimaryKey( Request.LocationId );

            TierIIData.TierIIMaterial NewMaterial = new TierIIData.TierIIMaterial
            {
                //Material = MaterialName, 
                //HazardClass = HazardClass,
                //HazardCategory = HMISMaterial.HazardCategory,
                //Class = HMISMaterial.Class,
                //PhysicalState = MaterialNode.PhysicalState.Value,
                //SortOrder = HMISMaterial.SortOrder
            };
                                            
            Data.Materials.Add( NewMaterial );
                                        
            return Data;
        }

        #endregion Public Methods

        #region Private Methods

        

        #endregion Private Methods
    }
}
