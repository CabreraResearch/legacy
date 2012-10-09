using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW;
using ChemSW.Core;
using ChemSW.WebSvc;
using ChemSW.Nbt;
using ChemSW.Nbt.WebServices;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Logic.Labels;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Reports
{
    public class CswNbtWebServiceMailReports
    {
        private HttpContext _Context = HttpContext.Current;

        #region WCF Data Objects

        /// <summary>
        /// Return Object for Mail Report Subscriptions, which inherits from CswWebSvcReturn
        /// </summary>
        [DataContract]
        public class MailReportSubscriptionsReturn : CswWebSvcReturn
        {
            public MailReportSubscriptionsReturn()
            {
                Data = new MailReportSubscriptions();
            }
            [DataMember]
            public MailReportSubscriptions Data;
        }

        [DataContract]
        public class MailReportSubscriptions
        {
            public MailReportSubscriptions()
            {
                Subscriptions = new Collection<Subscription>();
            }

            [DataContract]
            public class Subscription
            {
                [DataMember]
                public string Name = string.Empty;
                [DataMember]
                public string NodeId = string.Empty;
                [DataMember]
                public bool Subscribed = false;
                [DataMember]
                public bool Modified = false;
            }

            [DataMember]
            public Collection<Subscription> Subscriptions;

        } // MailReportSubscriptions

        #endregion WCF Data Objects

        public static void getSubscriptions( ICswResources CswResources, MailReportSubscriptionsReturn Return, object Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            MailReportSubscriptions Subs = new MailReportSubscriptions();

            CswPrimaryKey ThisUserPk = CswNbtResources.CurrentNbtUser.UserId;

            CswNbtMetaDataObjectClass MailReportOC = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            foreach( CswNbtObjClassMailReport MailReportNode in MailReportOC.getNodes( false, false ) )
            {
                MailReportSubscriptions.Subscription sub = new MailReportSubscriptions.Subscription();
                sub.Name = MailReportNode.NodeName;
                sub.NodeId = MailReportNode.NodeId.ToString();
                sub.Subscribed = MailReportNode.Recipients.IsSubscribed( ThisUserPk );
                Subs.Subscriptions.Add( sub );
            }
            Return.Data = Subs;
        } // getSubscriptions()

        public static void saveSubscriptions( ICswResources CswResources, CswWebSvcReturn Return, MailReportSubscriptions Request )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey ThisUserPk = CswNbtResources.CurrentNbtUser.UserId;
            CswNbtMetaDataObjectClass MailReportOC = CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            foreach( MailReportSubscriptions.Subscription sub in Request.Subscriptions )
            {
                if( sub.Modified )
                {
                    CswPrimaryKey NodeId = new CswPrimaryKey();
                    NodeId.FromString( sub.NodeId );

                    CswNbtObjClassMailReport MailReportNode = CswNbtResources.Nodes[NodeId];
                    if( sub.Subscribed )
                    {
                        MailReportNode.Recipients.AddUser( ThisUserPk );
                    }
                    else
                    {
                        MailReportNode.Recipients.RemoveUser( ThisUserPk );
                    }
                    MailReportNode.postChanges( false );
                }
            }
        } // saveSubscriptions()

    } // class CswNbtWebServiceMailReports
} // namespace NbtWebApp.WebSvc.Logic.Reports
