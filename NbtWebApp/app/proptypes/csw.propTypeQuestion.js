/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('question', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.showCorrectiveAction = function () {
                return (false == cswPrivate.isAnswerCompliant(cswPrivate.selectedAnswer) &&
                    (cswPrivate.isActionRequired || cswPrivate.correctiveAction !== cswPrivate.defaultText));
            };

            cswPrivate.isAnswerCompliant = function (answer) {
                var answerCompliant = false;
                for (var i = 0; i < cswPrivate.splitCompliantAnswers.length; i += 1) {
                    if (Csw.string(cswPrivate.splitCompliantAnswers[i]).trim().toLowerCase() === Csw.string(answer).trim().toLowerCase()) {
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
                    isCompliant = cswPrivate.isAnswerCompliant(cswPrivate.selectedAnswer);
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


            cswPrivate.answer = Csw.string(nodeProperty.propData.values.answer).trim();
            cswPrivate.allowedAnswers = Csw.string(nodeProperty.propData.values.allowedanswers).trim();
            cswPrivate.compliantAnswers = Csw.string(nodeProperty.propData.values.compliantanswers).trim();
            cswPrivate.comments = Csw.string(nodeProperty.propData.values.comments).trim();
            cswPrivate.correctiveAction = Csw.string(nodeProperty.propData.values.correctiveaction).trim();
            cswPrivate.multi = nodeProperty.isMulti();
            cswPrivate.dateAnswered = Csw.string(nodeProperty.propData.values.dateanswered.date).trim();
            cswPrivate.dateCorrected = Csw.string(nodeProperty.propData.values.datecorrected.date).trim();
            cswPrivate.isActionRequired = Csw.bool(nodeProperty.propData.values.isactionrequired); //case 25035
            cswPrivate.defaultText = (false === cswPrivate.multi) ? '' : Csw.enums.multiEditDefaultValue;
            cswPrivate.splitCompliantAnswers = cswPrivate.compliantAnswers.split(',');

            var table;
            if (nodeProperty.isReadOnly()) {
                var div = nodeProperty.propDiv.div({ cssclass: 'cswInline' });
                table = div.table({
                    TableCssClass: 'CswFieldTypeQuestion_table',
                    CellCssClass: 'CSwFieldTypeQuestion_cell'
                });
                //Answer Row
                var answerTable = table.cell(1, 1).table();
                answerTable.css('width', '250px');
                var label = answerTable.cell(1, 1).label({
                    text: cswPrivate.answer + '   ',
                    cssclass: 'CswFieldTypeQuestion_answer'
                });
                if (false == cswPrivate.isAnswerCompliant(cswPrivate.answer)) {
                    label.img({
                        src: "Images\\newicons\\18\\warning.png"
                    });
                }
                var answerCell = answerTable.cell(1, 2).div({ cssclass: 'CSwFieldTypeQuestion_cell' });
                answerCell.css('text-align', 'right');
                if (cswPrivate.dateAnswered !== '') {
                    answerCell.append(' (' + cswPrivate.dateAnswered + ')');
                }
                //Corrective Action Row
                var correctiveActionPresent = false;
                if (false == Csw.isNullOrEmpty(cswPrivate.correctiveAction)) {
                    var correctiveActionCell = table.cell(3, 1).div({ cssclass: 'CSwFieldTypeQuestion_cell CSwFieldTypeQuestion_cellHighlight' });
                    correctiveActionCell.append('Corrective Action: ' + cswPrivate.correctiveAction);
                    correctiveActionPresent = true;
                    if (cswPrivate.dateCorrected !== '') {
                        correctiveActionCell.append(' (' + cswPrivate.dateCorrected + ')');
                    }
                }
                //Comments Row
                var commentsCell;
                if (false == Csw.isNullOrEmpty(cswPrivate.comments)) {
                    if (correctiveActionPresent) {
                        commentsCell = table.cell(4, 1).span({ cssclass: 'CSwFieldTypeQuestion_cell' });
                    } else {
                        commentsCell = table.cell(4, 1).div({ cssclass: 'CSwFieldTypeQuestion_cell CSwFieldTypeQuestion_cellHighlight' });
                    }
                    commentsCell.append('Comments: ' + cswPrivate.comments);
                }

            } else {
                table = nodeProperty.propDiv.table({
                    FirstCellRightAlign: true
                });

                table.cell(1, 1).text('Answer');
                cswPrivate.splitAnswers = cswPrivate.allowedAnswers.split(',');
                if (Csw.isNullOrEmpty(cswPrivate.answer)) {
                    cswPrivate.splitAnswers.push('');
                }

                cswPrivate.answerSel = table.cell(1, 2)
                    .select({
                        name: nodeProperty.name + '_ans',
                        onChange: function (val) {
                            cswPrivate.checkCompliance();

                            if (false == Csw.isNullOrEmpty(val)) { //once the user has selected something other than the blank answer, remove that blank option from the list of options
                                cswPrivate.answerSel.removeOption('');
                            }
                            nodeProperty.propData.values.answer = val;
                            nodeProperty.broadcastPropChange();
                        },
                        values: cswPrivate.splitAnswers,
                        selected: cswPrivate.answer
                    });

                cswPrivate.correctiveActionLabel = table.cell(2, 1).text('Corrective Action');
                cswPrivate.correctiveActionTextBox = table.cell(2, 2).textArea({
                    name: nodeProperty.name + '_cor',
                    text: cswPrivate.correctiveAction,
                    onChange: function (val) {
                        cswPrivate.checkCompliance();

                        nodeProperty.propData.values.correctiveaction = val;
                        nodeProperty.broadcastPropChange();
                    }
                });

                table.cell(3, 1).text('Comments');
                cswPrivate.commentsArea = table.cell(3, 2).textArea({
                    name: nodeProperty.name + '_com',
                    text: cswPrivate.comments,
                    onChange: function (val) {
                        nodeProperty.propData.values.comments = val;
                        nodeProperty.broadcastPropChange();
                    }
                });

                cswPrivate.correctiveActionTextBox.hide();
                cswPrivate.correctiveActionLabel.hide();

                cswPrivate.checkCompliance();

            }

        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());
