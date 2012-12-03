using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 9111
    /// </summary>
    public class CswUpdateSchemaCase9111 : CswUpdateSchemaTo
    {
        public override void update()
        {
            string LogFileName = "logmessages";
            string LogFileNamePk = "logmessageid";
            string SequenceName = LogFileNamePk + "_seq";
            string TriggerName = LogFileNamePk + "_trig";
            _CswNbtSchemaModTrnsctn.addTable( LogFileName, LogFileNamePk );


            foreach( ChemSW.Log.LegalAttribute CurrentLegalAttribute in Enum.GetValues( typeof( ChemSW.Log.LegalAttribute ) ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( LogFileName, CurrentLegalAttribute.ToString(), "The " + CurrentLegalAttribute.ToString() + " ", false, false, 32 );
            }

            //The bulk loader will hurl on a non-unique pk value unless have a trigger to add a PK val the old-fashioned way
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "create sequence " + SequenceName + " start with 1 increment by 1" );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "create trigger " + TriggerName + " before insert on " + LogFileName + " for each row begin select " + SequenceName + ".nextval into :new." + LogFileNamePk + " from dual; end;" );



        }//Update()

    }//class CswUpdateSchemaCase9111

}//namespace ChemSW.Nbt.Schema