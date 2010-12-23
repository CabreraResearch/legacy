//using System;
//using System.Data;
//using System.Collections;
//using System.Text.RegularExpressions;
//using ChemSW.TblDn;
////using ChemSW.RscAdo;
////using CafSecCom;
//using ChemSW.DB;
//using ChemSW.Exceptions;

//namespace ChemSW.Nbt.TableEvents
//{

//    public class CswNbtTableEventBeforeUpdateNodeTypeProps
//    {

//        public CswNbtTableEventBeforeUpdateNodeTypeProps()
//        {
//        }//ctor()

//        /*
//        private void _validateNodeTypeProps( CswTableModEventArgs CswTableModEventArgs )
//        {
//            int NodeTypeId = -1;
//            ArrayList BarCodeIdRows = new ArrayList();
//            foreach( DataRow InsertedRow in CswTableModEventArgs.InsertedRows )
//            {

//                if( !InsertedRow.IsNull( "nodetypeid" ) )
//                {
//                    if( -1 == NodeTypeId ) 
//                    { 
//                        NodeTypeId = CswConvert.ToInt32( InsertedRow["nodetypeid"] );

//                    } else if( CswConvert.ToInt32( InsertedRow["nodetypeid"] ) != NodeTypeId ) 
//                    {
//                        throw( new CswDniException( "Current nodetype_props table contains records for more than one nodetype id; this validation routine would need to be redsigned to handle this contingency; IINS?" ) );
//                    }//

//                    if( !InsertedRow.IsNull( "fieldtypeid" ) )
//                    {
//                        int FieldTypeId = CswConvert.ToInt32( InsertedRow[ "fieldtypeid" ] );
//                        CswTableCaddy FieldTypesCaddy = _CswTableCaddyFactory.makeCswTableCaddy( "field_types" );
//                        FieldTypesCaddy.FilterColumn = "fieldtypeid";
//                        FieldTypesCaddy.RequireOneRow = true;
//                        string FieldTypeName = FieldTypesCaddy[ FieldTypeId ].Table.Rows[ 0 ][ "fieldtype" ].ToString();
//                        if( "barcode" == FieldTypeName.ToLower() )
//                        {
//                            BarCodeIdRows.Add( InsertedRow );
//                        }//

//                    }
//                    else
//                    {
//                        throw ( new CswDniException( "Current nodetype prop record does not have a field type id" ) );
//                    }//

//                }
//                else
//                {
//                    throw ( new CswDniException( "Current nodetype prop record does not have a nodetypeid" ) );
//                }//if-else nodetypeid id is there.

//            }//iterate inserted rows 


//            if( BarCodeIdRows.Count == 1 )
//            {
//                CswTableCaddy NodeTypePropsCaddy = _CswTableCaddyFactory.makeCswTableCaddy( "nodetype_props" );
//                DataRow OneBarCodeRow = ( DataRow ) BarCodeIdRows[ 0 ];
//                string FieldTypeId = OneBarCodeRow[ "fieldtypeid" ].ToString();

//                NodeTypePropsCaddy.WhereClause = "where nodetypeid=" + NodeTypeId + " and fieldtypeid=" + FieldTypeId;

//                if( NodeTypePropsCaddy.Table.Rows.Count > 0 )
//                    throw ( new CswDniException( "The nodetype_prop update table for nodetypeid id  " + NodeTypeId + " has a record of fieldtype barcode, but a record of this field type is already assigned to this nodetype" ) );
//            }
//            else if( BarCodeIdRows.Count > 1 )
//            {
//                throw ( new CswDniException( "The nodetype_prop update table for nodetypeid id  " + NodeTypeId + " contains more than one record of fieldtype barcode" ) );
//            }//if-else 


//        }//_validateNodeTypeProps()
//         */

//        //private ICswTableCaddyFactory _CswTableCaddyFactory = null;
//        public void BeforeUpdateHandler( Object sender, CswTableModEventArgs CswTableModEventArgs )
//        {
//            /*
//            CswTableCaddy CswTableCaddy = ( CswTableCaddy ) sender;
//            _CswTableCaddyFactory = CswTableModEventArgs.CswTableCaddyFactory;

//            _validateNodeTypeProps( CswTableModEventArgs  );

//            foreach( DataRow InsertedRow in CswTableModEventArgs.InsertedRows )
//            {
//                if( ! InsertedRow.IsNull( "nodetypeid" ) )
//                {
//                    int NodeTypeId = CswConvert.ToInt32( InsertedRow[ "nodetypeid" ] );
//                    // Add the prop to the first open slot in the first column on the tab.
//                    CswTableCaddy TabCaddy = _CswTableCaddyFactory.makeCswTableCaddy( "nodetype_tabset" );
//                    DataTable NodeTypeTabs = null;
//                    if (InsertedRow.IsNull("nodetypetabsetid"))
//                    {
//                        TabCaddy.FilterColumn = "nodetypeid";
//                        TabCaddy.addOrderByColumn("taborder");
//                        NodeTypeTabs = TabCaddy[NodeTypeId].Table;
//                    }
//                    else
//                    {
//                        TabCaddy.FilterColumn = "nodetypetabsetid";
//                        TabCaddy.addOrderByColumn("taborder");
//                        NodeTypeTabs = TabCaddy[CswConvert.ToInt32(InsertedRow["nodetypetabsetid"].ToString())].Table;
//                    }
//                    if( NodeTypeTabs.Rows.Count > 0 )
//                    {
//                        int TabId = CswConvert.ToInt32( NodeTypeTabs.Rows[ 0 ][ "nodetypetabsetid" ].ToString() );
//                        InsertedRow[ "nodetypetabsetid" ] = TabId;
//                        InsertedRow[ "display_col" ] = 1;

//                        CswTableCaddy NodeTypePropCaddy = _CswTableCaddyFactory.makeCswTableCaddy( "nodetype_props" );
//                        NodeTypePropCaddy.FilterColumn = "nodetypetabsetid";
//                        NodeTypePropCaddy.addOrderByColumn( "display_col" );
//                        NodeTypePropCaddy.addOrderByColumn( "display_row" );
//                        DataTable NodeTypeProps = NodeTypePropCaddy[ TabId ].Table;
//                        int maxrow = 0;
//                        foreach( DataRow PropRow in NodeTypeProps.Rows )
//                        {
//                            if( PropRow[ "display_col" ].ToString() != "" &&
//                                PropRow[ "display_row" ].ToString() != "" &&
//                                1 == CswConvert.ToInt32( PropRow[ "display_col" ].ToString() ) &&
//                                maxrow < CswConvert.ToInt32( PropRow[ "display_row" ].ToString() ) )
//                            {
//                                maxrow = CswConvert.ToInt32( PropRow[ "display_row" ].ToString() );
//                            }
//                        }
//                        InsertedRow[ "display_row" ] = maxrow + 1;
//                    }
//                    else
//                    {
//                        throw new CswDniException( "NodeType " + NodeTypeId.ToString() + " has no tabs.",
//                                                  "NodeType " + NodeTypeId.ToString() + " has no tabs." );
//                    }
//                }
//                else
//                {
//                    throw new CswDniException( "New Property " + InsertedRow[ "propname" ].ToString() + " has no nodetypeid.",
//                                              "New Property " + InsertedRow[ "propname" ].ToString() + " has no nodetypeid." );
//                }

//            }//iterate inserted node type rows
//            */
//        }//DataTableRowChangedEventHandler

//    }//class CswNbtTableEventBeforeUpdateNodeTypes

//}//namespace ChemSW.Nbt.TableEvents


