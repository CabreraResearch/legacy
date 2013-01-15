using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceKioskMode
    {
        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceKioskMode( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

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

        [DataContract]
        public class KioskModeData
        {
            [DataMember]
            public Collection<Mode> AvailableModes = new Collection<Mode>();
        }

        [DataContract]
        public class Mode
        {
            [DataMember]
            public string name = string.Empty;
        }

        #endregion

        public static void GetAvailableModes( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData ImgData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassRole currentUserRoleNode = NbtResources.Nodes.makeRoleNodeFromRoleName( NbtResources.CurrentNbtUser.Rolename );

            KioskModeData kioskModeData = new KioskModeData();

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Move"
            } );
            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Owner"
            } );
            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Transfer"
            } );
            CswNbtPermit permissions = new CswNbtPermit( NbtResources );
            if( permissions.can( CswNbtActionName.DispenseContainer ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswNbtActionName.DispenseContainer.ToString()
                } );
            }
            if( permissions.can( CswNbtActionName.DisposeContainer ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswNbtActionName.DisposeContainer.ToString()
                } );
            }

            Return.Data = kioskModeData;
        }

    }

}