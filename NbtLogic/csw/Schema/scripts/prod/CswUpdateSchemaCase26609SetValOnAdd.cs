
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;



namespace ChemSW.Nbt.Schema
{

    /// <summary>
    /// Schema Update for case 26609
    /// </summary>
    public class CswUpdateSchemaCase26609SetValOnAdd : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RptOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            foreach( CswNbtMetaDataNodeType CurrentNodeType in RptOC.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ReportUserNameProp = CurrentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassReport.ReportUserNamePropertyName );
                ReportUserNameProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                CswNbtMetaDataNodeTypeProp FormattedSqlProp = CurrentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassReport.FormattedSqlPropertyName );
                FormattedSqlProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

            }

        }//Update()

    }//class CswUpdateSchemaCase26609ReportUserName

}//namespace ChemSW.Nbt.Schema