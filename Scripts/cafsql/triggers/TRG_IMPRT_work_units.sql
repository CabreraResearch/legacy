CREATE OR REPLACE TRIGGER TRG_IMPRT_work_units AFTER INSERT OR DELETE OR UPDATE OF workunitname,workunitid,deleted ON work_units@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.workunitid, 'work_units', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.workunitid, 'work_units', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.workunitid, 'work_units', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.workunitid, 'work_units', '', '');      END IF
    
                                END IF;
  
                                END;