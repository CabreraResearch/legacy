/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeQuestion';
    var multi = false;
    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var answer = (false === o.Multi) ? Csw.string(propVals.answer).trim() : Csw.enums.multiEditDefaultValue;
            var allowedAnswers = Csw.string(propVals.allowedanswers).trim();
            var compliantAnswers = Csw.string(propVals.compliantanswers).trim();
            var comments = (false === o.Multi) ? Csw.string(propVals.comments).trim() : Csw.enums.multiEditDefaultValue;
            var correctiveAction = (false === o.Multi) ? Csw.string(propVals.correctiveaction).trim() : Csw.enums.multiEditDefaultValue;
            multi = o.Multi;

            var dateAnswered = (false === o.Multi) ? Csw.string(propVals.dateanswered.date).trim() : '';
            var dateCorrected = (false === o.Multi) ? Csw.string(propVals.datecorrected.date).trim() : '';

            if (o.ReadOnly) {
                propDiv.append('Answer: ' + answer);
                if (dateAnswered !== '') {
                    propDiv.append(' (' + dateAnswered + ')');
                }
                propDiv.br();
                propDiv.append('Corrective Action: ' + correctiveAction);
                if (dateCorrected !== '') {
                    propDiv.append(' (' + dateCorrected + ')');
                }
                propDiv.br();
                propDiv.append('Comments: ' + comments);
                propDiv.br();
            } else {
                var table = propDiv.table({
                    ID: Csw.makeId(o.ID, 'tbl'),
                    FirstCellRightAlign: true
                });

                table.cell(1, 1).text('Answer');
                var splitAnswers = allowedAnswers.split(',');
                if (o.Multi) {
                    splitAnswers.push(Csw.enums.multiEditDefaultValue);
                } else {
                    splitAnswers.push('');
                }
                var answerSel = table.cell(1, 2)
                                      .select({
                                          ID: o.ID + '_ans',
                                          onChange: function () {
                                              checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox);
                                              o.onChange();
                                          },
                                          values: splitAnswers,
                                          selected: answer
                                      });

                var correctiveActionLabel = table.cell(2, 1).text('Corrective Action');
                var correctiveActionTextBox = table.cell(2, 2).textArea({
                    ID: o.ID + '_cor',
                    text: correctiveAction,
                    onChange: function () {
                        checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox);
                        o.onChange();
                    }
                });

                table.cell(3, 1).text('Comments');
                table.cell(3, 2).textArea({
                    ID: o.ID + '_com',
                    text: comments,
                    onChange: o.onChange
                });

                checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox);
            }
        },
        save: function (o) {
            var attributes = {
                answer: null,
                correctiveaction: null,
                comments: null
            };
            var answer = o.propDiv.find('#' + o.ID + '_ans');
            if (false === Csw.isNullOrEmpty(answer, true)) {
                attributes.answer = answer.val();
            }
            var correct = o.propDiv.find('#' + o.ID + '_cor');
            if (false === Csw.isNullOrEmpty(correct, true)) {
                attributes.correctiveaction = correct.val();
            }
            var comments = o.propDiv.find('#' + o.ID + '_com');
            if (false === Csw.isNullOrEmpty(comments, true)) {
                attributes.comments = comments.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    function checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox) {
        if (false === multi) {
            var splitCompliantAnswers = compliantAnswers.split(',');
            var isCompliant = true;
            var selectedAnswer = answerSel.val();
            var correctiveAction = correctiveActionTextBox.val();

            if (selectedAnswer !== '' && correctiveAction === '') {
                isCompliant = false;
                for (var i = 0; i < splitCompliantAnswers.length; i += 1) {
                    isCompliant = isCompliant || (Csw.string(splitCompliantAnswers[i]).trim().toLowerCase() === Csw.string(selectedAnswer).trim().toLowerCase());
                }
            }
            if (isCompliant) {
                answerSel.removeClass('CswFieldTypeQuestion_OOC');
                if (correctiveAction === '') {
                    correctiveActionLabel.hide();
                    correctiveActionTextBox.hide();
                }
            } else {
                answerSel.addClass('CswFieldTypeQuestion_OOC');
                correctiveActionLabel.show();
                correctiveActionTextBox.show();
            }
        }
    } // checkCompliance()

    // Method calling logic
    $.fn.CswFieldTypeQuestion = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }
    };
})(jQuery);
