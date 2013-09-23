using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleStatus : CswNbtKioskModeRule
    {
        public CswNbtKioskModeRuleStatus( CswNbtResources NbtResources )
            : base( NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        public override void SetFields( ref OperationData OpData )
        {
            base.SetFields( ref OpData );
            OpData.Field1.Name = "Status:";
            OpData.Field2.Name = "Item:";
        }

        public override void ValidateFieldOne( ref OperationData OpData )
        {
            bool IsValid = _validateStatus( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldOne( ref OpData );
            }
        }

        public override void ValidateFieldTwo( ref OperationData OpData )
        {
            bool IsValid = _validateItem( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldTwo( ref OpData );
            }
        }

        public override void CommitOperation( ref OperationData OpData )
        {
            CswNbtNode item = _CswNbtResources.Nodes[OpData.Field2.NodeId];
            string statusPropName = "Status";
            switch( OpData.Field2.FoundObjClass )
            {
                case CswEnumNbtObjectClass.EquipmentClass:
                    statusPropName = CswNbtObjClassEquipment.PropertyName.Status;
                    break;
                case CswEnumNbtObjectClass.EquipmentAssemblyClass:
                    statusPropName = CswNbtObjClassEquipmentAssembly.PropertyName.Status;
                    break;
            }
            string itemTypeName = item.getNodeType().NodeTypeName;

            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, item.getNodeType() ) && false == item.Properties[statusPropName].ReadOnly )
            {
                item.Properties[statusPropName].AsList.Value = OpData.Field1.Value;
                item.postChanges( false );

                OpData.Log.Add( DateTime.Now + " - Status of " + itemTypeName + " " + OpData.Field2.Value + " changed to \"" + OpData.Field1.Value + "\"" );
                base.CommitOperation( ref OpData );
            }
            else
            {
                string statusMsg = "You do not have permission to edit " + itemTypeName + " (" + OpData.Field2.Value + ")";
                if( OpData.Field2.FoundObjClass.Equals( CswEnumNbtObjectClass.EquipmentClass ) )
                {
                    CswNbtObjClassEquipment nodeAsEquip = item;
                    if( null != nodeAsEquip.Assembly.RelatedNodeId )
                    {
                        CswNbtObjClassEquipmentAssembly assembly = _CswNbtResources.Nodes[nodeAsEquip.Assembly.RelatedNodeId];
                        if( null != assembly )
                        {
                            statusMsg = "Cannot perform STATUS operation on Equipment (" + OpData.Field2.Value + ") when it belongs to Assembly (" + assembly.Barcode.Barcode + ")";
                        }
                    }
                }
                OpData.Field2.FoundObjClass = string.Empty;
                OpData.Field2.StatusMsg = statusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateStatus( ref OperationData OpData )
        {
            bool ret = false;
            string status = OpData.Field1.Value;

            Regex alphNums = new Regex( "[^a-zA-Z0-9]" );
            string strippedStatus = alphNums.Replace( status, "" );

            Collection<CswNbtMetaDataNodeTypeProp> statusNTPs = new Collection<CswNbtMetaDataNodeTypeProp>();

            CswNbtMetaDataObjectClass equipmentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp statusOCP = equipmentOC.getObjectClassProp( CswNbtObjClassEquipment.PropertyName.Status );
            foreach( CswNbtMetaDataNodeTypeProp statusNTP in statusOCP.getNodeTypeProps() )
            {
                statusNTPs.Add( statusNTP );
            }

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentAssemblyClass );
            statusOCP = assemblyOC.getObjectClassProp( CswNbtObjClassEquipmentAssembly.PropertyName.Status );
            foreach( CswNbtMetaDataNodeTypeProp statusNTP in statusOCP.getNodeTypeProps() )
            {
                statusNTPs.Add( statusNTP );
            }

            foreach( CswNbtMetaDataNodeTypeProp statusNTP in statusNTPs )
            {
                CswCommaDelimitedString statusOptCDS = new CswCommaDelimitedString();
                statusOptCDS.FromString( statusNTP.ListOptions );

                foreach( string candidateStatus in statusOptCDS )
                {
                    if( false == ret )
                    {
                        if( string.Equals( candidateStatus, status, StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            ret = true;
                            OpData.Field1.Value = candidateStatus;
                        }
                        else
                        {
                            string strippedCandidateStatus = alphNums.Replace( candidateStatus, "" );
                            if( strippedStatus.Equals( strippedCandidateStatus, StringComparison.CurrentCultureIgnoreCase ) )
                            {
                                ret = true;
                                OpData.Field1.Value = candidateStatus;
                            }
                        }
                    }
                }
            }

            if( false == ret )
            {
                OpData.Field1.ServerValidated = false;
                OpData.Field1.StatusMsg = status + " is not a valid option for a Status Mode scan.";
            }

            return ret;
        }

        private bool _validateItem( ref OperationData OpData )
        {
            bool ret = false;

            CswNbtSearch search = new CswNbtSearch( _CswNbtResources )
            {
                SearchTerm = OpData.Field2.Value
            };
            ICswNbtTree tree = search.Results();

            int childCount = tree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                tree.goToNthChild( i );
                CswNbtNode node = tree.getNodeForCurrentPosition();
                CswNbtMetaDataNodeType nodeType = node.getNodeType();
                CswNbtMetaDataNodeTypeProp barcodeProp = (CswNbtMetaDataNodeTypeProp) nodeType.getBarcodeProperty();

                if( null != barcodeProp )
                {
                    string barcodeValue = node.Properties[barcodeProp].AsBarcode.Barcode;
                    string ObjClass = node.ObjClass.ObjectClass.ObjectClass;

                    if( string.Equals( barcodeValue, OpData.Field2.Value, StringComparison.CurrentCultureIgnoreCase ) )
                    {
                        if( ObjClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.EquipmentAssemblyClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                            ret = true;
                        }

                        if( ObjClass == CswEnumNbtObjectClass.EquipmentClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.EquipmentClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                            ret = true;
                        }
                    }
                }
                tree.goToParentNode();
            }

            if( string.IsNullOrEmpty( OpData.Field2.FoundObjClass ) )
            {
                string StatusMsg = "Could not find " + CswEnumNbtObjectClass.EquipmentClass.Replace( "Class", "" );
                StatusMsg += " or " + CswEnumNbtObjectClass.EquipmentAssemblyClass.Replace( "Class", "" ) + " with barcode " + OpData.Field2.Value;

                OpData.Field2.StatusMsg = StatusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + StatusMsg );
                ret = false;
            }

            return ret;
        }

        #endregion
    }
}
