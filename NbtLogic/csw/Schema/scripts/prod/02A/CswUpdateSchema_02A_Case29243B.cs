using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29243
    /// </summary>
    public class CswUpdateSchema_02A_Case29243B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29243; }
        }

        public override void update()
        {
            // Part 2: Update the HazardClass list on the ChemicalNTP
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtMetaDataNodeTypeProp HazardClassNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ChemicalNT.NodeTypeId, "Hazard Classes" );
                if( null != HazardClassNTP )
                {
                    CswCommaDelimitedString FireHazardClassTypes = new CswCommaDelimitedString
                        {
                            "Aero-1",
                            "Aero-2",
                            "Aero-3",
                            "Carc",
                            "CF/D (bailed)",
                            "CF/D (loose)",
                            "CL-II",
                            "CL-IIIA",
                            "CL-IIIB",
                            "Corr",
                            "Corr (liquified gas)",
                            "CRY-FG",
                            "CRY-NFG",
                            "CRY-OXY",
                            "Exp",
                            "Exp-1.1",
                            "Exp-1.2",
                            "Exp-1.3",
                            "Exp-1.4",
                            "Exp-1.4G",
                            "Exp-1.5",
                            "Exp-1.6",
                            "FG (gaseous)",
                            "FG (liquified)",
                            "FL-1A",
                            "FL-1B",
                            "FL-1C",
                            "FS",
                            "H.T.",
                            "H.T. (liquified gas)",
                            "Irr",
                            "N/R",
                            "NFG",
                            "NFG (liquified)",
                            "NFS",
                            "OHH",
                            "Oxy-1",
                            "Oxy-2",
                            "Oxy-3",
                            "Oxy-4",
                            "Oxy-Gas",
                            "Oxy-Gas (liquid)",
                            "Perox-Det",
                            "Perox-I",
                            "Perox-II",
                            "Perox-III",
                            "Perox-IV",
                            "Perox-V",
                            "Pyro",
                            "RAD-Alpha",
                            "RAD-Beta",
                            "RAD-Gamma",
                            "Sens",
                            "Tox",
                            "Tox (liquified gas)",
                            "UR-1",
                            "UR-2",
                            "UR-3",
                            "UR-4",
                            "WR-1",
                            "WR-2",
                            "WR-3"
                        };

                    HazardClassNTP.ListOptions = FireHazardClassTypes.ToString();
                }
            }

        }

    }//class CswUpdateSchema_02A_Case29243B

}//namespace ChemSW.Nbt.Schema