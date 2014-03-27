using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52017B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 52017; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }


        public override string Title
        {
            get { return "Migrate Biological NTPs to new Object Class"; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType BiologicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Biological" );
            if( null != BiologicalNT )
            {
                _migrateBiologicalProps( BiologicalNT );
                _cleanupNTPrecords( BiologicalNT );
            }
        } // update()


        private void _migrateBiologicalProps( CswNbtMetaDataNodeType BiologicalNT )
        {
            CswNbtMetaDataObjectClass BiologicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BiologicalClass );
            CswNbtObjClassDesignNodeType BiologicalDNT = BiologicalNT.DesignNode;

            //assign the Biological NT to the Biological OC. This will automatically force a re-sync of props
            BiologicalDNT.changeParentObjectClass( BiologicalOC );

            //we need to create the nodetype props now that the object class is correct
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
            //add Physical State to the Add layout
            CswNbtMetaDataNodeTypeProp StateNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( BiologicalNT.NodeTypeId, CswNbtObjClassBiological.PropertyName.PhysicalState );
            StateNTP.updateLayout( CswEnumNbtLayoutType.Add, false, DisplayRow: 1 );
        }

        private void _cleanupNTPrecords( CswNbtMetaDataNodeType BiologicalNT )
        {
            CswNbtMetaDataObjectClass BiologicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BiologicalClass );

            //the design nodetypeprops don't track OCP, but the NTP table does, so we have to update it manually
            CswTableUpdate NTPTable = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "52017B_NTP", "nodetype_props" );
            DataTable NTPRows = NTPTable.getTable( "where nodetypeid=" + BiologicalNT.NodeTypeId + " and objectclasspropid not in (select objectclasspropid from object_class_props where objectclassid = " + BiologicalOC.ObjectClassId + ")");

            foreach( DataRow Row in NTPRows.Rows )
            {
                CswNbtMetaDataObjectClassProp Prop = BiologicalOC.getObjectClassProp( CswConvert.ToString( Row["propname"] ) );
                if( null != Prop )
                {
                    Row["objectclasspropid"] = BiologicalOC.getObjectClassProp( CswConvert.ToString( Row["propname"] ) ).ObjectClassPropId;
                }
            }

            NTPTable.update( NTPRows );
        }
    }

}//namespace ChemSW.Nbt.Schema