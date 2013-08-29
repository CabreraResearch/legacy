CREATE OR REPLACE TRIGGER TRG_IMPRT_roles AFTER INSERT OR DELETE OR UPDATE OF roledescription,rolename,timeout,roleid,deleted ON roles@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.roleid, 'roles', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.roleid, 'roles', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.roleid, 'roles', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.roleid, 'roles', '', '');      END IF
    
                                END IF;
  
                                END;