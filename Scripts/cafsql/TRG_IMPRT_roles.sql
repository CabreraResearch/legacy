CREATE OR REPLACE TRIGGER TRG_IMPRT_roles AFTER INSERT OR DELETE OR UPDATE OF roledescription,rolename,timeout,roleid,deleted ON roles@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.roleid, 'roles@CAFLINK', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.roleid, 'roles@CAFLINK', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.roleid, 'roles@CAFLINK', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.roleid, 'roles@CAFLINK', '', '');      END IF
    
                                END IF;
  
                                END;