using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30043_WorkUnits : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30043; }
        }

        public override void update()
        {
            // Case 30043 - CAF Migration: Sites/Locations/Work Units

            // View code
            //select w.*, s.sitename || ' ' || b.businessunitname as workunitname from work_units w
            //left outer join business_units b on (b.businessunitid = w.businessunitid)
            //left outer join sites s on (s.siteid = w.siteid)

            CswNbtSchemaUpdateImportMgr importMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "work_units", "Work Unit", "workunits_view" );

            // Binding
            importMgr.importBinding( "workunitname", CswNbtObjClassWorkUnit.PropertyName.Name, "" );

            // Relationship
            //none

            importMgr.finalize();

            // Columns in work_units table
            //deleted
            //retestwarndays
            //workunitid
            //siteid
            //businessunitid
            //stdapprovalmode
            //skiplotcodedefault
            //expiryinterval
            //retestintervaldefault
            //messagetext
            //expiryintervalunits
            //retaincount
            //retainkeepyears
            //retainquantity
            //retainunitofmeasureid
            //mininventorylevel
            //mininventoryunitofmeasureid
            //homeinventorygroupid
            //stdexpiryinterval
            //stdexpiryintervalunits
            //autolotapproval
            //samplecollectionrequired
            //dispense_percent
            //canorderdraft
            //canoverreq
            //cansynchcontainers
            //srireviewgroupid
            //def_reqaschild
            //amountenable
            //alloc1
            //alloc2
            //alloc3
            //alloc4
            //alloc5
            //removegroupondispense
            //removegrouponmove

        } // update()

    } // class CswUpdateSchema_02F_Case30043_WorkUnits

}//namespace ChemSW.Nbt.Schema