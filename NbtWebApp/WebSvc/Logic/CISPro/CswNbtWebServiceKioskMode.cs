using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Actions.KioskMode;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceKioskMode
    {

        #region Data Contracts

        [DataContract]
        public class KioskModeDataReturn : CswWebSvcReturn
        {
            public KioskModeDataReturn()
            {
                Data = new KioskModeData();
            }
            [DataMember]
            public KioskModeData Data;
        }

        #endregion

        public static void GetAvailableModes( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassRole currentUserRoleNode = NbtResources.Nodes.makeRoleNodeFromRoleName( NbtResources.CurrentNbtUser.Rolename );

            KioskModeData kioskModeData = new KioskModeData();
            string applies_to_all = "";
            string applies_to_cont = "Containers";
            string applies_to_imcs = "Equipment & Assemblies";

            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
                {
                    applies_to_all = "Containers, Equipment & Assemblies";
                }
                else
                {
                    applies_to_all = applies_to_cont;
                }
            }
            else if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
            {
                applies_to_all = "Equipment & Assemblies";
                applies_to_cont = "";
            }


            kioskModeData.AvailableModes.Add( new Mode
            {
                name = CswTools.UppercaseFirst( CswEnumNbtKioskModeRuleName.Move._Name ),
                imgUrl = "Images/newicons/KioskMode/Move_code39.png",
                applies_to_types = applies_to_all
            } );

            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( CswEnumNbtKioskModeRuleName.Owner._Name ),
                    imgUrl = "Images/newicons/KioskMode/Owner_code39.png",
                    applies_to_types = applies_to_all
                } );
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( CswEnumNbtKioskModeRuleName.Transfer._Name ),
                    imgUrl = "Images/newicons/KioskMode/Transfer_code39.png",
                    applies_to_types = applies_to_all
                } );
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( CswEnumNbtKioskModeRuleName.Open._Name ),
                    imgUrl = "Images/newicons/KioskMode/Open_code39.png",
                    applies_to_types = applies_to_cont
                } );

                CswNbtPermit permissions = new CswNbtPermit( NbtResources );
                if( permissions.can( CswEnumNbtActionName.DispenseContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( CswEnumNbtKioskModeRuleName.Dispense._Name ),
                        imgUrl = "Images/newicons/KioskMode/Dispense_code39.png",
                        applies_to_types = applies_to_cont
                    } );
                }
                if( permissions.can( CswEnumNbtActionName.DisposeContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( CswEnumNbtKioskModeRuleName.Dispose._Name ),
                        imgUrl = "Images/newicons/KioskMode/Dispose_code39.png",
                        applies_to_types = applies_to_cont
                    } );
                }
            }

            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( CswEnumNbtKioskModeRuleName.Status._Name ),
                    imgUrl = "Images/newicons/KioskMode/Status_code39.png",
                    applies_to_types = applies_to_imcs
                } );
            }

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Reset",
                imgUrl = "Images/newicons/KioskMode/Reset_code39.png"
            } );

            Return.Data = kioskModeData;
        }

        public static void HandleScan( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( _isModeScan( KioskModeData.OperationData.LastItemScanned, KioskModeData ) )
            {
                KioskModeData.OperationData.Mode = KioskModeData.OperationData.LastItemScanned;
                _setFields( NbtResources, KioskModeData.OperationData );
                KioskModeData.OperationData.Field1.Active = true;
                KioskModeData.OperationData.Field2.Active = false;
            }
            else
            {
                CswNbtKioskModeRule rule = CswNbtKioskModeRuleFactory.Make( NbtResources, KioskModeData.OperationData.Mode );
                if( false == KioskModeData.OperationData.Field2.ServerValidated && KioskModeData.OperationData.Field2.Active )
                {
                    rule.ValidateFieldTwo( ref KioskModeData.OperationData );
                }
                else if( false == KioskModeData.OperationData.Field1.ServerValidated && KioskModeData.OperationData.Field1.Active )
                {
                    rule.ValidateFieldOne( ref KioskModeData.OperationData );
                }
                else
                {
                    KioskModeData.OperationData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    KioskModeData.OperationData.ModeServerValidated = false;
                    KioskModeData.OperationData.Field1.Active = false;
                    KioskModeData.OperationData.Field2.Active = false;
                }
            }
            Return.Data = KioskModeData;
        }

        public static void CommitOperation( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            OperationData OpData = KioskModeData.OperationData;

            CswNbtKioskModeRule rule = CswNbtKioskModeRuleFactory.Make( NbtResources, OpData.Mode );
            rule.CommitOperation( ref OpData );

            KioskModeData.OperationData = OpData;
            Return.Data = KioskModeData;
        }

        public static void getKioskModeBarcodeReport( ICswResources CswResources, KioskModeDataReturn Return, object KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtMetaDataObjectClass ReportOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtSearch FindReport = new CswNbtSearch( NbtResources );
            FindReport.addFilter( ReportOC, false );
            FindReport.SearchType = CswEnumSqlLikeMode.Exact;
            FindReport.SearchTerm = "Kiosk Mode Barcodes";
            ICswNbtTree SearchResults = FindReport.Results();
            if( SearchResults.getChildNodeCount() > 0 )
            {
                SearchResults.goToNthChild( 0 );
                CswNbtObjClassReport ReportNode = SearchResults.getNodeForCurrentPosition();
                Return.Data.ReportNode = ReportNode.NodeId.ToString();
                SearchResults.goToParentNode();
            }
        }

        #region Private Methods

        private static void _setFields( CswNbtResources NbtResources, OperationData OpData )
        {
            CswNbtKioskModeRule rule = CswNbtKioskModeRuleFactory.Make( NbtResources, OpData.Mode );
            rule.SetFields( ref OpData );
        }

        private static bool _isModeScan( string ScannedMode, KioskModeData KMData )
        {
            bool Ret = KMData.AvailableModes.Any<Mode>( mode => mode.name.ToLower().Equals( ScannedMode.ToLower() ) );
            return Ret;
        }

        #endregion
    }

}