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

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.showCorrectiveAction = function () {
                        return (false == cswPrivate.isAnswerCompliant() &&
                            (cswPrivate.isActionRequired || cswPrivate.correctiveAction !== cswPrivate.defaultText));
                    }

                    cswPrivate.isAnswerCompliant = function () {
                        var answerCompliant = false;
                        for (var i = 0; i < cswPrivate.splitCompliantAnswers.length; i += 1) {
                            if (Csw.string(cswPrivate.splitCompliantAnswers[i]).trim().toLowerCase() === Csw.string(cswPrivate.selectedAnswer).trim().toLowerCase()) {
                                answerCompliant = true;
                            }
                        }
                        return answerCompliant;
                    };

                    cswPrivate.checkCompliance = function () {
                        cswPrivate.selectedAnswer = cswPrivate.answerSel.val();
                        cswPrivate.correctiveAction = cswPrivate.correctiveActionTextBox.val();
                        var isCompliant = true;

                        if (cswPrivate.selectedAnswer !== cswPrivate.defaultText && cswPrivate.correctiveAction === cswPrivate.defaultText) {
                            isCompliant = cswPrivate.isAnswerCompliant();
                        }

                        if (isCompliant) {
                            cswPrivate.answerSel.removeClass('CswFieldTypeQuestion_Deficient');
                            if (cswPrivate.showCorrectiveAction()) {
                                cswPrivate.correctiveActionLabel.show();
                                cswPrivate.correctiveActionTextBox.show();
                            } else {
                                cswPrivate.correctiveActionLabel.hide();
                                cswPrivate.correctiveActionTextBox.hide();
                            }
                        } else {
                            cswPrivate.answerSel.addClass('CswFieldTypeQuestion_Deficient');
                            if (cswPrivate.isActionRequired && false == Csw.isNullOrEmpty(cswPrivate.selectedAnswer)) {
                                cswPrivate.correctiveActionLabel.show();
                                cswPrivate.correctiveActionTextBox.show();
                            }
                        }
                    }; // checkCompliance()


                    cswPrivate.answer = Csw.string(cswPrivate.propVals.answer).trim();
                    cswPrivate.allowedAnswers = Csw.string(cswPrivate.propVals.allowedanswers).trim();
                    cswPrivate.compliantAnswers = Csw.string(cswPrivate.propVals.compliantanswers).trim();
                    cswPrivate.comments = Csw.string(cswPrivate.propVals.comments).trim();
                    cswPrivate.correctiveAction = Csw.string(cswPrivate.propVals.correctiveaction).trim();
                    cswPrivate.multi = cswPublic.data.isMulti();
                    cswPrivate.dateAnswered = Csw.string(cswPrivate.propVals.dateanswered.date).trim();
                    cswPrivate.dateCorrected = Csw.string(cswPrivate.propVals.datecorrected.date).trim();
                    cswPrivate.isActionRequired = Csw.bool(cswPrivate.propVals.isactionrequired); //case 25035

                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.div();
                        cswPublic.table = cswPublic.control.table({
                            TableCssClass: 'CswFieldTypeQuestion_table',
                            CellCssClass: 'CSwFieldTypeQuestion_cell'
                        });

                        cswPublic.table.cell(1, 1).label({
                            text: cswPrivate.answer,
                            cssclass: 'CswFieldTypeQuestion_answer'
                        });
                        var answerCell = cswPublic.table.cell(2, 1).div({ cssclass: 'CSwFieldTypeQuestion_cell CSwFieldTypeQuestion_cellHighlight' });
                        answerCell.append('Answer: ' + cswPrivate.answer + ' ');
                        if (cswPrivate.dateAnswered !== '') {
                            answerCell.append(' (' + cswPrivate.dateAnswered + ')');
                        }
                        var correctiveActionPresent = false;
                        if (false == Csw.isNullOrEmpty(cswPrivate.correctiveAction)) {
                            cswPublic.table.cell(3, 1).append('Corrective Action: ' + cswPrivate.correctiveAction);
                            var correctiveActionPresent = true;
                            if (cswPrivate.dateCorrected !== '') {
                                cswPublic.table.cell(3, 1).append(' (' + cswPrivate.dateCorrected + ')');
                            }
                        }
                        var commentsCell;
                        if (correctiveActionPresent) {
                            commentsCell = cswPublic.table.cell(4, 1).div({ cssclass: 'CSwFieldTypeQuestion_cell CSwFieldTypeQuestion_cellHighlight' });
                        } else {
                            commentsCell = cswPublic.table.cell(4, 1).span({ cssclass: 'CSwFieldTypeQuestion_cell' });
                        }
                        commentsCell.append('Comments: ' + cswPrivate.comments);

                    } else {
                        cswPublic.control = cswPrivate.parent.table({
                            FirstCellRightAlign: true
                        });

                        cswPublic.control.cell(1, 1).text('Answer');
                        cswPrivate.splitAnswers = cswPrivate.allowedAnswers.split(',');
                        if (Csw.isNullOrEmpty(cswPrivate.answer)) {
                            cswPrivate.splitAnswers.push('');
                        }


                        cswPrivate.answerSel = cswPublic.control.cell(1, 2)
                                              .select({
                                                  name: cswPublic.data.name + '_ans',
                                                  onChange: function () {
                                                      cswPrivate.propVals = {}
                                                      cswPrivate.checkCompliance();
                                                      var val = cswPrivate.answerSel.val();
                                                      if (false == Csw.isNullOrEmpty(val)) { //once the user has selected something other than the blank answer, remove that blank option from the list of options
                                                          cswPrivate.answerSel.removeOption('');
                                                      }
                                                      Csw.tryExec(cswPublic.data.onChange, val);
                                                      cswPublic.data.onPropChange({ answer: val });
                                                  },
                                                  values: cswPrivate.splitAnswers,
                                                  selected: cswPrivate.answer
                                              });

                        cswPrivate.correctiveActionLabel = cswPublic.control.cell(2, 1).text('Corrective Action');
                        cswPrivate.correctiveActionTextBox = cswPublic.control.cell(2, 2).textArea({
                            name: cswPublic.data.name + '_cor',
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
                            name: cswPublic.data.name + '_com',
                            text: cswPrivate.comments,
                            onChange: function () {
                                var val = cswPrivate.commentsArea.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ comments: val });
                            }
                        });

                        cswPrivate.correctiveActionTextBox.hide();
                        cswPrivate.correctiveActionLabel.hide();

                        cswPrivate.defaultText = (false === cswPrivate.multi) ? '' : Csw.enums.multiEditDefaultValue;
                        cswPrivate.splitCompliantAnswers = cswPrivate.compliantAnswers.split(',');

                        cswPrivate.checkCompliance();

                    }

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());
