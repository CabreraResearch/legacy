using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Mobile;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.WebSvc.Logic.Mobile.CISProNbt
{
    public class CswNbtWebServiceLegacyMobile
    {
        /// <summary>
        /// Legacy mobile data formats
        /// </summary>
        public enum DataFormat
        {
            multi1,
            reconcilectrlctns
        }

        [DataContract]
        public class CswNbtMobileReturn : CswWebSvcReturn
        {
            public CswNbtMobileReturn()
            {
                Data = new CswNbtCISProNbtMobileData.LegacyMobileResponse();
            }

            [DataMember]
            public CswNbtCISProNbtMobileData.LegacyMobileResponse Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CswResources"></param>
        /// <param name="ReturnObj"></param>
        /// <param name="LegacyFileData"></param>
        public static void performOperations( ICswResources CswResources, CswNbtMobileReturn ReturnObj, string LegacyFileData )
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            Collection<CswNbtCISProNbtMobileData.MobileRequest.Operation> LegacyOperations = new Collection<CswNbtCISProNbtMobileData.MobileRequest.Operation>();

            // string filecontents = MPFDF.FileContents;
            string filecontents = LegacyFileData;

            //Convert string into an array of strings
            string[] delimiters = new string[] { "\r\n", "\n" };
            List<string> ops = new List<string>( filecontents.Split( delimiters, StringSplitOptions.RemoveEmptyEntries ) );

            //Grab the first line and store it as programname
            CswNbtCISProNbtMobileData.MobileRequest.Data LegacyData = new CswNbtCISProNbtMobileData.MobileRequest.Data();
            LegacyData.programname = ops[0];

            //What to do if the first line doesn't match with enum?
            DataFormat Format;
            if( Enum.TryParse( LegacyData.programname, out Format ) )
            {
                //remove the first line
                ops.RemoveAt( 0 );

                if( DataFormat.multi1 == Format )
                {
                    //iterate through operations and save as objects
                    foreach( string op in ops )
                    {
                        string[] currentOp = op.Split( ',' );
                        //create an operation object
                        CswNbtCISProNbtMobileData.MobileRequest.Operation NewOperation = new CswNbtCISProNbtMobileData.MobileRequest.Operation();
                        NewOperation.op = textInfo.ToTitleCase( currentOp[0].ToLower() );
                        NewOperation.barcode = currentOp[3];
                        //create an update object
                        CswNbtCISProNbtMobileData.MobileRequest.Update NewUpdate = new CswNbtCISProNbtMobileData.MobileRequest.Update();
                        NewUpdate.user = currentOp[1];
                        NewUpdate.location = currentOp[2];
                        //add update object to the operation object
                        NewOperation.update = NewUpdate;

                        //save the operation object to the collection of operations
                        LegacyOperations.Add( NewOperation );
                    }

                }
                else if( DataFormat.reconcilectrlctns == Format )
                {
                    //iterate through operations and save as objects
                    foreach( string op in ops )
                    {
                        string[] currentOp = op.Split( ',' );
                        //create an operation object
                        CswNbtCISProNbtMobileData.MobileRequest.Operation NewOperation = new CswNbtCISProNbtMobileData.MobileRequest.Operation();
                        NewOperation.op = "Reconcile";
                        NewOperation.barcode = currentOp[1];
                        //create an update object
                        CswNbtCISProNbtMobileData.MobileRequest.Update NewUpdate = new CswNbtCISProNbtMobileData.MobileRequest.Update();
                        NewUpdate.location = currentOp[0];
                        //add update object to the operation object
                        NewOperation.update = NewUpdate;

                        //save the operation object to the collection of operations
                        LegacyOperations.Add( NewOperation );
                    }
                }

                //Add the collection of operations to the data object
                LegacyData.MultiOpRows = LegacyOperations;

                //Create a new mobilerequest object and add the data object to it
                CswNbtCISProNbtMobileData.MobileRequest LegacyMobileDataObj = new CswNbtCISProNbtMobileData.MobileRequest();
                LegacyMobileDataObj.data = LegacyData;

                //Call the action class which makes the batch op
                CswNbtActUploadLegacyMobileData _CswNbtActUploadLegacyMobileData = new CswNbtActUploadLegacyMobileData( (CswNbtResources) CswResources );
                CswNbtObjClassBatchOp BatchNode = _CswNbtActUploadLegacyMobileData.makeNodesBatch( LegacyMobileDataObj );

                //Create a JObject to display the tree view in the last step of the wizard
                JObject ret = new JObject();

                if( null != BatchNode )
                {
                    ret["result"] = 1;

                    CswNbtView BatchOpsView = new CswNbtView( (CswNbtResources) CswResources );
                    BatchOpsView.ViewName = "New Batch Operations";
                    BatchOpsView.ViewMode = NbtViewRenderingMode.Tree;
                    CswNbtViewRelationship BatchRel = BatchOpsView.AddViewRelationship( BatchNode.NodeType, false );
                    BatchRel.NodeIdsToFilterIn.Add( BatchNode.NodeId );

                    CswNbtWebServiceTree ws = new CswNbtWebServiceTree( (CswNbtResources) CswResources, BatchOpsView );
                    ret["treedata"] = ws.runTree( null, null, false, true, string.Empty );

                    BatchOpsView.SaveToCache( true );
                    ret["sessionviewid"] = BatchOpsView.SessionViewId.ToString();
                    ret["viewmode"] = BatchOpsView.ViewMode.ToString();
                }

                ReturnObj.Data.TreeData = ret.ToString();
            }
            else
            {
                ReturnObj.Data.Error = "The program name is invalid.";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CswResources"></param>
        /// <param name="ReturnObj"></param>
        /// <param name="Stream"></param>
        public static void parseDataFile( ICswResources CswResources, CswNbtMobileReturn ReturnObj, Stream Stream )
        {
            CswTools.MultiPartFormDataFile MPFDF = new CswTools.MultiPartFormDataFile( Stream );

            ReturnObj.Data.FileContents = MPFDF.FileContents;
        }

    }//class CswNbtWebServiceCISProNbtMobile

}//namespace NbtWebApp.WebSvc.Logic.CISProNbtMobile
