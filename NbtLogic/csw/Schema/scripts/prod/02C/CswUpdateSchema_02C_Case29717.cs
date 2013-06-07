using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29717
    /// </summary>
    public class CswUpdateSchema_02C_Case29717 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29717; }
        }

        private CswNbtMetaDataNodeType GHSPhraseNT;

        public override void update()
        {
            CswNbtMetaDataObjectClass GHSPhraseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            GHSPhraseNT = GHSPhraseOC.FirstNodeType;
            foreach( CswNbtObjClassGHSPhrase GHSPhraseNode in GHSPhraseOC.getNodes( false, true ) )
            {
                if( GHSPhraseNode.Code.Text.StartsWith( "H4" ) )
                {
                    GHSPhraseNode.Category.Value = "Environmental";
                    GHSPhraseNode.postChanges( false );
                }
            }

            _addGHSPhrase( "P210", "Keep away from heat/sparks/open flames/hot surfaces. - No smoking." );
            _addGHSPhrase( "P261", "Avoid breathing dust/ fume/ gas/ mist/ vapours/ spray." );
            _addGHSPhrase( "P301+310", "IF SWALLOWED : Immediately call a POISON CENTER or doctor/physician" );
            _addGHSPhrase( "P301+312", "IF SWALLOWED : Call a POISON CENTER or doctor/physician if you feel unwell" );
            _addGHSPhrase( "P301+330+331", "IF SWALLOWED : Rinse mouth. Do NOT induce vomiting" );
            _addGHSPhrase( "P302+334", "IF ON SKIN : Immerse in cool water/wrap in wet bandages" );
            _addGHSPhrase( "P302+350", "IF ON SKIN : Gently wash with soap and water" );
            _addGHSPhrase( "P302+352", "IF ON SKIN : Wash with soap and water" );
            _addGHSPhrase( "P303+361+353", "IF ON SKIN (or hair) : Remove/Take off immediately all contaminated clothing. Rinse skin with water/shower" );
            _addGHSPhrase( "P304+312", "IF INHALED : Call a POISON CENTER or doctor/physician if you feel unwell" );
            _addGHSPhrase( "P304+340", "IF INHALED : Remove victim to fresh air and keep at rest in a position comfortable for breathing" );
            _addGHSPhrase( "P304+341", "IF INHALED : If breathing is difficult, remove victim to fresh air and keep at rest in a position comfortable for breathing" );
            _addGHSPhrase( "P305+351+338", "IF IN EYES : Rinse continuously with water for several minutes. Remove contact lenses if present and easy to do... continue rinsing" );
            _addGHSPhrase( "P306+360", "IF ON CLOTHING : Rinse immediately contaminated clothing and skin with plenty of water before removing clothes" );
            _addGHSPhrase( "P307+311", "IF exposed : Call a POISON CENTER or doctor/physician" );
            _addGHSPhrase( "P308+313", "IF exposed or concerned : Get medical advice/attention" );
            _addGHSPhrase( "P309+311", "IF exposed or you feel unwell : Call a POISON CENTER or doctor/physician" );
            _addGHSPhrase( "P332+313", "IF skin irriration occurs : Get medical advice/attention" );
            _addGHSPhrase( "P333+313", "If skin irritation or a rash occurs : Get medical advice/attention" );
            _addGHSPhrase( "P335+334", "Brush off loose particles from skin. Immerse in cool water/wrap in wet bandages" );
            _addGHSPhrase( "P337+313", "Get medical advice/attention" );
            _addGHSPhrase( "P341", "If breathing is difficult, remove victim to fresh air and keep at rest in a position comfortable for breathing" );
            _addGHSPhrase( "P342", "If experiencing respiratory symptoms:" );
            _addGHSPhrase( "P342+311", "Call a POISON CENTER or doctor/physician" );
            _addGHSPhrase( "P350", "Gently wash with soap and water" );
            _addGHSPhrase( "P351", "Rinse continuously with water for several minutes" );
            _addGHSPhrase( "P352", "Wash with soap and water" );
            _addGHSPhrase( "P353", "Rinse skin with water/shower" );
            _addGHSPhrase( "P360", "Rinse immediately contaminated clothing and skin with plenty of water before removing clothes" );
            _addGHSPhrase( "P361", "Remove/Take off immediately all contaminated clothing" );
            _addGHSPhrase( "P362", "Take off contaminated clothing and wash before reuse" );
            _addGHSPhrase( "P363", "Wash contaminated clothing before reuse" );
            _addGHSPhrase( "P370", "In case of fire:" );
            _addGHSPhrase( "P370+376", "In case of fire : Stop leak if safe to do so" );
            _addGHSPhrase( "P370+378", "In case of fire : Use ... for extinction" );
            _addGHSPhrase( "P370+380", "In case of fire : Evacuate area" );
            _addGHSPhrase( "P370+380+375", "In case of fire : Evacuate area. Fight fire remotely due to the risk of explosion" );
            _addGHSPhrase( "P371", "In case of major fire and large quantities:" );
            _addGHSPhrase( "P371+380+375", "In case of major fire and large quantities : Evacuate area. Fight fire remotely due to the risk of explosion" );
            _addGHSPhrase( "P372", "Explosion risk in case of fire" );
            _addGHSPhrase( "P373", "DO NOT fight fire when fire reaches explosives" );
            _addGHSPhrase( "P374", "Fight fire with normal precautions from a reasonable distance" );
            _addGHSPhrase( "P375", "Fight fire remotely due to the risk of explosion" );
            _addGHSPhrase( "P376", "Stop leak if safe to do so" );
            _addGHSPhrase( "P377", "Leaking gas fire... do not extinguish unless leak can be stopped safely" );
            _addGHSPhrase( "P378", "Use... for extinction" );
            _addGHSPhrase( "P380", "Evacuate area" );
            _addGHSPhrase( "P381", "Eliminate all ignition sources if safe to do so" );
            _addGHSPhrase( "P401", "Store ..." );
            _addGHSPhrase( "P402", "Store in a dry place" );
            _addGHSPhrase( "P402+404", "Store in a dry place. Store in a closed container" );
            _addGHSPhrase( "P403", "Store in a well ventilated place" );
            _addGHSPhrase( "P403+233", "Store in a well ventilated place. Keep container tightly closed" );
            _addGHSPhrase( "P403+235", "Store in a well ventilated place. Keep cool" );
            _addGHSPhrase( "P404", "Store in a closed container" );
            _addGHSPhrase( "P405", "Store locked up" );
            _addGHSPhrase( "P406", "Store in a corrosive resistant/... container with a resistant inner liner" );
            _addGHSPhrase( "P407", "Maintain air gap between stacks/pallets" );
            _addGHSPhrase( "P410", "Protect from sunlight" );
            _addGHSPhrase( "P410+403", "Protect from sunlight. Store in a well ventilated place" );
            _addGHSPhrase( "P410+412", "Protect from sunlight. Do not expose to temperatures exceeding 50 ?C/122 ?F" );
            _addGHSPhrase( "P411", "Store at temperatures not exceeding... ?C/... ?F" );
            _addGHSPhrase( "P411+235", "Store at temperatures not exceeding... ?C/... ?F. Keep cool" );
            _addGHSPhrase( "P412", "Do not expose to temperatures exceeding 50 ?C/122 ?F" );
            _addGHSPhrase( "P420", "Store away from other materials" );
            _addGHSPhrase( "P422", "Store contents under..." );
            _addGHSPhrase( "P501", "Dispose of contents/container to..." );
            _addGHSPhrase( "P502", "Refer to manufacturer/supplier for information on recovery/recycling" );

        } // update()

        private void _addGHSPhrase( String Code, String English )
        {
            CswNbtObjClassGHSPhrase GHSPhraseNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GHSPhraseNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
            GHSPhraseNode.Code.Text = Code;
            GHSPhraseNode.Category.Value = "Precaution";
            GHSPhraseNode.English.Text = English;
            GHSPhraseNode.postChanges( false );
        }

    }//class CswUpdateSchema_02B_Case29717

}//namespace ChemSW.Nbt.Schema