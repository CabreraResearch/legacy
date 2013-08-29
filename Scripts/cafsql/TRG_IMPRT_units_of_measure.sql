CREATE OR REPLACE TRIGGER TRG_IMPRT_units_of_measure AFTER INSERT OR DELETE OR UPDATE OF unitofmeasurename,convertfromkg_base,unittype,unitofmeasureid,deleted ON units_of_measure@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.unitofmeasureid, 'units_of_measure@CAFLINK', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.unitofmeasureid, 'units_of_measure@CAFLINK', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.unitofmeasureid, 'units_of_measure@CAFLINK', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.unitofmeasureid, 'units_of_measure@CAFLINK', '', '');      END IF
    
                                END IF;
  
                                END;