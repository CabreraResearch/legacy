using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26524
    /// </summary>
    public class CswUpdateSchemaCase26524 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Remove 'Deficient Issues last 30 days' Report
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            IEnumerable<CswNbtNode> ReportNodes = ReportOC.getNodes( false, true );
            foreach( CswNbtNode Node in ReportNodes )
            {
                string nodeName = CswNbtNodeCaster.AsReport( Node ).ReportName.Text;
                if(nodeName == "Deficient Issues last 30 days")
                {
                    Node.delete();
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase26524

}//namespace ChemSW.Nbt.Schema