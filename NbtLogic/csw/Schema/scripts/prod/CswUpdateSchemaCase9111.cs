using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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
            _CswNbtSchemaModTrnsctn.addTable( LogFileName, LogFileNamePk );


            foreach( ChemSW.Log.LegalAttribute CurrentLegalAttribute in Enum.GetValues( typeof( ChemSW.Log.LegalAttribute ) ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( LogFileName, CurrentLegalAttribute.ToString(), "The " + CurrentLegalAttribute.ToString() + " ", false, false, 32 );
            }


            //bulk loader complains about PKs, so we need to nuke pk column
            _CswNbtSchemaModTrnsctn.dropColumn( LogFileName, LogFileNamePk );

        }//Update()

    }//class CswUpdateSchemaCase9111

}//namespace ChemSW.Nbt.Schema