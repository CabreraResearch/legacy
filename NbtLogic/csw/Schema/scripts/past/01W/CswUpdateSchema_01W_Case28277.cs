using System.Collections;
using System.Collections.Generic;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28277
    /// </summary>
    public class CswUpdateSchema_01W_Case28277 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28277; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass FireExtinguisherOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "FireExtinguisherClass" );
            if (null != FireExtinguisherOC)
            {
                /* Remove the remaining object class props */
                IEnumerable<CswNbtMetaDataObjectClassProp> FireExtinguisherOCProps = FireExtinguisherOC.getObjectClassProps();
                foreach (CswNbtMetaDataObjectClassProp FireExtinguisherOCP in FireExtinguisherOCProps)
                {
                    // Remove any rows from jct_nodes_props
                    CswTableUpdate JctNodesPropsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("removeRowsAssocWithFireExtOCP_28277", "jct_nodes_props");
                    DataTable JctNodesPropsDt = JctNodesPropsTU.getTable("where objectclasspropid = " + FireExtinguisherOCP.PropId);
                    foreach (DataRow CurrentDataRow in JctNodesPropsDt.Rows)
                    {
                        CurrentDataRow.Delete();
                    }
                    JctNodesPropsTU.update(JctNodesPropsDt);

                    // Delete the OCP
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp(FireExtinguisherOCP, true);
                }

                /* Remove any rows from jct_modules_objectclass */
                _CswNbtSchemaModTrnsctn.deleteAllModuleObjectClassJunctions(FireExtinguisherOC);
            }

            /* Remove the FireExtinguisherClass from object_class */
            CswTableUpdate ObjectClassTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "removeFireExtinguisherOC_28277", "object_class" );
            DataTable ObjectClassDt = ObjectClassTU.getTable( "where objectclass = 'FireExtinguisherClass'" );
            if( 1 == ObjectClassDt.Rows.Count ) // we should only get one row
            {
                ObjectClassDt.Rows[0].Delete();
            }

            ObjectClassTU.update( ObjectClassDt );

        } //Update()

    }//class CswUpdateSchema_01V_Case28277

}//namespace ChemSW.Nbt.Schema