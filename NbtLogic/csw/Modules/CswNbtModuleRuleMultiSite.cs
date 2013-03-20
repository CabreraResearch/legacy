using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Multi Site Module
    /// </summary>
    public class CswNbtModuleRuleMultiSite : CswNbtModuleRule
    {
        public CswNbtModuleRuleMultiSite( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.MultiSite; } }
        public override void OnEnable()
        {
            CswNbtMetaDataNodeType siteNT = _CswNbtResources.MetaData.getNodeType( "Site" );
            if( null != siteNT )
            {
                CswNbtActQuotas QuotasAct = new CswNbtActQuotas( _CswNbtResources );
                QuotasAct.SetQuotaForNodeType( siteNT.NodeTypeId, Int32.MinValue, false );
            }
        }

        public override void OnDisable()
        {

            CswNbtMetaDataNodeType siteNT = _CswNbtResources.MetaData.getNodeType( "Site" );
            if( null != siteNT )
            {
                CswNbtActQuotas QuotasAct = new CswNbtActQuotas( _CswNbtResources );
                int SitesCount = QuotasAct.GetNodeCountForNodeType( siteNT.NodeTypeId );
                if( SitesCount > 1 && false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
                {
                    throw new CswDniException( ErrorType.Warning, "Cannot disable the MultiSite Module when multiple Sites exist", SitesCount + " Site nodes exist, cannot disable the MultiSite module" );
                }
                else
                {
                    QuotasAct.SetQuotaForNodeType( siteNT.NodeTypeId, 1, true );
                }
            }

        } // OnDisable()

    } // class CswNbtModuleRuleMultiSite
}// namespace ChemSW.Nbt
