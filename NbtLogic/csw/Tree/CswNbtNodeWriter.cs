using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{

    public class CswNbtNodeWriter
    {

        private CswNbtResources _CswNbtResources = null;
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
        private CswNbtNodeWriterRelationalDb _CswNbtNodeWriterRelationalDb = null;


        private CswTableUpdate _CswTableUpdateNodes = null;
        private CswTableUpdate CswTableUpdateNodes
        {
            get
            {
                if( _CswTableUpdateNodes == null )
                    _CswTableUpdateNodes = _CswNbtResources.makeCswTableUpdate( "CswNbtNodeWriterNative_update", "nodes" );
                return _CswTableUpdateNodes;
            }
        }

        public CswNbtNodeWriter( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeWriterRelationalDb = new CswNbtNodeWriterRelationalDb( _CswNbtResources );
        }

        public void clear()
        {
            if( null != _CswTableUpdateNodes )
            {
                _CswTableUpdateNodes.clear();
            }
        }//clear() 

        //public void makeNewNodeEntry( CswNbtNode Node, bool IsCopy, bool OverrideUniqueValidation )
        public void makeNewNodeEntry( CswNbtNode Node )
        {
            // case 20970
            CswNbtActQuotas Quotas = new CswNbtActQuotas( _CswNbtResources );
            CswNbtActQuotas.Quota Quota = Quotas.CheckQuotaNT( Node.NodeTypeId );
            if( !Quota.HasSpace )
            {
                Node.Locked = true;
            }

            DataTable NewNodeTable = CswTableUpdateNodes.getEmptyTable();
            DataRow NewNodeRow = NewNodeTable.NewRow();
            NewNodeRow["nodename"] = Node.NodeName;
            NewNodeRow["nodetypeid"] = CswConvert.ToDbVal( Node.NodeTypeId );
            NewNodeRow["pendingupdate"] = CswConvert.ToDbVal( false );
            NewNodeRow["readonly"] = CswConvert.ToDbVal( false );
            NewNodeRow["isdemo"] = CswConvert.ToDbVal( false );
            NewNodeRow["issystem"] = CswConvert.ToDbVal( false );
            NewNodeRow["hidden"] = CswConvert.ToDbVal( false );
            NewNodeRow["searchable"] = CswConvert.ToDbVal( true );
            NewNodeRow["iconfilename"] = Node.IconFileNameOverride;
            NewNodeRow["created"] = DateTime.Now;

            //case 27709: nodes must have an explicit audit level
            CswNbtMetaDataNodeType CswNbtMetaDataNodeType = Node.getNodeType();
            if( null != CswNbtMetaDataNodeType )
            {
                NewNodeRow[_CswAuditMetaData.AuditLevelColName] = CswNbtMetaDataNodeType.AuditLevel;
                Node.AuditLevel = CswNbtMetaDataNodeType.AuditLevel; //Otherwise the Node's deafult NoAudit setting gets written to db; trust me on this one
            }

            NewNodeTable.Rows.Add( NewNodeRow );

            Node.NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NewNodeTable.Rows[0]["nodeid"] ) );

            // case 29311 - Sync with relational data
            if( Node.getNodeType().DoRelationalSync )
            {
                _CswNbtNodeWriterRelationalDb.makeNewNodeEntry( Node, false );
            }
            if( CswTools.IsPrimaryKey( Node.RelationalId ) )
            {
                NewNodeRow["relationalid"] = Node.RelationalId.PrimaryKey;
                NewNodeRow["relationaltable"] = Node.RelationalId.TableName;
            }
            CswTableUpdateNodes.update( NewNodeTable, ( false == Node.IsTemp ) );

        }//makeNewNodeEntry()

        public void write( CswNbtNode Node, bool ForceSave, bool IsCopy, bool OverrideUniqueValidation, bool Creating, bool AllowAuditing, bool SkipEvents )
        {
            if( CswEnumNbtNodeSpecies.Plain == Node.NodeSpecies &&
                ( ForceSave || CswEnumNbtNodeModificationState.Modified == Node.ModificationState ) )
            {
                //When CswNbtNode.NodeId is Int32.MinValue, we know that the node data was not 
                //filled from an existing node and therefore needs to be written to 
                //the db, after which it will have a node id
                if( null == Node.NodeId )
                {
                    makeNewNodeEntry( Node );
                    //makeNewNodeEntry( Node, IsCopy, OverrideUniqueValidation );
                }

                //bz # 5878
                //propcoll knows whether or not he's got new values to update (presumably)
                Node.Properties.update( Node, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing, SkipEvents );

                //set nodename with updated prop values
                _synchNodeName( Node );

                // save nodename and pendingupdate
                if( Node.NodeId.TableName != "nodes" )
                    throw new CswDniException( CswEnumErrorType.Error, "Internal data error", "CswNbtNodeWriterNative attempted to write a node in table: " + Node.NodeId.TableName );

                DataTable NodesTable = CswTableUpdateNodes.getTable( "nodeid", Node.NodeId.PrimaryKey );
                if( 1 != NodesTable.Rows.Count )
                    throw ( new CswDniException( CswEnumErrorType.Error, "Internal data errors", "There are " + NodesTable.Rows.Count.ToString() + " node table records for node id (" + Node.NodeId.ToString() + ")" ) );

                NodesTable.Rows[0]["nodename"] = Node.NodeName;
                NodesTable.Rows[0]["pendingupdate"] = CswConvert.ToDbVal( Node.PendingUpdate );
                NodesTable.Rows[0]["readonly"] = CswConvert.ToDbVal( Node.ReadOnlyPermanent );
                NodesTable.Rows[0]["locked"] = CswConvert.ToDbVal( Node.Locked );
                NodesTable.Rows[0]["isdemo"] = CswConvert.ToDbVal( Node.IsDemo );
                NodesTable.Rows[0]["istemp"] = CswConvert.ToDbVal( Node.IsTemp );
                NodesTable.Rows[0]["sessionid"] = CswConvert.ToDbVal( Node.SessionId );
                NodesTable.Rows[0][_CswAuditMetaData.AuditLevelColName] = Node.AuditLevel;
                NodesTable.Rows[0]["hidden"] = CswConvert.ToDbVal( Node.Hidden );
                NodesTable.Rows[0]["iconfilename"] = Node.IconFileNameOverride;
                NodesTable.Rows[0]["searchable"] = CswConvert.ToDbVal( Node.Searchable );
                NodesTable.Rows[0]["legacyid"] = CswConvert.ToDbVal( Node.LegacyId );

                // case 29311 - Sync with relational data
                if( Node.getNodeType().DoRelationalSync )
                {
                    _CswNbtNodeWriterRelationalDb.write( Node, ForceSave, IsCopy, AllowAuditing );
                }

                if( null != Node.RelationalId )
                {
                    NodesTable.Rows[0]["relationalid"] = Node.RelationalId.PrimaryKey;
                    NodesTable.Rows[0]["relationaltable"] = Node.RelationalId.TableName;
                }
                CswTableUpdateNodes.update( NodesTable );


            }//if node was modified

        }//write()

        private string _makeDefaultNodeName( CswNbtNode Node )
        {
            string ReturnVal = "";

            ReturnVal = Node.getNodeType().NodeTypeName + " " + Node.NodeId.PrimaryKey.ToString();

            return ( ReturnVal );
        }//_makeDefaultNodeName


        public void setDefaultPropertyValues( CswNbtNode Node )
        {
            foreach( CswNbtNodePropWrapper Prop in Node.Properties )
            {
                Prop.NodeId = Node.NodeId;
                // Only set values for unmodified properties
                // This is important for nodes created through workflows
                if( false == Prop.wasAnySubFieldModified() )
                {
                    Prop.SetDefaultValue();
                }
            } // foreach( CswNbtNodePropWrapper Prop in Node.Properties )
        } // setDefaultPropertyValues()

        public void setSequenceValues( CswNbtNode Node )
        {
            if( false == Node.IsTemp )
            {
                foreach( CswNbtNodePropWrapper Prop in Node.Properties )
                {
                    CswEnumNbtFieldType FT = Prop.getFieldTypeValue();
                    if( FT == CswEnumNbtFieldType.Barcode )
                    {
                        Prop.AsBarcode.setBarcodeValue(); // does not overwrite
                    }
                    else if( FT == CswEnumNbtFieldType.Sequence )
                    {
                        Prop.AsSequence.setSequenceValue(); // does not overwrite
                    }
                }
            }
        }

        public void updateRelationsToThisNode( CswNbtNode Node )
        {
            if( Node.NodeId.TableName != "nodes" )
                throw new CswDniException( CswEnumErrorType.Error, "Internal System Error", "CswNbtNodeWriterNative.updateRelationsToThisNode() called on a non-native node" );

            string SQL = @"update jct_nodes_props 
                              set pendingupdate = '" + CswConvert.ToDbVal( true ) + @"' 
                            where jctnodepropid in (select j.jctnodepropid
                                                      from jct_nodes_props j
                                                      join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                                      join field_types f on p.fieldtypeid = f.fieldtypeid
                                                     where (f.fieldtype = 'Relationship' or f.fieldtype = 'Location' or f.fieldtype = 'Quantity')
                                                       and j.field1_fk = " + Node.NodeId.PrimaryKey.ToString() + ")";

            // We're not doing this in a CswTableUpdate because it might be a large operation, 
            // and we don't care about auditing for this change.
            _CswNbtResources.execArbitraryPlatformNeutralSql( SQL );

            //// case 29311 - Sync with relational data
            //if( Node.getNodeType().DoRelationalSync )
            //{
            //    _CswNbtNodeWriterRelationalDb.updateRelationsToThisNode( Node );
            //}
        }

        public void delete( CswNbtNode Node )
        {
            //Case 31416 - Delete any blob_data associated with this nodes properties
            CswCommaDelimitedString DoomedBlobData = new CswCommaDelimitedString();
            foreach( CswNbtNodePropWrapper BlobProp in Node.Properties.Where( P => P.IsBlobProp() ) )
            {
                DoomedBlobData.Add( BlobProp.JctNodePropId.ToString() );
            }
            CswTableUpdate CswTableUpdateBlobData = _CswNbtResources.makeCswTableUpdate( "deletenode_blobdata", "blob_data" );
            if( DoomedBlobData.Count > 0 )
            {
                DataTable BlobData = CswTableUpdateBlobData.getTable( "where jctnodepropid in (" + DoomedBlobData + ")" );
                foreach( DataRow BlobRow in BlobData.Rows )
                {
                    BlobRow.Delete();
                }
                CswTableUpdateBlobData.update( BlobData );
            }

            // Delete this node's property values
            CswTableUpdate CswTableUpdateJct = _CswNbtResources.makeCswTableUpdate( "deletenode_update", "jct_nodes_props" );
            DataTable JctTable = CswTableUpdateJct.getTable( " where nodeid=" + Node.NodeId.PrimaryKey.ToString() );
            foreach( DataRow Row in JctTable.Rows )
            {
                Row.Delete();
            }
            CswTableUpdateJct.update( JctTable );

            // Delete property values of relationships to this node
            if( Node.NodeId.TableName != "nodes" )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Internal System Error", "CswNbtNodeWriterNative.delete() called on a non-native node" );
            }

            // From getRelationshipsToNode.  see case 27711
            string InClause = @"select j.jctnodepropid
                                      from jct_nodes_props j
                                      join nodes n on j.nodeid = n.nodeid
                                      join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                      join field_types f on p.fieldtypeid = f.fieldtypeid
                                     where (f.fieldtype = 'Relationship' or f.fieldtype = 'Location')
                                       and j.field1_fk = " + Node.NodeId.PrimaryKey.ToString();

            DataTable RelatedJctTable = CswTableUpdateJct.getTable( " where jctnodepropid in (" + InClause + ")" );
            foreach( DataRow Row in RelatedJctTable.Rows )
            {
                Row.Delete();
            }
            CswTableUpdateJct.update( RelatedJctTable );

            // Delete the node

            DataTable NodesTable = CswTableUpdateNodes.getTable( "nodeid", Node.NodeId.PrimaryKey, true );
            NodesTable.Rows[0].Delete();
            CswTableUpdateNodes.update( NodesTable );


            // case 29311 - Sync with relational data
            if( Node.getNodeType().DoRelationalSync )
            {
                _CswNbtNodeWriterRelationalDb.delete( Node );
            }

        }//delete()

        private void _synchNodeName( CswNbtNode Node )
        {
            string OldNodeName = Node.NodeName;
            string NewNodeName = string.Empty;
            string NameTemplate = Node.getNodeType().NameTemplateValue;

            if( NameTemplate.Length > 0 )
            {
                //we assume that the nodetype has nodetype_props corresponding to all "[]" 
                //parameters in the nametemplate. 
                if( Node.Properties.Count > 0 )
                {
                    NewNodeName = NameTemplate;
                    RegexOptions RegExOpts = new RegexOptions();
                    RegExOpts |= RegexOptions.IgnoreCase;

                    foreach( CswNbtNodePropWrapper CurrentProp in Node.Properties )
                    {
                        string TemplateParamCandidate = CswNbtMetaData.MakeTemplateEntry( CurrentProp.NodeTypePropId.ToString() );
                        int TemplateParamStartIdx = NewNodeName.ToLower().IndexOf( TemplateParamCandidate );
                        if( TemplateParamStartIdx > -1 )
                        {
                            Regex RegExObj = new Regex( "\\" + TemplateParamCandidate, RegExOpts );
                            NewNodeName = RegExObj.Replace( NewNodeName, CurrentProp.ValueForNameTemplate );
                        } //if current property is used in the name template

                    } //iterate props
                } // if( Node.Properties.Count > 0 )
            } // if( NameTemplate.Length > 0 )

            //Case 31057 - nodename can only be 255 chars max
            if( NewNodeName.Length > 255 )
            {
                NewNodeName = NewNodeName.Substring( 0, 255 );
            }

            NewNodeName = NewNodeName.Trim();
            if( NewNodeName != string.Empty )
            {
                Node.NodeName = NewNodeName;
            }
            else
            {
                Node.NodeName = _makeDefaultNodeName( Node );
            }

            // When a node's name changes, we need to update any relationships (and locations) referencing that node
            if( Node.NodeName != OldNodeName )
            {
                updateRelationsToThisNode( Node );
            }

        }//_synchNodeName()


        /// <summary>
        /// Create audit records as if the node is being inserted, for use with temp nodes
        /// </summary>
        public void AuditInsert( CswNbtNode Node )
        {
            DataTable NodesTable = CswTableUpdateNodes.getTable( "nodeid", Node.NodeId.PrimaryKey );
            if( NodesTable.Rows.Count > 0 )
            {
                _CswNbtResources.AuditRecorder.addInsertRow( NodesTable.Rows[0] );
            }
        }

    }//CswNbtNodeWriter

}//namespace ChemSW.Nbt
