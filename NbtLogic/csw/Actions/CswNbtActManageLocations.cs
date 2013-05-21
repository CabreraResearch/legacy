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
    public class CswNbtActManageLocations
    {

        private CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtActManageLocations( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void assignPropsToLocations( string LocationNodeKeys, bool UpdateInventoryGroup, string SelectedInventoryGroupNodeId, bool UpdateAllowInventory, string AllowInventory, bool UpdateControlZone, string SelectedControlZoneNodeId, bool UpdateStorageCompatability, string SelectedImages )
        {

            if( false == string.IsNullOrEmpty( LocationNodeKeys ) )
            {


                ///we don't pre-load he allowinventory value because there's no extra expense
                ///to doing so repeatedly in the loop

                CswNbtNode InventoryGroupNode = null;
                if( ( true == UpdateInventoryGroup ) && ( false == string.IsNullOrEmpty( SelectedInventoryGroupNodeId ) ) )
                {
                    CswPrimaryKey IGKey = new CswPrimaryKey();
                    IGKey.FromString( SelectedInventoryGroupNodeId );
                    InventoryGroupNode = _CswNbtResources.Nodes[IGKey];
                }

                CswNbtNode ControlZoneNode = null;
                if( ( true == UpdateControlZone ) && ( false == string.IsNullOrEmpty( SelectedControlZoneNodeId ) ) )
                {
                    CswPrimaryKey IGKey = new CswPrimaryKey();
                    IGKey.FromString( SelectedControlZoneNodeId );
                    ControlZoneNode = _CswNbtResources.Nodes[IGKey];
                }



                CswDelimitedString Images = new CswDelimitedString( ',' );
                if( true == UpdateStorageCompatability )
                {
                    if( false == string.IsNullOrEmpty( SelectedImages ) )
                    {
                        Images.FromString( SelectedImages );
                    }
                }



                foreach( string CurrentLocationKey in LocationNodeKeys.Split( ',' ) )
                {
                    if( false == string.IsNullOrEmpty( CurrentLocationKey ) )
                    {
                        CswNbtNodeKey LKey = new CswNbtNodeKey( CurrentLocationKey );
                        CswNbtObjClassLocation CurrentLocationNode = _CswNbtResources.Nodes[LKey];
                        if( null != CurrentLocationNode )
                        {

                            if( true == UpdateInventoryGroup )
                            {
                                if( null != InventoryGroupNode )
                                {
                                    CurrentLocationNode.InventoryGroup.RelatedNodeId = InventoryGroupNode.NodeId;
                                }
                                else
                                {
                                    CurrentLocationNode.InventoryGroup.RelatedNodeId = null;
                                }
                            }

                            if( true == UpdateControlZone )
                            {
                                if( null != ControlZoneNode )
                                {
                                    CurrentLocationNode.ControlZone.RelatedNodeId = ControlZoneNode.NodeId;
                                }
                                else
                                {
                                    CurrentLocationNode.ControlZone.RelatedNodeId = null;
                                }
                            }
                            

                            if( UpdateAllowInventory )
                            {
                                CurrentLocationNode.AllowInventory.Checked = CswConvert.ToTristate( AllowInventory );
                            }

                            if( UpdateStorageCompatability )
                            {
                                CurrentLocationNode.StorageCompatibility.Value = Images;
                            }

                            CurrentLocationNode.postChanges( true );

                        }//if current key yielded a node

                    } //if there is a location keye

                } //iterate locations


            }//if we have location keys

        }//assignInventoryGroupToLocations() 

    } // class  CswNbtActAssignInventoryGroups
}// namespace ChemSW.Nbt.Actions