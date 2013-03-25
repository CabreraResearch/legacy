using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Grid.ExtJs;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.Tree;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.Actions
{




    /// <summary>
    /// Holds logic for handling node quotas
    /// </summary>
    public class CswNbtActAssignInventoryGroups
    {

        private CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtActAssignInventoryGroups( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void assignInventoryGroupToLocations( string InventoryGroupNodeId, string LocationNodeKeys )
        {



            CswPrimaryKey IGKey = new CswPrimaryKey();
            IGKey.FromString( InventoryGroupNodeId );
            CswNbtNode InventoryGroupNode = _CswNbtResources.Nodes[IGKey];
            if( null != InventoryGroupNode )
            {
                foreach( string CurrentLocationKey in LocationNodeKeys.Split( ',' ) )
                {
                    if( string.Empty != CurrentLocationKey )
                    {
                        CswNbtNodeKey LKey = new CswNbtNodeKey( CurrentLocationKey );
                        CswNbtObjClassLocation CurrentLocationNode = _CswNbtResources.Nodes[LKey];
                        if( null != CurrentLocationNode )
                        {
                            CurrentLocationNode.InventoryGroup.RelatedNodeId = InventoryGroupNode.NodeId;
                            CurrentLocationNode.postChanges( true );
                        }
                    }//
                }//iterate locations
            }
            else
            {
                throw( new CswDniException("There is no Inventory Group node corresponding to the key: " + InventoryGroupNodeId ) ); 
            }//if-else the inventory group is valid


        }//assignInventoryGroupToLocations() 

    } // class  CswNbtActAssignInventoryGroups
}// namespace ChemSW.Nbt.Actions