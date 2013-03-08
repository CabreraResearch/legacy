using System;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{

    public class CswNbtNodeWriter
    {

        private CswNbtResources _CswNbtResources = null;
        private CswNbtNodeWriterNative _CswNbtNodeWriterNative = null;
        private CswNbtNodeWriterRelationalDb _CswNbtNodeWriterRelationalDb = null;
        public CswNbtNodeWriter( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void clear()
        {
            if( null != _CswNbtNodeWriterNative )
            {
                _CswNbtNodeWriterNative.clear();
            }


            if( null != _CswNbtNodeWriterRelationalDb )
            {
                _CswNbtNodeWriterRelationalDb.clear();
            }
        }//clear() 

        private ICswNbtNodeWriterImpl getWriterImpl( CswPrimaryKey NodePk )
        {
            return getWriterImpl( NodePk.TableName );
        }
        private ICswNbtNodeWriterImpl getWriterImpl( Int32 NodeTypeId )
        {
            return getWriterImpl( _CswNbtResources.MetaData.getNodeType( NodeTypeId ).TableName );
        }
        private ICswNbtNodeWriterImpl getWriterImpl( string TableName )
        {
            ICswNbtNodeWriterImpl ReturnVal = null;
            if( TableName.ToLower() == "nodes" )
            {
                if( _CswNbtNodeWriterNative == null )
                    _CswNbtNodeWriterNative = new CswNbtNodeWriterNative( _CswNbtResources );
                ReturnVal = _CswNbtNodeWriterNative;
            }
            else
            {
                if( _CswNbtNodeWriterRelationalDb == null )
                    _CswNbtNodeWriterRelationalDb = new CswNbtNodeWriterRelationalDb( _CswNbtResources );
                ReturnVal = _CswNbtNodeWriterRelationalDb;
            }
            return ( ReturnVal );
        }

        public void makeNewNodeEntry( CswNbtNode Node, bool PostToDatabase, bool IsCopy, bool OverrideUniqueValidation )
        {
            // case 20970
            CswNbtActQuotas Quotas = new CswNbtActQuotas( _CswNbtResources );
            if( !Quotas.CheckQuotaNT( Node.NodeTypeId ) )
            {
                Node.Locked = true;
            }

            getWriterImpl( Node.NodeTypeId ).makeNewNodeEntry( Node, PostToDatabase );
            //setDefaultPropertyValues( Node );

            // case 22591 - make empty rows for every property
            if( PostToDatabase )
            {
                foreach( CswNbtNodePropWrapper PropWrapper in Node.Properties )
                {
                    PropWrapper.makePropRow();
                }
                Node.postChanges( true, IsCopy, OverrideUniqueValidation );
            }
        }//makeNewNodeEntry()

        public void write( CswNbtNode Node, bool ForceSave, bool IsCopy, bool OverrideUniqueValidation )
        {
            if( NodeSpecies.Plain == Node.NodeSpecies &&
                ( ForceSave || NodeModificationState.Modified == Node.ModificationState ) )
            {
                //When CswNbtNode.NodeId is Int32.MinValue, we know that the node data was not 
                //filled from an existing node and therefore needs to be written to 
                //the db, after which it will have a node id
                if( null == Node.NodeId )
                {
                    makeNewNodeEntry( Node, true, IsCopy, OverrideUniqueValidation );
                    _incrementNodeCount( Node );
                    //setDefaultPropertyValues( Node );
                }
                else if( Node.IsTempModified )
                {
                    _incrementNodeCount( Node );
                    Node.IsTempModified = false;
                }

                //propcoll knows whether or not he's got new 
                //values to update (presumably)

                //bz # 5878
                //Node.Properties.ManageTransaction = _ManageTransaction;
                Node.Properties.update( IsCopy, OverrideUniqueValidation );

                //set nodename with updated prop values
                _synchNodeName( Node );

                getWriterImpl( Node.NodeId ).write( Node, ForceSave, IsCopy );

            }//if node was modified

        }//write()

        private void _incrementNodeCount( CswNbtNode Node )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Node.NodeTypeId );
            if( null != NodeType )
            {
                NodeType.IncrementNodeCount();
            }
        }

        private string _makeDefaultNodeName( CswNbtNode Node )
        {
            string ReturnVal = "";

            ReturnVal = Node.getNodeType().NodeTypeName + " " + Node.NodeId.PrimaryKey.ToString();

            return ( ReturnVal );
        }//_makeDefaultNodeName


        // TODO: This should defer to CswNbtFieldTypeRules for implementation once NbtBase and NbtLogic are merged
        public void setDefaultPropertyValues( CswNbtNode Node )
        {
            foreach( CswNbtNodePropWrapper Prop in Node.Properties )
            {
                Prop.NodeId = Node.NodeId;
                // Only set values for unmodified properties
                // This is important for nodes created through workflows
                if( !Prop.WasModified )
                {
                    Prop.SetDefaultValue();
                }
            } // foreach( CswNbtNodePropWrapper Prop in Node.Properties )
        } // setDefaultPropertyValues()

        public void setSequenceValues( CswNbtNode Node )
        {
            foreach( CswNbtNodePropWrapper Prop in Node.Properties )
            {
                CswNbtMetaDataFieldType.NbtFieldType FT = Prop.getFieldTypeValue();
                if( FT == CswNbtMetaDataFieldType.NbtFieldType.Barcode )
                {
                    Prop.AsBarcode.setBarcodeValue();  // does not overwrite
                }
                else if( FT == CswNbtMetaDataFieldType.NbtFieldType.Sequence )
                {
                    Prop.AsSequence.setSequenceValue();  // does not overwrite
                }
            }
        }

        public void updateRelationsToThisNode( CswNbtNode Node )
        {
            getWriterImpl( Node.NodeId ).updateRelationsToThisNode( Node );
        }

        public void delete( CswNbtNode CswNbtNode )
        {
            getWriterImpl( CswNbtNode.NodeId ).delete( CswNbtNode );
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

            if( NewNodeName.Trim() != string.Empty )
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

    }//CswNbtNodeWriterNative

}//namespace ChemSW.Nbt
