using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25374
    /// </summary>
    public class CswUpdateSchemaCase25374 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Remove Report.View property
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp ReportViewOCP = ReportOC.getObjectClassProp( "View" );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( ReportViewOCP, true );

            // Remove old (defunct) reports
            Collection<CswNbtNode> DoomedNodes = new Collection<CswNbtNode>();
            foreach( CswNbtNode ReportNode in ReportOC.getNodes( false, true ) )
            {
                if( ReportNode.NodeName == "Due Tasks" ||
                    ReportNode.NodeName == "Equipment Detail" ||
                    ReportNode.NodeName == "Open Problems" ||
                    ReportNode.NodeName == "Task Work Order" )
                {
                    DoomedNodes.Add( ReportNode );
                }
            }
            foreach( CswNbtNode DoomedNode in DoomedNodes )
            {
                DoomedNode.delete();
            }


        }//Update()

    }//class CswUpdateSchemaCase25374

}//namespace ChemSW.Nbt.Schema