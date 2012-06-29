using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;



namespace ChemSW.Nbt.Schema
{

    /// <summary>
    /// Schema Update for case 26609
    /// </summary>
    public class CswUpdateSchemaCase26609UpdateHiddenProp : CswUpdateSchemaTo
    {
        public override void update()
        {



            CswNbtMetaDataObjectClass RptOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );

            Collection<CswNbtNode> CurrentReportNodes = RptOC.getNodes();
            foreach( CswNbtNode CurrentNode in CurrentReportNodes )
            {
                CurrentNode.Properties[CswNbtObjClassReport.ReportUserNamePropertyName].Hidden = true;
            }

        }//Update()

    }//class CswUpdateSchemaCase26609UpdateHiddenProp

}//namespace ChemSW.Nbt.Schema