CREATE OR REPLACE TRIGGER TRG_IMPRT_units_of_measure AFTER INSERT OR DELETE OR UPDATE OF unitofmeasurename,conversionfactor,unittype,unitofmeasureid,deleted ON units_of_measure@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.unitofmeasureid, 'units_of_measure', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.unitofmeasureid, 'units_of_measure', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.unitofmeasureid, 'units_of_measure', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.unitofmeasureid, 'units_of_measure', '', '');      END IF
    
                                END IF;
  
                                END;