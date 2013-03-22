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

                [DataMember( EmitDefaultValue = true, IsRequired = true  )]
                public string LocationNodeKeys;

                [DataMember( EmitDefaultValue = true, IsRequired = true  )]
                public string InventoryGroupNodeId;
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
            CswNbtActAssignInventoryGroups.assignInventoryGroupToLocations( Request.InventoryGroupNodeId, Request.LocationNodeKeys );
        }
        #endregion

    }//CswNbtWebServiceLocations

}//CswNbtWebServiceLocations
