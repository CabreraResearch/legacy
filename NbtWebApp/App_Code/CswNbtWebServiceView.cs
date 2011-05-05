using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.SessionState;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;
using Newtonsoft.Json.Linq;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceView
	{
		public enum ItemType
		{
			Root,
			View,
			//ViewCategory, 
			Category,
			Action,
			Report,
			//ReportCategory, 
			Search,
			RecentView,
			Unknown
		};

		private CswNbtResources _CswNbtResources;

		public CswNbtWebServiceView( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

		//public string getViews()
		//{
		//    string ret = string.Empty;
		//    DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false );
		//    foreach( DataRow ViewRow in ViewDT.Rows )
		//    {
		//        ret += "<view id=\"" + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
		//        ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
		//        ret += "/>";
		//    }
		//    return "<views>" + ret + "</views>";
		//}

		public const string ViewTreeSessionKey = "ViewTreeXml";

		// jsTree compatible format
		public string getViewTree( HttpSessionState Session )
		{

			//string ret = string.Empty;
			//DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false );
			//foreach( DataRow ViewRow in ViewDT.Rows )
			//{
			//    ret += "<item id=\"" + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\" rel=\"" + ViewRow["viewmode"].ToString() + "\" >";
			//    ret += "  <content><name>" + ViewRow["viewname"].ToString() + "</name></content>";
			//    ret += "</item>";
			//}
			//return "<root>" + ret + "</root>";

			XmlDocument TreeXmlDoc;
			if( Session[ViewTreeSessionKey] != null )
			{
				TreeXmlDoc = (XmlDocument) Session[ViewTreeSessionKey];
			}
			else
			{
				TreeXmlDoc = new XmlDocument();
				XmlNode DocRoot = TreeXmlDoc.CreateElement( "root" );
				TreeXmlDoc.AppendChild( DocRoot );

				// Views
				DataTable ViewsTable = _CswNbtResources.ViewSelect.getVisibleViews( "lower(NVL(v.category, v.viewname)), lower(v.viewname)", _CswNbtResources.CurrentNbtUser, false, false, NbtViewRenderingMode.Any );

				foreach( DataRow Row in ViewsTable.Rows )
				{
					// BZ 10121
					// This is a performance hit, but since this view list is cached, it's ok
					CswNbtView CurrentView = new CswNbtView( _CswNbtResources );
					CurrentView.LoadXml( Row["viewxml"].ToString() );
					CurrentView.ViewId = CswConvert.ToInt32( Row["nodeviewid"] );

					_makeViewTreeNode( DocRoot, Row["category"].ToString(), ItemType.View, CurrentView.ViewId, CurrentView.ViewName, CurrentView.ViewMode );
				}

				// Actions
				foreach( CswNbtAction Action in _CswNbtResources.Actions )
				{
					if( Action.ShowInList &&
						( Action.Name != CswNbtActionName.View_By_Location || _CswNbtResources.getConfigVariableValue( "loc_use_images" ) != "0" ) &&
							( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser ).CheckActionPermission( Action.Name ) )
					{
						XmlNode ActionNode = _makeViewTreeNode( DocRoot, Action.Category, ItemType.Action, Action.ActionId, Action.DisplayName );
						CswXmlDocument.AppendXmlAttribute( ActionNode, "actionurl", Action.Url.ToString() );
					}
				}


				// Reports
				CswNbtMetaDataObjectClass ReportMetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
				CswNbtView ReportView = ReportMetaDataObjectClass.CreateDefaultView();
				ReportView.ViewName = "CswViewTree.DataBinding.ReportView";
				ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( ReportView, true, true, false, false );
				for( int i = 0; i < ReportTree.getChildNodeCount(); i++ )
				{
					ReportTree.goToNthChild( i );

					CswNbtObjClassReport ReportNode = CswNbtNodeCaster.AsReport( ReportTree.getNodeForCurrentPosition() );
					_makeViewTreeNode( DocRoot, ReportNode.Category.Text, ItemType.Report, ReportNode.NodeId.PrimaryKey, ReportNode.ReportName.Text );

					ReportTree.goToParentNode();
				}

				Session[ViewTreeSessionKey] = TreeXmlDoc;

			}

			string ret = "<result>" +
						 "  <tree>" + TreeXmlDoc.InnerXml + "</tree>" +
						 "  <types> " + _getTypes() + " </types>" +
						 "</result>";

			return ret;
		} // getViewTree()

		private XmlNode _makeViewTreeNode( XmlNode DocRoot, string Category, ItemType Type, Int32 Id, string Text, NbtViewRenderingMode ViewMode = NbtViewRenderingMode.Unknown )
		{
			XmlNode CategoryNode = _getCategoryNode( DocRoot, Category );
			return _makeItemNode( CategoryNode, Type, Id, Text, ViewMode );
		}

		private static XmlNode _makeItemNode( XmlNode ParentNode, ItemType ItemType, Int32 Id, string Text, NbtViewRenderingMode ViewMode = NbtViewRenderingMode.Unknown )
		{
			XmlNode ItemNode = CswXmlDocument.AppendXmlNode( ParentNode, "item" );
			XmlNode ContentNode = CswXmlDocument.AppendXmlNode( ItemNode, "content" );
			XmlNode NameNode = CswXmlDocument.AppendXmlNode( ContentNode, "name" );
			NameNode.InnerText = Text;

			string Type = ItemType.ToString().ToLower();
			string Mode = ViewMode.ToString().ToLower();
			string Rel = Type;

			if( ViewMode != NbtViewRenderingMode.Unknown )
			{
				CswXmlDocument.AppendXmlAttribute( ItemNode, "viewmode", ViewMode.ToString().ToLower() );
				Rel += Mode;
			}

			CswXmlDocument.AppendXmlAttribute( ItemNode, "viewtype", Type );
			CswXmlDocument.AppendXmlAttribute( ItemNode, "rel", Rel );
			CswXmlDocument.AppendXmlAttribute( ItemNode, "id", Rel + "_" + Id.ToString() );
			CswXmlDocument.AppendXmlAttribute( ItemNode, Type + "id", Id.ToString() );

			return ItemNode;
		}

		private Int32 _Catcount = 0;
		private Dictionary<string, XmlNode> CategoryNodes = new Dictionary<string, XmlNode>();

		private XmlNode _getCategoryNode( XmlNode DocRoot, string Category )
		{
			XmlNode CategoryNode = null;
			if( Category != string.Empty )
			{
				if( CategoryNodes.ContainsKey( Category ) )
				{
					CategoryNode = CategoryNodes[Category];
				}
				else
				{
					// Make one
					_Catcount++;
					CategoryNode = _makeItemNode( DocRoot, ItemType.Category, _Catcount, Category );
					CategoryNodes.Add( Category, CategoryNode );
				}
			}
			else
			{
				CategoryNode = DocRoot;
			}
			return CategoryNode;
		} // _getCategoryNode()


		public string _getTypes()
		{
			JObject ReturnObj = new JObject( new JProperty( "default", "" ) );

			string[] types = { "action", "category", "report", "viewtree", "viewgrid", "viewlist" };
			foreach( string type in types )
			{
				bool Selectable = true;
				if( type == "category" )
					Selectable = false;
				ReturnObj.Add( new JProperty( type,
											  JObject.Parse( "{ \"icon\": { \"image\": \"Images/view/" + type + ".gif\" }, " +
															 "\"hover_node\": " + Selectable.ToString().ToLower() + ", " +
															 "\"select_node\": " + Selectable.ToString().ToLower() + " } }" ) ) );
				// this puts quotes around the boolean values
				//new JObject( 
				//    new JProperty( "icon", 
				//        new JObject( 
				//            new JProperty( "image", "Images/view/" + type + @".gif" ) 
				//        ) 
				//    ),
				//    new JProperty( "hover_node", Selectable.ToString().ToLower() )
				//    new JProperty( "select_node", Selectable.ToString().ToLower() )
				//) 
				//    )
				//);
			}
			string ret = ReturnObj.ToString();
			return ret;
		}


		public JObject getViewGrid( bool All )
		{
			JObject ReturnVal = new JObject();
			bool IsAdmin = _CswNbtResources.CurrentNbtUser.IsAdministrator();
			DataTable ViewsTable;
			if( IsAdmin )
			{
				if( All )
				{
					ViewsTable = _CswNbtResources.ViewSelect.getAllViews();
				}
				else
				{
					ViewsTable = _CswNbtResources.ViewSelect.getVisibleViews( true );
				}
			}
			else
			{
				ViewsTable = _CswNbtResources.ViewSelect.getUserViews();
			}

			if( ViewsTable.Columns.Contains( "viewxml" ) )
				ViewsTable.Columns.Remove( "viewxml" );
			if( ViewsTable.Columns.Contains( "roleid" ) )
				ViewsTable.Columns.Remove( "roleid" );
			if( ViewsTable.Columns.Contains( "userid" ) )
				ViewsTable.Columns.Remove( "userid" );
			if( ViewsTable.Columns.Contains( "mssqlorder" ) )
				ViewsTable.Columns.Remove( "mssqlorder" );

			if( !IsAdmin )
			{
				if( ViewsTable.Columns.Contains( "visibility" ) )
					ViewsTable.Columns.Remove( "visibility" );
				if( ViewsTable.Columns.Contains( "username" ) )
					ViewsTable.Columns.Remove( "username" );
				if( ViewsTable.Columns.Contains( "rolename" ) )
					ViewsTable.Columns.Remove( "rolename" );
			}

			CswGridData gd = new CswGridData( _CswNbtResources );
			gd.PkColumn = "nodeviewid";
			ReturnVal = gd.DataTableToJSON( ViewsTable );

			return ReturnVal;
		} // getViewGrid()

		public XElement getViewChildOptions( string ViewXml, string ArbitraryId, Int32 StepNo )
		{
			XElement ret = new XElement( "options" );

			CswNbtView View = new CswNbtView( _CswNbtResources );
			View.LoadXml( ViewXml );

			XmlDocument RDoc = new XmlDocument();

			if( View != null )
            {
                CswNbtViewNode SelectedViewNode = View.FindViewNodeByArbitraryId( ArbitraryId );
                if( View.ViewMode != NbtViewRenderingMode.List || View.Root.ChildRelationships.Count == 0 )
                {
					if( SelectedViewNode is CswNbtViewRelationship )
                    {
						if( StepNo == 3 )
						{
							// Potential child relationships

							CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;
							Int32 CurrentLevel = 0;
							CswNbtViewNode Parent = CurrentRelationship;
							while( !( Parent is CswNbtViewRoot ) )
							{
								CurrentLevel++;
								Parent = Parent.Parent;
							}

							// Child options are all relations to this nodetype
							Int32 CurrentId = CurrentRelationship.SecondId;

							ArrayList Relationships = null;
							if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
							{
								Relationships = getObjectClassRelatedNodeTypesAndObjectClasses( CurrentId, View, CurrentLevel );
							}
							else if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
							{
								Relationships = getNodeTypeRelatedNodeTypesAndObjectClasses( CurrentId, View, CurrentLevel );
							}
							//else
							//    throw new CswDniException( "A Data Misconfiguration has occurred", "CswViewEditor2._initNextOptions() has a selected node which is neither a NodeTypeNode nor an ObjectClassNode" );

							foreach( CswNbtViewRelationship R in Relationships )
							{
								if( !CurrentRelationship.ChildRelationships.Contains( R ) )
								{
									R.Parent = CurrentRelationship;
									string Label = String.Empty;

									if( R.PropOwner == CswNbtViewRelationship.PropOwnerType.First )
									{
										Label = R.SecondName + " (by " + R.PropName + ")";
									}
									else if( R.PropOwner == CswNbtViewRelationship.PropOwnerType.Second )
									{
										Label = R.SecondName + " (by " + R.SecondName + "'s " + R.PropName + ")";
									}

									//if( isSelectable( R.SecondType, R.SecondId ) )
									//    R.Selectable = true;
									//else
									//    R.Selectable = false;

									XmlNode RNode = R.ToXml( RDoc );
									ret.Add(
											new XElement( "option",
												new XAttribute( "value", RNode.OuterXml ),
												new XAttribute( "name", Label ) ) );

								} //  if( !CurrentRelationship.ChildRelationships.Contains( R ) )
							} // foreach( CswNbtViewRelationship R in Relationships )
						} // if( StepNo == 3)
						else
						{
							// Potential child properties

							CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;

							ICollection PropsCollection = null;
							if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
							{
								PropsCollection = _getObjectClassPropsCollection( CurrentRelationship.SecondId );
							}
							else if( CurrentRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
							{
								PropsCollection = _getNodeTypePropsCollection( CurrentRelationship.SecondId );
							}
							else
							{
								throw new CswDniException( "A Data Misconfiguration has occurred", "CswViewEditor.initPropDataTable() has a selected node which is neither a NodeTypeNode nor an ObjectClassNode" );
							}

							foreach( CswNbtMetaDataNodeTypeProp ThisProp in PropsCollection )
							{
								// BZs 7085, 6651, 6644, 7092
								if( ThisProp.FieldTypeRule.SearchAllowed )
								{
									CswNbtViewProperty ViewProp = View.AddViewProperty( null, (CswNbtMetaDataNodeTypeProp) ThisProp );
									if( !CurrentRelationship.Properties.Contains( ViewProp ) )
									{
										ViewProp.Parent = CurrentRelationship;

										string PropName = ViewProp.Name;
										if( !ThisProp.NodeType.IsLatestVersion )
											PropName += "&nbsp;(v" + ThisProp.NodeType.VersionNo + ")";

										XmlNode PropNode = ViewProp.ToXml( RDoc );
										ret.Add(
												new XElement( "option",
													new XAttribute( "value", PropNode.OuterXml ),
													new XAttribute( "name", PropName ) ) );

									} // if( !CurrentRelationship.Properties.Contains( ViewProp ) )
								} // if( ThisProp.FieldTypeRule.SearchAllowed )
							} // foreach (DataRow Row in Props.Rows)
						} // if-else(StepNo == 3)
					} // if( SelectedViewNode is CswNbtViewRelationship )
					else if( SelectedViewNode is CswNbtViewRoot )
                    {
                        // Set NextOptions to be all viewable nodetypes and objectclasses
                        foreach( CswNbtMetaDataNodeType LatestNodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
                        {
                            if( _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, LatestNodeType.NodeTypeId, null, null ) )
                            {
                                // This is purposefully not the typical way of creating CswNbtViewRelationships.
                                CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, LatestNodeType.FirstVersionNodeType, false );
                                R.Parent = SelectedViewNode;
								
								//if( isSelectable( R.SecondType, R.SecondId ) )
								//    R.Selectable = true;
								//else
								//    R.Selectable = false;

                                bool IsChildAlready = false;
								foreach( CswNbtViewRelationship ChildRel in ( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships )
                                {
                                    if( ChildRel.SecondType == R.SecondType && ChildRel.SecondId == R.SecondId )
                                        IsChildAlready = true;
                                }

                                if( !IsChildAlready )
                                {
									XmlNode RNode = R.ToXml( RDoc );
									ret.Add(
											new XElement( "option",
												new XAttribute( "value", RNode.OuterXml ),
												new XAttribute( "name", LatestNodeType.NodeTypeName ) ) );
                                }
                            }
                        }

                        foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
                        {
                            // This is purposefully not the typical way of creating CswNbtViewRelationships.
                            CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, ObjectClass, false );
							R.Parent = SelectedViewNode;
							
							//if( isSelectable( R.SecondType, R.SecondId ) )
							//    R.Selectable = true;
							//else
							//    R.Selectable = false;

							if( !( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships.Contains( R ) )
                            {
								XmlNode RNode = R.ToXml( RDoc );
								ret.Add(
										new XElement( "option",
											new XAttribute( "value", RNode.OuterXml ),
											new XAttribute( "name", "Any " + ObjectClass.ObjectClass ) ) );
                            }
                        }
					} // else if( SelectedViewNode is CswNbtViewRoot )
					else if( SelectedViewNode is CswNbtViewProperty )
					{
						ret.Add(
								new XElement( "option",
									new XAttribute( "value", "" ),
									new XAttribute( "name", "Filters" ) ) );
					}
					else if( SelectedViewNode is CswNbtViewPropertyFilter )
					{
					}

                } // if( _View.ViewMode != NbtViewRenderingMode.List || _View.Root.ChildRelationships.Count == 0 )
            } // if( _View != null )

			return ret;
		} // getViewChildOptions()



		#region Helper Functions

		private ArrayList getNodeTypeRelatedNodeTypesAndObjectClasses( Int32 FirstVersionId, CswNbtView View, Int32 Level )
		{
			ArrayList Relationships = new ArrayList();

			// If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
			// rather than things that are related to the provided nodetype.
			// If this is a property grid, then the above rule does not apply to the first level.
			bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid ) &&
							( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

			CswNbtMetaDataNodeType FirstVersionNodeType = _CswNbtResources.MetaData.getNodeType( FirstVersionId );
			CswNbtMetaDataNodeType LatestVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( FirstVersionNodeType );
			CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.ObjectClass;

			CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForNodeTypeId_select", "getRelationsForNodeTypeId" );
			RelationshipPropsSelect.S4Parameters.Add( "getnodetypeid", FirstVersionNodeType.NodeTypeId );
			//RelationshipPropsQueryCaddy.S4Parameters.Add("getroleid", _CswNbtResources.CurrentUser.RoleId);
			DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

			foreach( DataRow PropRow in RelationshipPropsTable.Rows )
			{
				// Ignore relationships that don't have a target
				if( PropRow["fktype"].ToString() != String.Empty &&
					PropRow["fkvalue"].ToString() != String.Empty )
				{
					CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );

					if( ( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString() &&
						  PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
						( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
						  PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) )
					{
						if( _CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, FirstVersionNodeType.NodeTypeId, null, null ) )
						{
							// Special case -- relationship to my own type
							// We need to create two relationships from this

							CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
							//CswNbtViewRelationship R1 = View.MakeEmptyViewRelationship();
							//R1.setProp( CswNbtViewRelationship.PropOwnerType.First, ThisProp );
							//R1.setFirst( FirstVersionNodeType );
							//R1.setSecond( FirstVersionNodeType );
							Relationships.Add( R1 );

							if( !Restrict )
							{
								// Copy it
								//CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
								//R2.setProp( CswNbtViewRelationship.PropOwnerType.Second, ThisProp );
								CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
								Relationships.Add( R2 );
							}
						}
					}
					else if( ( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString() &&
							   PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
							 ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
							   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
					{
						// Special case -- relationship to my own class
						// We need to create two relationships from this

						CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
						//R1.setProp( CswNbtViewRelationship.PropOwnerType.First, ThisProp );
						R1.overrideFirst( FirstVersionNodeType );
						R1.overrideSecond( ObjectClass );
						Relationships.Add( R1 );

						if( !Restrict )
						{
							// Copy it
							//CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
							//R2.setProp( CswNbtViewRelationship.PropOwnerType.Second, ThisProp );
							CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
							R2.overrideFirst( FirstVersionNodeType );
							R2.overrideSecond( ObjectClass );
							Relationships.Add( R2 );
						}
					}
					else
					{
						CswNbtViewRelationship R = null;
						if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.NodeTypePropId.ToString() &&
							PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() )
						{
							// my relation to something else
							R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
							if( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
								R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
							else
								R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );

							if( R.SecondType != CswNbtViewRelationship.RelatedIdType.NodeTypeId ||
								_CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, R.SecondId, null, null ) )
							{
								Relationships.Add( R );
							}
						}
						else if( ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
								   PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) ||
								 ( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
								   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
						{
							if( !Restrict )
							{
								// something else's relation to me or my object class
								R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
								if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() )
									R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
								else
									R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );

								if( R.SecondType != CswNbtViewRelationship.RelatedIdType.NodeTypeId ||
									_CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, R.SecondId, null, null ) )
								{
									Relationships.Add( R );
								}
							}
						}
						else
						{
							throw new CswDniException( "An unexpected data condition has occurred", "CswDataSourceNodeType.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original nodetypeid" );
						}
						if( R != null )
							R.overrideFirst( FirstVersionNodeType );

					}
				}
			}

			return Relationships;
		}

		private ArrayList getObjectClassRelatedNodeTypesAndObjectClasses( Int32 ObjectClassId, CswNbtView View, Int32 Level )
		{
			ArrayList Relationships = new ArrayList();

			// If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
			// rather than things that are related to the provided nodetype.
			// If this is a property grid, then the above rule does not apply to the first level.
			bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid ) &&
							( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

			CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );

			CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForObjectClassId_select", "getRelationsForObjectClassId" );
			RelationshipPropsSelect.S4Parameters.Add( "getobjectclassid", ObjectClassId );
			DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

			foreach( DataRow PropRow in RelationshipPropsTable.Rows )
			{
				// Ignore relationships that don't have a target
				if( PropRow["fktype"].ToString() != String.Empty &&
					 PropRow["fkvalue"].ToString() != String.Empty )
				{
					if( ( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() &&
						  PropRow["typeid"].ToString() == ObjectClassId.ToString() ) &&
						( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() &&
						  PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) )
					{
						CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );

						// Special case -- relationship to my own class
						// We need to create two relationships from this
						CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
						R1.overrideFirst( ObjectClass );
						R1.overrideSecond( ObjectClass );
						Relationships.Add( R1 );

						if( !Restrict )
						{
							// Copy it
							//CswNbtViewRelationship R2 = new CswNbtViewRelationship( _CswNbtResources, View, R1 );
							//R2.setProp( CswNbtViewRelationship.PropOwnerType.Second, ThisProp );
							CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
							R2.overrideFirst( ObjectClass );
							R2.overrideSecond( ObjectClass );
							Relationships.Add( R2 );
						}
					}
					else
					{
						CswNbtViewRelationship R = null;
						if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() &&
							PropRow["typeid"].ToString() == ObjectClassId.ToString() )
						{
							// my relation to something else
							CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
							R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.First, ThisProp, false );
							if( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
								R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
							else
								R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
							R.overrideFirst( ObjectClass );
							Relationships.Add( R );
						}
						else if( PropRow["fktype"].ToString() == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() && PropRow["fkvalue"].ToString() == ObjectClassId.ToString() )
						{
							if( !Restrict )
							{
								// something else's relation to me
								if( PropRow["proptype"].ToString() == CswNbtViewRelationship.PropIdType.ObjectClassPropId.ToString() )
								{
									CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
									R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
									R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
								}
								else
								{
									CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
									R = View.AddViewRelationship( null, CswNbtViewRelationship.PropOwnerType.Second, ThisProp, false );
									R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
								}
								R.overrideFirst( ObjectClass );
								Relationships.Add( R );
							}
						}
						else
						{
							throw new CswDniException( "An unexpected data condition has occurred", "CswDataSourceObjectClass.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original objectclassid" );
						}
					}
				}
			}

			return Relationships;
		}


		private ICollection _getNodeTypePropsCollection( Int32 NodeTypeId )
		{
			// Need to generate a set of all Props, including latest version props and
			// all historical ones from previous versions that are no longer included in the latest.
			SortedList PropsByName = new SortedList();
			SortedList PropsById = new SortedList();

			CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
			CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getLatestVersion( NodeType );
			while( ThisVersionNodeType != null )
			{
				foreach( CswNbtMetaDataNodeTypeProp ThisProp in ThisVersionNodeType.NodeTypeProps )
				{
					//string ThisKey = ThisProp.PropName.ToLower(); //+ "_" + ThisProp.FirstPropVersionId.ToString();
					if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
						!PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
					{
						PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
						PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
					}
				}
				ThisVersionNodeType = ThisVersionNodeType.PriorVersionNodeType;
			}
			return PropsByName.Values;
		}

		private ICollection _getObjectClassPropsCollection( Int32 ObjectClassId )
		{
			// Need to generate all properties on all nodetypes of this object class
			SortedList AllProps = new SortedList();
			CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
			foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
			{
				ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
				foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeProps )
				{
					string ThisKey = NodeTypeProp.PropName.ToLower(); //+ "_" + NodeTypeProp.FirstPropVersionId.ToString();
					if( !AllProps.ContainsKey( ThisKey ) )
						AllProps.Add( ThisKey, NodeTypeProp );
				}
			}
			return AllProps.Values;
		}

		#endregion Helper Functions

	} // class CswNbtWebServiceView

} // namespace ChemSW.Nbt.WebServices
