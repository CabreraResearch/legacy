using ChemSW.MtSched.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_ScheduledRuleImport : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30041_ScheduledRuleImport"; }
        }

        public override void update()
        {
            // Scheduled rule for CAFImports
            // NOTE: Only do this once!
            _CswNbtSchemaModTrnsctn.createScheduledRule( CswEnumNbtScheduleRuleNames.CAFImport, CswEnumRecurrence.NHours, 1 );

            /*
            
            //Execute as sys to create CAF tablespaces
            create tablespace users_lob
            logging
            datafile 'E:\tablespace\users_idx.dbf'
            size 3072m
            autoextend on
            extent management local;
            
            create tablespace users_idx
            logging
            datafile 'E:\tablespace\users_idx.dbf'
            size 3072m
            autoextend on
            extent management local;

            create tablespace data01s
            logging
            datafile 'E:\tablespace\data01s.dbf'
            size 300072m
            autoextend on
            extent management local;

            grant create public database link to nbt;

            create public database link CAFLINK
             connect to cafimport identified by cafimport
             using 'w2008x64db';
 
             create table nbtimportqueue (
               nbtimportqueueid number(12) NOT NULL PRIMARY KEY,
               state varchar(1),
               itempk number(12) NOT NULL,
               tablename varchar(50) NOT NULL,
               priority number(12),
               errorlog varchar(4000)
             );
 
             create unique index unqidx_nbtimportqueue on NBTIMPORTQUEUE (state, itempk, tablename);
 
             create sequence seq_nbtimportqueueid start with 1 increment by 1;
             commit; 
             
             */


        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema