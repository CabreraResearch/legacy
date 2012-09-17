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
                    //setDefaultPropertyValues( Node );
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
                CswNbtMetaDataFieldType.NbtFieldType FT = Prop.getFieldType().FieldType;
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

            string NameTemplate = Node.getNodeType().NameTemplateValue;
            if( NameTemplate.Length > 0 )
            {
                //we assume that the nodetype has nodetype_props corresponding to all "[]" 
                //parameters in the nametemplate. 
                //                string NewName = "";
                if( Node.Properties.Count > 0 )
                {
                    //                    string RegExpInitialDivision = @"^\s*(select.*\sfrom\s+)([^\s].*)$";
                    RegexOptions RegExOpts = new RegexOptions();
                    RegExOpts |= RegexOptions.IgnoreCase;

                    //int FilledTemplateParameters = 0;
                    foreach( CswNbtNodePropWrapper CurrentProp in Node.Properties )
                    {
                        //if( !CurrentProp.Empty )
                        //{
                        string TemplateParamCandidate = CswNbtMetaData.MakeTemplateEntry( CurrentProp.NodeTypePropId.ToString() );
                        int TemplateParamStartIdx = NameTemplate.ToLower().IndexOf( TemplateParamCandidate );
                        if( TemplateParamStartIdx > -1 )
                        {
                            Regex RegExObj = new Regex( "\\" + TemplateParamCandidate, RegExOpts );
                            NameTemplate = RegExObj.Replace( NameTemplate, CurrentProp.Gestalt );
                            //FilledTemplateParameters++;
                        }//if current property is used in the name template

                    }//iterate props
                    if( NameTemplate.Trim() != string.Empty )
                    //                       0 < FilledTemplateParameters)
                    {
                        Node.NodeName = NameTemplate;
                    }
                    else
                    {
                        Node.NodeName = _makeDefaultNodeName( Node );
                    }
                }
                else
                {
                    Node.NodeName = _makeDefaultNodeName( Node );
                }
            }
            else
            {
                Node.NodeName = _makeDefaultNodeName( Node );
            }//if-else we have a nametemplate


            // When a node's name changes, we need to update any relationships (and locations) referencing that node
            if( Node.NodeName != OldNodeName )
            {
                updateRelationsToThisNode( Node );
            }

        }//_synchNodeName()

    }//CswNbtNodeWriterNative

}//namespace ChemSW.Nbt
