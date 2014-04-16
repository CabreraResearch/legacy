using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CswUpdateSchemaPLSQLViews
    /// </summary>    
    public class CswUpdateSchemaPLSQLViews
    {
        public sealed class Views : CswEnum<Views>
        {
            #region Properties and ctor

            private Views( string Dev, Int32 CaseNo, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            static Views() { }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<Views> _All { get { return All; } }
            public static implicit operator Views( string str )
            {
                Views ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region NBTDATA

            public static readonly Views NBTDATA = new Views( CswEnumDeveloper.NBT, 0,
            @"CREATE OR REPLACE NOFORCE VIEW NBTDATA
  AS select n.nodeid, n.nodename,  t.nodetypeid, t.nodetypename, o.objectclassid, o.objectclass,
p.nodetypepropid, p.propname, op.objectclasspropid, op.propname objectclasspropname, f.fieldtype,
j.jctnodepropid, j.field1, j.field2, j.field3, j.field4, j.field5, j.field1_fk, j.gestalt, j.clobdata
from nodes n
join nodetypes t on n.nodetypeid = t.nodetypeid
join object_class o on t.objectclassid = o.objectclassid
join nodetype_props p on p.nodetypeid = t.nodetypeid
join field_types f on p.fieldtypeid = f.fieldtypeid
left outer join object_class_props op on p.objectclasspropid = op.objectclasspropid
left outer join jct_nodes_props j on (j.nodeid = n.nodeid and j.nodetypepropid = p.nodetypepropid)
order by lower(n.nodename), lower(p.propname)" );

            #endregion NBTDATA

            #region VWNTPROPDEFS

            public static readonly Views VwNPV = new Views( CswEnumDeveloper.NBT, 0,
            @"CREATE OR REPLACE FORCE VIEW VWNPV (NID, GESTALT, FIELD1_FK, FIELD1_DATE, FIELD2_DATE, FIELD1_NUMERIC, FIELD2_NUMERIC, FIELD3_NUMERIC, FIELD1, FIELD2, FIELD3, FIELD4, FIELD5, CLOBDATA, NTPID, OCPID)
            AS
              SELECT j.nodeid nid,
                TO_CHAR(j.gestalt) gestalt,
                field1_fk,
                field1_date,
                field2_date,
                field1_numeric,
                field2_numeric,
                field3_numeric,
                field1,
                field2,
                field3,
                field4,
                field5,
                TO_CHAR(clobdata) clobdata,
                ntp.nodetypepropid ntpid,
                ntp.objectclasspropid ocpid
              FROM jct_nodes_props j
              JOIN nodetype_props ntp
              ON ntp.nodetypepropid=j.nodetypepropid" );

            #endregion VWNTPROPDEFS

            #region VWNTPROPDEFS

            public static readonly Views VwNtPropDefs = new Views( CswEnumDeveloper.NBT, 0,
            @"create or replace view vwntpropdefs as
            select ntp.nodetypeid,ntp.questionno,ntp.propname,ntp.firstpropversionid nodetypepropid,ft.fieldtype,ft.fieldtypeid,ntp.fktype,ntp.fkvalue,
                   nt.nodetypename,oc.objectclass, ps.name propertyset
              from nodetype_props ntp
              join field_types ft on (ft.fieldtypeid=ntp.fieldtypeid and ft.deleted='0')
              left outer join nodetypes nt on (nt.nodetypeid=ntp.fkvalue and ntp.fktype='NodeTypeId')
              left outer join object_class oc on (oc.objectclassid=ntp.fkvalue and ntp.fktype='ObjectClassId')
              left outer join property_set ps on (ps.propertysetid=ntp.fkvalue and ntp.fktype='PropertySetId')" );

            #endregion VWNTPROPDEFS

            #region VWAUTOVIEWCOLNAMES

            public static readonly Views VWAUTOVIEWCOLNAMES = new Views( CswEnumDeveloper.NBT, 0,
            @"create or replace view VWAUTOVIEWCOLNAMES as
            SELECT unique nodetypename, propname, viewname, colname from ( 
  SELECT 
    n.nodetypename, 
    p.propname, 
    upper(makeintovalidname(n.oraviewname)) VIEWNAME,
    CASE
      WHEN s.subfieldname IS NOT NULL
      THEN 
          CASE
              WHEN f.fieldtype = 'Quantity'
              THEN
                  CASE
                      WHEN s.subfieldname = 'name'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_UOM'
                      WHEN s.subfieldname = 'nodeid'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_UOMID'
                      WHEN s.subfieldname = 'value'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_VAL'
                      WHEN s.subfieldname = 'val_kg'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_VAL_KG'
                      WHEN s.subfieldname = 'val_liters'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_VAL_L'
                      ELSE upper(makeintovalidname(p.oraviewcolname))
                  END
              WHEN f.fieldtype = 'NFPA'
              THEN
                  CASE
                      WHEN s.subfieldname = 'special'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_S'
                      WHEN s.subfieldname = 'health'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_H'
                      WHEN s.subfieldname = 'reactivity'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_R'
                      WHEN s.subfieldname = 'flammability'
                      THEN upper(makeintovalidname(p.oraviewcolname)) || '_F'
                      ELSE upper(makeintovalidname(p.oraviewcolname))
                  END
              WHEN fktype ='ObjectClassId' or fktype ='NodeTypeId'
              THEN upper(makeintovalidname(p.oraviewcolname)) || '_ID'
              ELSE upper(makeintovalidname(p.oraviewcolname))
          END
      ELSE upper(makeintovalidname(p.oraviewcolname))
    END COLNAME
  FROM nodetype_props p
  JOIN nodetypes n ON n.nodetypeid=p.nodetypeid
  JOIN object_class c ON c.objectclassid=n.objectclassid
  JOIN field_types f ON f.fieldtypeid=p.fieldtypeid
  JOIN field_types_subfields s ON s.fieldtypeid=f.fieldtypeid AND s.reportable='1'
  LEFT OUTER JOIN nodetypes ntfk ON ntfk.nodetypeid=p.fkvalue AND p.fktype ='NodeTypeId'
  LEFT OUTER JOIN object_class ocfk ON ocfk.objectclassid=p.fkvalue AND p.fktype ='ObjectClassId')
ORDER BY nodetypename, propname" );
            
            #endregion VWAUTOVIEWCOLNAMES

            #region VWLEGACYVIEWCOLNAMES

            public static readonly Views VWLEGACYVIEWCOLNAMES = new Views( CswEnumDeveloper.NBT, 0,
            @"CREATE OR REPLACE FORCE VIEW VWLEGACYVIEWCOLNAMES
AS
  SELECT n.nodetypename,
    p.propname,
    'NT'
    || upper(alnumonly(n.nodetypename,'')) viewname,
    CASE
      WHEN s.subfieldname IS NULL
      THEN 'P'
        || TO_CHAR(p.firstpropversionid)
      WHEN s.subfieldname IS NOT NULL
      AND fktype           ='ObjectClassId'
      THEN 'P'
        || TO_CHAR(p.firstpropversionid)
        || '_'
        || alnumonly(ocfk.objectclass,'')
        || '_OCFK'
      WHEN s.subfieldname IS NOT NULL
      AND fktype           ='NodeTypeId'
      THEN 'P'
        || TO_CHAR(p.firstpropversionid)
        || '_'
        || alnumonly(ntfk.nodetypename,'')
        || '_NTFK'
      ELSE 'P'
        || TO_CHAR(p.firstpropversionid)
        || '_'
        || s.subfieldname
    END colname
  FROM nodetype_props p
  JOIN nodetypes n
  ON n.nodetypeid=p.nodetypeid
  JOIN object_class c
  ON c.objectclassid=n.objectclassid
  JOIN field_types f
  ON f.fieldtypeid=p.fieldtypeid
  JOIN field_types_subfields s
  ON s.fieldtypeid=f.fieldtypeid
  AND s.reportable='1'
  LEFT OUTER JOIN nodetypes ntfk
  ON ntfk.nodetypeid=p.fkvalue
  AND p.fktype      ='NodeTypeId'
  LEFT OUTER JOIN object_class ocfk
  ON ocfk.objectclassid=p.fkvalue
  AND p.fktype         ='ObjectClassId'
  ORDER BY n.nodetypename,
    p.propname,
    s.subfieldname" );

            #endregion VWLEGACYVIEWCOLNAMES
        }

    }//class CswUpdateSchemaPLSQLViews

}//namespace ChemSW.Nbt.Schema