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
        //private CswNbtNodeWriterRelationalDb _CswNbtNodeWriterRelationalDb = null;
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


            //if( null != _CswNbtNodeWriterRelationalDb )
            //{
            //    _CswNbtNodeWriterRelationalDb.clear();
            //}
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
            //if( TableName.ToLower() == "nodes" )
            //{
                if( _CswNbtNodeWriterNative == null )
                    _CswNbtNodeWriterNative = new CswNbtNodeWriterNative( _CswNbtResources );
                ReturnVal = _CswNbtNodeWriterNative;
            //}
            //else
            //{
            //    if( _CswNbtNodeWriterRelationalDb == null )
            //        _CswNbtNodeWriterRelationalDb = new CswNbtNodeWriterRelationalDb( _CswNbtResources );
            //    ReturnVal = _CswNbtNodeWriterRelationalDb;
            //}
            return ( ReturnVal );
        }

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

            getWriterImpl( Node.NodeTypeId ).makeNewNodeEntry( Node, true );
            //setDefaultPropertyValues( Node );

            // case 22591 - make empty rows for every property
            foreach( CswNbtNodePropWrapper PropWrapper in Node.Properties )
            {
                PropWrapper.makePropRow();
            }
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
                Node.Properties.update( Node, IsCopy, OverrideUniqueValidation, Creating, null, AllowAuditing, SkipEvents );

                //set nodename with updated prop values
                _synchNodeName( Node );

                getWriterImpl( Node.NodeId ).write( Node, ForceSave, IsCopy, AllowAuditing );

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
            foreach( CswNbtNodePropWrapper Prop in Node.Properties )
            {
                CswEnumNbtFieldType FT = Prop.getFieldTypeValue();
                if( FT == CswEnumNbtFieldType.Barcode )
                {
                    Prop.AsBarcode.setBarcodeValue();  // does not overwrite
                }
                else if( FT == CswEnumNbtFieldType.Sequence )
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
            getWriterImpl( Node.NodeId ).AuditInsert( Node );
        }

    }//CswNbtNodeWriterNative

}//namespace ChemSW.Nbt
