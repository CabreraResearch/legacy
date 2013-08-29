CREATE OR REPLACE TRIGGER TRG_IMPRT_users AFTER INSERT OR DELETE OR UPDATE OF disabled,namefirst,namelast,password,email,employeeid,navrows,locked,failedlogincount,defaultlanguage,phone,username,userid,deleted ON users@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.userid, 'users@CAFLINK', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.userid, 'users@CAFLINK', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.userid, 'users@CAFLINK', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.userid, 'users@CAFLINK', '', '');      END IF
    
                                END IF;
  
                                END;