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


                //Inventory group
                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public bool UpdateInventoryGroup;

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public string SelectedInventoryGroupNodeId;

                //Storage Compatability
                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public bool UpdateStorageCompatability;

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public string SelectedImages;

                //Allow Inventory
                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public bool UpdateAllowInventory;

                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public string AllowInventory;

                //Control Zone
                [DataMember( EmitDefaultValue = true, IsRequired = false )]
                public bool UpdateControlZone;

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
        public static void assignPropsToLocations( ICswResources CswResources, AssignInventoryGroupResponse Response, AssignInventoryGroupData.AssignRequest Request )
        {
            //Request.    
            
            CswNbtActManageLocations cswNbtActManageLocations = new CswNbtActManageLocations( (CswNbtResources) CswResources );
            cswNbtActManageLocations.assignPropsToLocations( Request.LocationNodeKeys, Request.UpdateInventoryGroup, Request.SelectedInventoryGroupNodeId, Request.UpdateAllowInventory, Request.AllowInventory, Request.UpdateControlZone, Request.SelectedControlZoneId, Request.UpdateStorageCompatability, Request.SelectedImages );
        }
        #endregion

    }//CswNbtWebServiceLocations

}//CswNbtWebServiceLocations
