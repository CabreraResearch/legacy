using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CswUpdateSchemaPLSQLProcedures
    /// </summary>    
    public class CswUpdateSchemaPLSQLProcedures
    {
        public sealed class Procedures : CswEnum<Procedures>
        {
            #region Properties and ctor

            private Procedures( string Dev, Int32 CaseNo, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<Procedures> _All { get { return All; } }
            public static implicit operator Procedures( string str )
            {
                Procedures ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region CORRECT_SEQUENCE_CACHE

            public static readonly Procedures CORRECT_SEQUENCE_CACHE = new Procedures( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE PROCEDURE correct_sequence_cache authid current_user IS
    cursor cur is select columnname, tablename from data_dictionary where columntype = 'pk' and isview= '0' and tablename in ('nodes','jct_nodes_props','nodes_audit','jct_nodes_props_audit','sessionlist','sessiondata','audit_transactions');

    cursor cur2 is select t.sequenceid, replace(t.sequencename, ' ', '') sequencename, x.maxval
                    from sequences t
                    join (select s.sequenceid,
                                (nvl(max(j.field1_numeric),0) + 1) maxval
                            from sequences s
                            left outer join nodetype_props p on (s.sequenceid = p.sequenceid)
                            left outer join jct_nodes_props j on (p.nodetypepropid = j.nodetypepropid)
                            group by s.sequenceid) x on x.sequenceid = t.sequenceid;

    var_maxval NUMBER;
    var_seqval NUMBER;
    var_seqcnt number;
    begin

    -- note: system must:
    --  grant execute on this_user.update_sequences to this_user;
    --  grant create any sequence to this_user;

    -- Table Sequences
    for rec in cur loop
        execute immediate 'alter sequence seq_' || rec.columnname || ' cache 1000';
    end loop;


    -- NBT Sequences
    for rec2 in cur2 loop
    execute immediate 'alter sequence seq_' || rec2.sequencename || ' cache 1000';
    end loop;
end;" );

            #endregion CORRECT_SEQUENCE_CACHE

            #region CREATEALLNTVIEWS

            public static readonly Procedures CREATEALLNTVIEWS = new Procedures( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE procedure CreateAllNtViews is
        cursor nts is
            select nodetypeid,nodetypename,firstversionid from nodetypes
            where firstversionid=nodetypeid order by lower(nodetypename);
        cursor ntsdel is
	    select object_name from user_objects where object_type='VIEW' and object_name like 'NT%' or object_name like 'OC%';
        var_sql varchar2(200);
    begin
        for delrec in ntsdel loop
	    var_sql := 'drop view ' || delrec.object_name;
	    execute immediate (var_sql);
        end loop;
        commit;

        for rec in nts loop
        --dbms_output.put_line('createntview(' || to_char(rec.nodetypeid) || ',' || rec.nodetypename || ')');
        CreateNtView(rec.nodetypeid);
        end loop;
    end CreateAllNtViews;" );

            #endregion CREATEALLNTVIEWS

            #region CREATENTVIEW

            public static readonly Procedures CREATENTVIEW = new Procedures( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE procedure createNTview(ntid in number) is
        cursor props is
        select v.*,s.propcolname subfieldname,s.is_default,s.subfieldname subfieldalias from vwNtPropDefs v
        join field_types_subfields s on s.fieldtypeid=v.fieldtypeid and s.reportable='1'
        where v.nodetypeid=ntid order by lower(propname);
        var_sql varchar2(32000);
        var_line varchar(2000);
        pname varchar2(200);
        pcount number;
        viewname varchar2(30);
        objid number;
        ntcount number;
    begin
        --dbms_output.enable(32000);


        select count(*) into ntcount from nodetypes where nodetypeid=ntid;

        if(ntcount>0) then
        select substr(nodetypename,1,29),objectclassid into viewname,objid from nodetypes where nodetypeid=ntid;

        var_line:='create or replace view ' || OraColLen('NT',alnumonly(viewname,''),'') || ' as select n.nodeid ';
        --dbms_output.put_line(var_line);
        var_sql := var_sql || var_line;
        pcount:=0;

        for rec in props loop
            pcount:=pcount+1;
            --dbms_output.put_line(to_char(pcount) || '|' || safeSqlParam(rec.propname) || '|' || rec.subfieldname || '|' || rec.fieldtype || '|' || rec.objectclass || '|' || rec.nodetypename);
            pname := to_char(rec.nodetypepropid);
            if(rec.is_default='1') then
            var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid);
            var_line := var_line || ')' || OraColLen('P',alnumonly(upper(pname),''),'');
            --dbms_output.put_line(var_line);
            var_sql := var_sql || var_line;
            else
            if(rec.fieldtype='Relationship' or rec.fieldtype='Location') then
                if(rec.fktype='NodeTypeId') then
                var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
                var_line := var_line  || OraColLen('P',alnumonly(upper(pname || '_' || rec.nodetypename),''),'_NTFK');
                --  dbms_output.put_line(var_line);
                var_sql := var_sql || var_line;
                else
                var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
                var_line:=var_line || OraColLen('P',alnumonly(upper(pname || rec.objectclass),''),'_OCFK');
                --  dbms_output.put_line(var_line);
                var_sql := var_sql || var_line;
                end if;
            else
                var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
                var_line:=var_line || OraColLen('P',alnumonly(upper(pname || '_' || rec.subfieldalias),''),'');
                --dbms_output.put_line(var_line);
                var_sql := var_sql || var_line;
            end if;
            end if;
        end loop;

        if(pcount>0) then
        var_line := ' from nodes n where n.nodetypeid=' || to_char(ntid);
        --dbms_output.put_line(var_line);
        var_sql := var_sql || var_line;
        execute immediate (var_sql);
        commit;
        createOCview(objid);
        end if;
        end if;

    end createNTview;" );

            #endregion CREATENTVIEW

            #region CREATEOCVIEW

            public static readonly Procedures CREATEOCVIEW = new Procedures( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE procedure createOCview(objid in number) is
    cursor props is select v.*
    from vwObjProps v
    where v.objectclassid=objid and objectclasspropid is not null order by objectclasspropid;
    var_sql varchar2(20000);
    aline varchar(2000);
    pcount number;
    viewname varchar(30);
    begin
    --dbms_output.enable(21000);
    --dbms_output.put_line(to_char(objid));
    select substr(objectclass,1,29) into viewname from object_class where objectclassid=objid;

    var_sql:='create or replace view ' || OraColLen('OC',alnumonly(viewname,''),'') || ' as select n.nodeid ';
    --dbms_output.put_line(var_sql);
    pcount:=0;

    for rec in props loop
    pcount:=pcount+1;

    --OLD aline:=',(select ' || rec.subfieldname || ' from vwNpvname where nid=n.nodeid and safepropname=''' || rec.safepropname || ''') ' || upper(rec.safepropname);
    aline:=',(select ' || rec.subfieldname || ' from vwNpvname where nid=n.nodeid and objectclasspropid=' || rec.objectclasspropid || ') ' || 'OP_' || to_char(rec.objectclasspropid);
    --dbms_output.put_line(aline);
    var_sql:=var_sql || aline;

    if(rec.fieldtype='Relationship' or rec.fieldtype='Location') then
    --OLD aline:=',(select field1_fk from vwNpvname where nid=n.nodeid and safepropname=''' || rec.safepropname || ''') ' || OraColLen('',upper(rec.safepropname),'_FK');
    aline:=',(select field1_fk from vwNpvname where nid=n.nodeid and objectclasspropid=' || rec.objectclasspropid || ') OP_' || rec.objectclasspropid || '_FK';
    -- dbms_output.put_line(aline);
    var_sql:=var_sql || aline;
    end if;

    end loop;

    if(pcount>0) then
    aline:= ' from nodes n join nodetypes nt on nt.nodetypeid=n.nodetypeid and nt.objectclassid=' || to_char(objid);
    --dbms_output.put_line(aline);
    var_sql:=var_sql || aline;
    --dbms_output.put_line(var_sql);
    execute immediate (var_sql);
    commit;
    end if;

end createOCview;" );

            #endregion CREATEOCVIEW

            #region DROPSEQUENCES

            public static readonly Procedures DROPSEQUENCES = new Procedures( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE procedure DROP_SEQUENCES is
    begin
        DECLARE
        CURSOR cur_objects(obj_type VARCHAR2) IS
            SELECT object_name FROM user_objects WHERE object_type IN (obj_type);

        obj_name VARCHAR(200);
        sql_str  VARCHAR(500);

        BEGIN
        OPEN cur_objects('SEQUENCE');

        LOOP
            FETCH cur_objects
            INTO obj_name;
            EXIT WHEN cur_objects%NOTFOUND;

            sql_str := 'drop SEQUENCE ' || obj_name;
            EXECUTE IMMEDIATE sql_str;

        END LOOP;

        CLOSE cur_objects;
        END;

    end;" );

            #endregion DROPSEQUENCES

            #region UPDATESEQUENCES

            public static readonly Procedures UPDATESEQUENCES = new Procedures( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE PROCEDURE update_sequences authid current_user IS
    cursor cur is select columnname, tablename from data_dictionary where columntype = 'pk' and isview= '0';

    cursor cur2 is select t.sequenceid, replace(t.sequencename, ' ', '') sequencename, x.maxval
                    from sequences t
                    join (select s.sequenceid,
                                (nvl(max(j.field1_numeric),0) + 1) maxval
                            from sequences s
                            left outer join nodetype_props p on (s.sequenceid = p.sequenceid)
                            left outer join jct_nodes_props j on (p.nodetypepropid = j.nodetypepropid)
                            group by s.sequenceid) x on x.sequenceid = t.sequenceid;

    var_maxval NUMBER;
    var_seqval NUMBER;
    var_seqcnt number;
    begin

    -- note: system must:
    --  grant execute on this_user.update_sequences to this_user;
    --  grant create any sequence to this_user;

    -- Table Sequences
    for rec in cur loop
    execute immediate 'select count(*) from user_sequences where lower(sequence_name)=lower(''seq_' || rec.columnname || ''')' into var_seqcnt;

    if(var_seqcnt<1) then

        execute immediate 'create sequence seq_' || rec.columnname || ' minvalue 1 start with 1 increment by 1';
    end if;

    execute immediate 'select nvl(max(' || rec.columnname || '), 1) from ' || rec.tablename into var_maxval;
    execute immediate 'SELECT NVL(last_number,0) FROM user_sequences WHERE lower(sequence_name)=lower(''seq_' || rec.columnname || ''')' into var_seqval;

    if var_maxval >= var_seqval then
        execute immediate 'alter sequence seq_' || rec.columnname || ' increment by ' || to_char(greatest(var_maxval - var_seqval + 1, 1)) || ' ';
        execute immediate 'select seq_' || rec.columnname || '.nextval from dual' into var_seqval;
        execute immediate 'alter sequence seq_' || rec.columnname || ' increment by 1';
    end if;
    end loop;


    -- NBT Sequences
    for rec2 in cur2 loop
    execute immediate 'select count(*) from user_sequences where lower(sequence_name)=lower(''seq_' || rec2.sequencename || ''')' into var_seqcnt;

    if(var_seqcnt<1) then
        execute immediate 'create sequence seq_' || rec2.sequencename || ' minvalue 1 start with 1 increment by 1';
    end if;

    execute immediate 'SELECT NVL(last_number,0) FROM user_sequences WHERE lower(sequence_name)=lower(''seq_' || rec2.sequencename || ''')' into var_seqval;

    if rec2.maxval is not null and rec2.maxval >= var_seqval then
        execute immediate 'alter sequence seq_' || rec2.sequencename || ' increment by ' || to_char(greatest(rec2.maxval - var_seqval + 1, 1)) || ' ';
        execute immediate 'select seq_' || rec2.sequencename || '.nextval from dual' into var_seqval;
        execute immediate 'alter sequence seq_' || rec2.sequencename || ' increment by 1';
    end if;

    end loop;
    end;" );

            #endregion UPDATESEQUENCES
        }

    }//class CswUpdateSchemaPLSQLProcedures

}//namespace ChemSW.Nbt.Schema