using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceLocationsCis
    {
        #region DataContract

        [DataContract]
        public class AssignInventoryGroupData
        {
            [DataContract]
            public class AssignRequest
            {

                public AssignRequest() { }

                [DataMember( EmitDefaultValue = true, IsRequired = false  )]
                public string LocationNodeKeys;

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public string SelectedInventoryGroupNodeId;

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public List<string> SelectedImages;

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public bool AllowInventory;

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public string SelectedControlZoneId;
            }


        }//AssignInventoryGroupsData

        [DataContract]
        public class AssignInventoryGroupResponse : CswWebSvcReturn
        {
            [DataMember]
            public string Data;
        }

        #endregion DataContract


        #region Public Methods
        public static void assignInventoryGroupToLocations( ICswResources CswResources, AssignInventoryGroupResponse Response, AssignInventoryGroupData.AssignRequest Request )
        {

            CswNbtActAssignInventoryGroups CswNbtActAssignInventoryGroups = new CswNbtActAssignInventoryGroups( (CswNbtResources) CswResources );
            CswNbtActAssignInventoryGroups.assignInventoryGroupToLocations( Request.SelectedInventoryGroupNodeId, Request.LocationNodeKeys );
        }
        #endregion

    }//CswNbtWebServiceLocations

}//CswNbtWebServiceLocations
