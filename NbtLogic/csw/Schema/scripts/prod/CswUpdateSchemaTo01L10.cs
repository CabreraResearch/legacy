using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-10
    /// </summary>
    public class CswUpdateSchemaTo01L10 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 10 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24566/24656

            CswNbtMetaDataNodeType InspectionSchedNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            if( null != InspectionSchedNt )
            {
                CswNbtMetaDataNodeTypeProp WarningDaysNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.WarningDaysPropertyName );

                CswNbtMetaDataNodeTypeProp OwnerNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                OwnerNtp.DefaultValue.ClearValue();
                OwnerNtp.HelpText = "Which set of targets (Inspection Points) will be scheduled. Usually by locations or types of items. (ex: Safety Equipment - Fixed)";
                OwnerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, WarningDaysNtp );

                CswNbtMetaDataNodeTypeProp ParentTypeNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
                ParentTypeNtp.DefaultValue.ClearValue();
                ParentTypeNtp.HelpText = "What will be inspected? (ex: Eye Wash Station)";
                ParentTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, OwnerNtp );

                CswNbtMetaDataNodeTypeProp TargetTypeNtp = InspectionSchedNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
                TargetTypeNtp.DefaultValue.ClearValue();
                TargetTypeNtp.HelpText = "What Inspection Design will be used. (ex: Eye Wash Station Check)";
                TargetTypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, ParentTypeNtp );
            }

            /* We used the unfortunate naming conventions "A" and "B". If they still exist on the Master schema's data, fix 'em. */
            CswNbtMetaDataNodeType InspectionGroupNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inspection Group" );
            if( null != InspectionGroupNt )
            {
                foreach( CswNbtNode GroupNode in InspectionGroupNt.getNodes( true, false ) )
                {
                    CswNbtObjClassInspectionTargetGroup NodeAsTargetGroup = CswNbtNodeCaster.AsInspectionTargetGroup( GroupNode );
                    if( NodeAsTargetGroup.Name.Text == "A" )
                    {
                        NodeAsTargetGroup.Name.Text = "Inspection Point Group A";
                        GroupNode.postChanges( true );
                    }
                    else if( NodeAsTargetGroup.Name.Text == "B" )
                    {
                        NodeAsTargetGroup.Name.Text = "Inspection Point Group B";
                        GroupNode.postChanges( true );
                    }
                }
            }

            #endregion Case 24566

            #region Case 24564

            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClassProp TypeOcp = InspectionTargetOc.getObjectClassProp( "Type" );

            Collection<CswNbtMetaDataNodeTypeProp> NodeTypeProps = _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( TypeOcp, false );

            foreach( CswNbtMetaDataNodeTypeProp Prop in NodeTypeProps )
            {
                if( string.IsNullOrEmpty( Prop.ListOptions ) )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( Prop );
                }
            }

            #endregion Case 24564

        }//Update()

    }//class CswUpdateSchemaTo01L10

}//namespace ChemSW.Nbt.Schema


