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
            CswNbtMetaDataNodeType UNCodeNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( UNCodeNodeTypeName );
            if( null != UNCodeNodeType )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( UNCodeNodeType );
            }


            //TO DO: In addition to the other meta data items on the spec, will need to change the field-type 
            //at the object-class prop level from relationship to text.


            //string ChemicalNodeTypeName = "Chemical";
            //string LQNoNodeTypeName = "LQNo";
            //string HazardsTabName = "Hazards";

            //CswNbtMetaDataNodeType ChemicaNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChemicalNodeTypeName );

            //if( null != ChemicaNodeType )
            //{
            //    CswNbtMetaDataNodeType UNCodeNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( UNCodeNodeTypeName );
            //    if( null != UNCodeNodeType )
            //    {
            //        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( UNCodeNodeType );
            //    }

            //    CswNbtMetaDataNodeTypeProp OldUnCodeProp = ChemicaNodeType.getNodeTypeProp( UNCodeNodeTypeName );
            //    if( null != OldUnCodeProp )
            //    {
            //        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( OldUnCodeProp );
            //    }


            //    CswNbtMetaDataNodeTypeProp NewUnCodeProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicaNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, UNCodeNodeTypeName, HazardsTabName );


            //    CswNbtMetaDataNodeType LQNoNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( LQNoNodeTypeName );
            //    if( null != LQNoNodeType )
            //    {
            //        CswNbtMetaDataNodeTypeProp LQNoProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicaNodeType, CswNbtMetaDataFieldType.NbtFieldType.Relationship, LQNoNodeType.NodeTypeName, HazardsTabName );
            //        LQNoProp.SetFK( NbtViewPropIdType.NodeTypePropId.ToString(), LQNoNodeType.NodeTypeId );
            //    }

            //}//if we have a chemcial node type



            ///_CswNbtSchemaModTrnsctn.MetaData.remove
        } //Update()

    }//class CswUpdateSchema_01Y_Case28671

}//namespace ChemSW.Nbt.Schema