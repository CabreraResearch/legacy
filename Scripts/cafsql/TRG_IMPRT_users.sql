CREATE OR REPLACE TRIGGER TRG_IMPRT_users AFTER INSERT OR DELETE OR UPDATE OF disabled,namefirst,namelast,password,email,employeeid,navrows,locked,failedlogincount,defaultlanguage,phone,username,defaultlocationid,roleid,workunitid,userid,deleted ON users@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.userid, 'users', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.userid, 'users', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.userid, 'users', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.userid, 'users', '', '');      END IF
    
                                END IF;
  
                                END;