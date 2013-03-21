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
    public class CswNbtKioskModeRuleStatus: CswNbtKioskModeRule
    {
        public CswNbtResources _CswNbtResources;

        public CswNbtKioskModeRuleStatus( CswNbtResources NbtResources )
            : base( NbtResources )
        {
            _CswNbtResources = NbtResources;
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
            CswNbtNode item = _getNodeByBarcode( OpData.Field2.FoundObjClass, OpData.Field2.Value, false );
            string statusPropName = "Status";
            switch( OpData.Field2.FoundObjClass )
            {
                case NbtObjectClass.EquipmentClass:
                    statusPropName = CswNbtObjClassEquipment.PropertyName.Status;
                    break;
                case NbtObjectClass.EquipmentAssemblyClass:
                    statusPropName = CswNbtObjClassEquipmentAssembly.PropertyName.Status;
                    break;
            }
            string itemTypeName = item.getNodeType().NodeTypeName;

            if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, item.getNodeType() ) && false == item.Properties[statusPropName].ReadOnly )
            {
                item.Properties[statusPropName].AsList.Value = OpData.Field1.Value;
                item.postChanges( false );

                OpData.Log.Add( DateTime.Now + " - Status of " + itemTypeName + " " + OpData.Field2.Value + " changed to \"" + OpData.Field1.Value + "\"" );
                base.CommitOperation( ref OpData );
            }
            else
            {
                string statusMsg = "You do not have permission to edit " + itemTypeName + " (" + OpData.Field2.Value + ")";
                OpData.Field2.StatusMsg = statusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateStatus( ref OperationData OpData )
        {
            bool ret = false;
            string foundMatch = "";
            string status = OpData.Field1.Value;

            Regex alphNums = new Regex( "[^a-zA-Z0-9]" );
            string strippedStatus = alphNums.Replace( status, "" );

            Collection<CswNbtMetaDataNodeTypeProp> statusNTPs = new Collection<CswNbtMetaDataNodeTypeProp>();

            CswNbtMetaDataObjectClass equipmentOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp statusOCP = equipmentOC.getObjectClassProp( CswNbtObjClassEquipment.PropertyName.Status );
            foreach( CswNbtMetaDataNodeTypeProp statusNTP in statusOCP.getNodeTypeProps() )
            {
                statusNTPs.Add( statusNTP );
            }

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass );
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
                    if( String.IsNullOrEmpty( foundMatch ) )
                    {
                        if( candidateStatus.Equals( status ) )
                        {
                            foundMatch = candidateStatus;
                            ret = true;
                        }
                        else
                        {
                            string strippedCandidateStatus = alphNums.Replace( candidateStatus, "" );
                            if( strippedStatus.Equals( strippedCandidateStatus ) )
                            {
                                foundMatch = candidateStatus;
                                ret = true;
                                OpData.Field1.Value = candidateStatus;
                                OpData.Field1.SecondValue = "(entered: \"" + status + "\")";
                            }
                        }
                    }
                }
            }

            if( String.IsNullOrEmpty( foundMatch ) )
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
                string ObjClass = node.ObjClass.ObjectClass.ObjectClass;

                if( ObjClass == NbtObjectClass.EquipmentAssemblyClass )
                {
                    OpData.Field2.FoundObjClass = NbtObjectClass.EquipmentAssemblyClass;
                    ret = true;
                }

                if( ObjClass == NbtObjectClass.EquipmentClass )
                {
                    OpData.Field2.FoundObjClass = NbtObjectClass.EquipmentClass;
                    ret = true;
                }
                tree.goToParentNode();
            }

            if( null == OpData.Field2.FoundObjClass )
            {
                string StatusMsg = "Could not find " + NbtObjectClass.EquipmentClass.Replace( "Class", "" );
                StatusMsg += " or " + NbtObjectClass.EquipmentAssemblyClass.Replace( "Class", "" ) + " with barcode " + OpData.Field2.Value;

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
