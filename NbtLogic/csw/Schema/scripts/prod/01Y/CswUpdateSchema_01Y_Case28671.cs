using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01Y_Case28671 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 28671; }
        }

        public override void update()
        {

            //CswNbtObjClassMaterial MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            string UNCodeNodeTypeName = "UN Code";
            string ChemicalNodeTypeName = "Chemical";
            string LQNoNodeTypeName = "LQNo";
            string HazardsTabName = "Hazards";

            CswNbtMetaDataNodeType UNCodeNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( UNCodeNodeTypeName );
            if( null != UNCodeNodeType )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( UNCodeNodeType );
            }


            //TO DO: In addition to the other meta data items on the spec, will need to change the field-type 
            //at the object-class prop level from relationship to text.



            CswNbtMetaDataNodeType ChemicaNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChemicalNodeTypeName );

            if( null != ChemicaNodeType )
            {
                //get the Un Code's tab squared away
                //                CswNbtMetaDataNodeTypeProp UnCodeProp = ChemicaNodeType.getNodeTypeProp( UNCodeNodeTypeName );

                CswNbtMetaDataNodeTypeProp UnCodeProp = ChemicaNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.UNCode );

                if( null != UnCodeProp )
                {
                    CswNbtMetaDataNodeTypeTab HazardsTab = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeTab( ChemicaNodeType.NodeTypeId, HazardsTabName );

                    if( null != HazardsTab )
                    {
                        UnCodeProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, HazardsTab.TabId );
                    }//if we have a hazards tab

                }//if we have a uncode 

                //CswNbtMetaDataNodeTypeProp IdTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );

                //CswNbtMetaDataNodeTypeProp ITargetLocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Location );
                //CswNbtMetaDataNodeTypeProp IDesignLocationNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Location );
                //IDesignLocationNtp.SetFK( NbtViewPropIdType.NodeTypePropId.ToString(), IdTargetNtp.PropId, NbtViewPropIdType.NodeTypePropId.ToString(), ITargetLocationNtp.PropId );


                //Add the LQNo prop on Chemical
                CswNbtMetaDataNodeType LQNoNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( LQNoNodeTypeName );
                if( null != LQNoNodeType )
                {
                    CswNbtMetaDataNodeTypeProp LQNoProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicaNodeType, CswNbtMetaDataFieldType.NbtFieldType.Relationship, LQNoNodeType.NodeTypeName, HazardsTabName );
                    LQNoProp.SetFK( NbtViewPropIdType.NodeTypePropId.ToString(), LQNoNodeType.NodeTypeId );

                    CswNbtView LQPNoView = _CswNbtSchemaModTrnsctn.makeView();
                    LQPNoView.saveNew( LQNoNodeTypeName, NbtViewVisibility.Hidden );
                    LQPNoView.ViewMode = NbtViewRenderingMode.Table;
                    LQPNoView.Width = 100;

                    //CswNbtMetaDataNodeType Rel1SecondNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "LQNo" );
                    CswNbtViewRelationship Rel1 = LQPNoView.AddViewRelationship( LQNoNodeType, true );
                    LQPNoView.save();

                    LQNoProp.ViewId = LQPNoView.ViewId;
                }

            }//if we have a chemcial node type




            ///_CswNbtSchemaModTrnsctn.MetaData.remove
        } //Update()

    }//class CswUpdateSchema_01Y_Case28671

}//namespace ChemSW.Nbt.Schema