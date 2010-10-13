-- Constraints for NBT Schema

-- Primary Key

alter table actions                 add constraint actionid_pk                primary key (actionid);  
ALTER TABLE configuration_variables add CONSTRAINT configuration_variables_pk PRIMARY KEY (configurationvariableid); 
ALTER TABLE data_dictionary         add CONSTRAINT data_dictionary_pk         PRIMARY KEY (tablecolid); 
ALTER TABLE field_types             add CONSTRAINT field_types_pk             PRIMARY KEY (fieldtypeid); 
ALTER TABLE fkey_definitions        add CONSTRAINT fkey_definitions_pk        PRIMARY KEY (fkeydefid); 
alter table jct_modules_actions     add constraint jctmoduleactionid_pk       primary key (jctmoduleactionid);
ALTER TABLE jct_modules_objectclass add CONSTRAINT jct_modules_objectclass_pk PRIMARY KEY (jctmoduleobjectclassid); 
ALTER TABLE jct_nodes_props         add CONSTRAINT jct_nodes_props_pk         PRIMARY KEY (jctnodepropid); 
ALTER TABLE jct_nodes_props_audit   add CONSTRAINT jct_nodes_props_audit_pk   PRIMARY KEY (jctnodepropauditid); 
ALTER TABLE modules                 add CONSTRAINT modules_pk                 PRIMARY KEY (moduleid); 
ALTER TABLE node_views              add CONSTRAINT node_views_pk              PRIMARY KEY (nodeviewid); 
ALTER TABLE nodes                   add CONSTRAINT nodes_pk                   PRIMARY KEY (nodeid); 
ALTER TABLE nodes_audit             add CONSTRAINT nodes_audit_pk             PRIMARY KEY (nodeauditid); 
ALTER TABLE nodetype_props          add CONSTRAINT nodetype_props_pk          PRIMARY KEY (nodetypepropid); 
ALTER TABLE nodetype_props_audit    add CONSTRAINT nodetype_props_audit_pk    PRIMARY KEY (nodetypepropauditid); 
ALTER TABLE nodetype_tabset         add CONSTRAINT nodetype_tabset_pk         PRIMARY KEY (nodetypetabsetid); 
ALTER TABLE nodetypes               add CONSTRAINT nodetypes_pk               PRIMARY KEY (nodetypeid); 
ALTER TABLE nodetypes_audit         add CONSTRAINT nodetypes_audit_pk         PRIMARY KEY (nodetypeauditid); 
ALTER TABLE object_class            add CONSTRAINT object_class_pk            PRIMARY KEY (objectclassid); 
ALTER TABLE object_class_props      add CONSTRAINT object_class_props_pk      PRIMARY KEY (objectclasspropid); 
ALTER TABLE sequences               add CONSTRAINT sequences_pk               PRIMARY KEY (sequenceid); 
ALTER TABLE static_sql_selects      add CONSTRAINT static_sql_selects_pk      PRIMARY KEY (staticsqlselectid); 
alter table statistics              add constraint statisticsid_pk            primary key (statisticsid);
alter table statistics_nodetypes    add constraint statisticsnodetypeid_pk    primary key (statisticsnodetypeid);
alter table statistics_reports      add constraint statisticsreportid_pk      primary key (statisticsreportid);
alter table statistics_views        add constraint statisticsviewid_pk        primary key (statisticsviewid);
commit;


-- Foreign Key

ALTER TABLE jct_modules_actions     add CONSTRAINT jma_actionid_fk           FOREIGN KEY (actionid)          REFERENCES actions            (actionid);
ALTER TABLE jct_modules_actions     add CONSTRAINT jma_moduleid_fk           FOREIGN KEY (moduleid)          REFERENCES modules            (moduleid);
ALTER TABLE jct_modules_objectclass add CONSTRAINT jmo_moduleid_fk           FOREIGN KEY (moduleid)          REFERENCES modules            (moduleid);
ALTER TABLE jct_modules_objectclass add CONSTRAINT jmo_objectclassid_fk      FOREIGN KEY (objectclassid)     REFERENCES object_class       (objectclassid);
ALTER TABLE jct_nodes_props         add CONSTRAINT jnp_nodeid_fk             FOREIGN KEY (nodeid)            REFERENCES nodes              (nodeid);
ALTER TABLE jct_nodes_props         add CONSTRAINT jnp_nodetypepropid_fk     FOREIGN KEY (nodetypepropid)    REFERENCES nodetype_props     (nodetypepropid);
ALTER TABLE node_views              add CONSTRAINT nv_roleid_fk              FOREIGN KEY (roleid)            REFERENCES nodes              (nodeid);
ALTER TABLE node_views              add CONSTRAINT nv_userid_fk              FOREIGN KEY (userid)            REFERENCES nodes              (nodeid);
ALTER TABLE nodes                   add CONSTRAINT n_nodetypeid_fk           FOREIGN KEY (nodetypeid)        REFERENCES nodetypes          (nodetypeid);
ALTER TABLE nodetype_props          add CONSTRAINT ntp_fieldtypeid_fk        FOREIGN KEY (fieldtypeid)       REFERENCES field_types        (fieldtypeid);
ALTER TABLE nodetype_props          add CONSTRAINT ntp_nodetypeid_fk         FOREIGN KEY (nodetypeid)        REFERENCES nodetypes          (nodetypeid);
ALTER TABLE nodetype_props          add CONSTRAINT ntp_nodetypetabsetid_fk   FOREIGN KEY (nodetypetabsetid)  REFERENCES nodetype_tabset    (nodetypetabsetid);
ALTER TABLE nodetype_props          add CONSTRAINT ntp_nodeviewid_fk         FOREIGN KEY (nodeviewid)        REFERENCES node_views         (nodeviewid);
ALTER TABLE nodetype_props          add CONSTRAINT ntp_objectclasspropid_fk  FOREIGN KEY (objectclasspropid) REFERENCES object_class_props (objectclasspropid);
ALTER TABLE nodetype_props          add CONSTRAINT ntp_sequenceid_fk         FOREIGN KEY (sequenceid)        REFERENCES sequences          (sequenceid);
ALTER TABLE nodetype_tabset         add CONSTRAINT ntts_nodetypeid_fk        FOREIGN KEY (nodetypeid)        REFERENCES nodetypes          (nodetypeid);
ALTER TABLE nodetypes               add CONSTRAINT nt_objectclassid_fk       FOREIGN KEY (objectclassid)     REFERENCES object_class       (objectclassid);
ALTER TABLE object_class_props      add CONSTRAINT ocp_fieldtypeid_fk        FOREIGN KEY (fieldtypeid)       REFERENCES field_types        (fieldtypeid);
ALTER TABLE object_class_props      add CONSTRAINT ocp_objectclassid_fk      FOREIGN KEY (objectclassid)     REFERENCES object_class       (objectclassid);
ALTER TABLE static_sql_selects      add CONSTRAINT s4_datadictionaryid_fk    FOREIGN KEY (datadictionaryid)  REFERENCES data_dictionary    (tablecolid);
ALTER TABLE statistics_views        add CONSTRAINT sv_statisticsid_fk        FOREIGN KEY (statisticsid)      REFERENCES statistics         (statisticsid);
ALTER TABLE statistics_nodetypes    add CONSTRAINT so_statisticsid_fk        FOREIGN KEY (statisticsid)      REFERENCES statistics         (statisticsid);
ALTER TABLE statistics_reports      add CONSTRAINT sr_statisticsid_fk        FOREIGN KEY (statisticsid)      REFERENCES statistics         (statisticsid);
commit;
