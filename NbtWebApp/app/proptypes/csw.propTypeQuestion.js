/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.question = Csw.properties.question ||
        Csw.properties.register('question',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };

                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.checkCompliance = function() {
                        var defaultText = (false === cswPrivate.multi) ? '' : Csw.enums.multiEditDefaultValue;
                        //if (false === multi) {//cases 26445 and 26442
                        var splitCompliantAnswers = cswPrivate.compliantAnswers.split(',');
                        var isCompliant = true;
                        var selectedAnswer = cswPrivate.answerSel.val();
                        var correctiveAction = cswPrivate.correctiveActionTextBox.val();

                        if (correctiveAction === defaultText) {
                            if (selectedAnswer !== defaultText) {
                                isCompliant = false;
                                for (var i = 0; i < splitCompliantAnswers.length; i += 1) {
                                    isCompliant = isCompliant || (Csw.string(splitCompliantAnswers[i]).trim().toLowerCase() === Csw.string(selectedAnswer).trim().toLowerCase());
                                }
                            }
                            cswPrivate.correctiveActionLabel.hide();
                            cswPrivate.correctiveActionTextBox.hide();
                        }
                        var showCorrectiveAction = (false === isCompliant) && cswPrivate.isActionRequired;
                        if (false === showCorrectiveAction) {
                            cswPrivate.answerSel.removeClass('CswFieldTypeQuestion_Deficient');
                            cswPrivate.correctiveActionLabel.hide();
                            cswPrivate.correctiveActionTextBox.hide();
                        } else {
                            cswPrivate.answerSel.addClass('CswFieldTypeQuestion_Deficient');
                            cswPrivate.correctiveActionLabel.show();
                            cswPrivate.correctiveActionTextBox.show();
                        }
                        //}
                    }; // checkCompliance()


                    cswPrivate.answer = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.answer).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.allowedAnswers = Csw.string(cswPrivate.propVals.allowedanswers).trim();
                    cswPrivate.compliantAnswers = Csw.string(cswPrivate.propVals.compliantanswers).trim();
                    cswPrivate.comments = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.comments).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.correctiveAction = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.correctiveaction).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.multi = cswPublic.data.isMulti();
                    cswPrivate.dateAnswered = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.dateanswered.date).trim() : '';
                    cswPrivate.dateCorrected = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.datecorrected.date).trim() : '';
                    cswPrivate.isActionRequired = Csw.bool(cswPrivate.propVals.isactionrequired); //case 25035
                    
                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.div();
                        cswPublic.control.append('Answer: ' + cswPrivate.answer);
                        if (cswPrivate.dateAnswered !== '') {
                            cswPublic.control.append(' (' + cswPrivate.dateAnswered + ')');
                        }
                        cswPublic.control.br();
                        cswPublic.control.append('Corrective Action: ' + cswPrivate.correctiveAction);
                        if (cswPrivate.dateCorrected !== '') {
                            cswPublic.control.append(' (' + cswPrivate.dateCorrected + ')');
                        }
                        cswPublic.control.br();
                        cswPublic.control.append('Comments: ' + cswPrivate.comments);
                        cswPublic.control.br();
                    } else {
                        cswPublic.control = cswPrivate.parent.table({
                            ID: Csw.makeId(cswPublic.data.ID, 'tbl'),
                            FirstCellRightAlign: true
                        });

                        cswPublic.control.cell(1, 1).text('Answer');
                        cswPrivate.splitAnswers = cswPrivate.allowedAnswers.split(',');
                        if (cswPublic.data.isMulti()) {
                            cswPrivate.splitAnswers.push(Csw.enums.multiEditDefaultValue);
                        } else {
                            cswPrivate.splitAnswers.push('');
                        }
                        cswPrivate.answerSel = cswPublic.control.cell(1, 2)
                                              .select({
                                                  ID: cswPublic.data.ID + '_ans',
                                                  onChange: function () {
                                                      cswPrivate.checkCompliance();
                                                      var val = cswPrivate.answerSel.val();
                                                      Csw.tryExec(cswPublic.data.onChange, val);
                                                      cswPublic.data.onPropChange({ answer: val });
                                                  },
                                                  values: cswPrivate.splitAnswers,
                                                  selected: cswPrivate.answer
                                              });

                        cswPrivate.correctiveActionLabel = cswPublic.control.cell(2, 1).text('Corrective Action');
                        cswPrivate.correctiveActionTextBox = cswPublic.control.cell(2, 2).textArea({
                            ID: cswPublic.data.ID + '_cor',
                            text: cswPrivate.correctiveAction,
                            onChange: function () {
                                cswPrivate.checkCompliance();
                                var val = cswPrivate.correctiveActionTextBox.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ correctiveaction: val });
                            }
                        });

                        cswPublic.control.cell(3, 1).text('Comments');
                        cswPrivate.commentsArea = cswPublic.control.cell(3, 2).textArea({
                            ID: cswPublic.data.ID + '_com',
                            text: cswPrivate.comments,
                            onChange: function () {
                                var val = cswPrivate.commentsArea.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ comments: val });
                            }
                        });

                        cswPrivate.checkCompliance();
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
