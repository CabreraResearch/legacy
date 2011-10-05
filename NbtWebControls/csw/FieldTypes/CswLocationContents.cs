using System;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswLocationContents : CswFieldTypeWebControl
    {
        //private CswNbtNodeKey _SelectedNodeKey;

        public CswLocationContents( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswLocationContents_DataBinding );

            EnsureChildControls();
        }

        protected void CswLocationContents_DataBinding( object sender, EventArgs e )
        {
            try
            {
                //CswNbtNode ThisNode = _CswNbtResources.Nodes[this.NodeKey];
                //Prop = ThisNode.Properties[_CswNbtMetaDataNodeTypeProp];

                //_LocationNavigator.ParentNodeId = ThisNode.NodeId;
                //_LocationNavigator.ParentNodeName = ThisNode.NodeName;
                if( Prop != null )
                {
                    _LocationNavigator.View = Prop.AsLocationContents.View;
                    _LocationNavigator.OnClientSideLocationImageClick = "openEditNodePopupFromNodeId";
                    _LocationNavigator.PropOwnerNodeId = Prop.NodeId;
                    this.Controls.Add( _LocationNavigator );
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public override void Save()
        {
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
        }

        private CswLocationNavigator _LocationNavigator;

        protected override void CreateChildControls()
        {
            try
            {
                _LocationNavigator = new CswLocationNavigator(_CswNbtResources);//, false);
                _LocationNavigator.OnError += new CswErrorHandler(HandleError);
                _LocationNavigator.ID = "locnav";
                _LocationNavigator.MoveMode = false;
                //this.Controls.Add( _LocationNavigator );   see databind

                base.CreateChildControls();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            base.OnPreRender(e);
        }
    }
}
