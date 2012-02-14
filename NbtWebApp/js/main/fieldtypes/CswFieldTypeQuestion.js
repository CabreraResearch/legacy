/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeQuestion';
    var multi = false;
    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();
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
                $Div.append('Answer: ' + answer);
                if (dateAnswered !== '') {
                    $Div.append(' (' + dateAnswered + ')');
                }
                $Div.append('<br/>');
                $Div.append('Corrective Action: ' + correctiveAction);
                if (dateCorrected !== '') {
                    $Div.append(' (' + dateCorrected + ')');
                }
                $Div.append('<br/>');
                $Div.append('Comments: ' + comments + '<br/>');
            } else {
                var table = Csw.controls.table({
                    $parent: $Div,
                    ID: Csw.controls.dom.makeId(o.ID, 'tbl'),
                    FirstCellRightAlign: true
                });

                table.add(1, 1, 'Answer');
                var splitAnswers = allowedAnswers.split(',');
                if (o.Multi) {
                    splitAnswers.push(Csw.enums.multiEditDefaultValue);
                } else {
                    splitAnswers.push('');
                }
                var $AnswerSel = table.cell(1, 2)
                                       .$.CswSelect('init', {
                                           ID: o.ID + '_ans',
                                           onChange: function () {
                                               checkCompliance(compliantAnswers, $AnswerSel, correctiveActionLabel, $CorrectiveActionTextBox);
                                               o.onChange();
                                           },
                                           values: splitAnswers,
                                           selected: answer
                                       });

                var correctiveActionLabel = table.add(2, 1, 'Corrective Action');
                var $CorrectiveActionTextBox = $('<textarea id="' + o.ID + '_cor" />')
                                    .text(correctiveAction)
                                    .change(function () {
                                        checkCompliance(compliantAnswers, $AnswerSel, correctiveActionLabel, $CorrectiveActionTextBox);
                                        o.onChange();
                                    });
                table.add(2, 2, $CorrectiveActionTextBox);
                table.add(3, 1, 'Comments');
                var $comments = $('<textarea id="' + o.ID + '_com" />')
                                    .text(comments)
                                    .change(o.onChange);
                table.add(3, 2, $comments);
                checkCompliance(compliantAnswers, $AnswerSel, correctiveActionLabel, $CorrectiveActionTextBox);
            }
        },
        save: function (o) {
            var attributes = {
                answer: null,
                correctiveaction: null,
                comments: null
            };
            var $answer = o.$propdiv.find('#' + o.ID + '_ans');
            if (false === Csw.isNullOrEmpty($answer, true)) {
                attributes.answer = $answer.val();
            }
            var $correct = o.$propdiv.find('#' + o.ID + '_cor');
            if (false === Csw.isNullOrEmpty($correct, true)) {
                attributes.correctiveaction = $correct.val();
            }
            var $comments = o.$propdiv.find('#' + o.ID + '_com');
            if (false === Csw.isNullOrEmpty($comments, true)) {
                attributes.comments = $comments.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    function checkCompliance(compliantAnswers, $AnswerSel, correctiveActionLabel, $CorrectiveActionTextBox) {
        if (false === multi) {
            var splitCompliantAnswers = compliantAnswers.split(',');
            var isCompliant = true;
            var selectedAnswer = $AnswerSel.val();
            var correctiveAction = $CorrectiveActionTextBox.val();

            if (selectedAnswer !== '' && correctiveAction === '') {
                isCompliant = false;
                for (var i = 0; i < splitCompliantAnswers.length; i++) {
                    isCompliant = isCompliant || (Csw.string(splitCompliantAnswers[i]).trim().toLowerCase() === Csw.string(selectedAnswer).trim().toLowerCase());
                }
            }
            if (isCompliant) {
                $AnswerSel.removeClass('CswFieldTypeQuestion_OOC');
                if (correctiveAction === '') {
                    correctiveActionLabel.hide();
                    $CorrectiveActionTextBox.hide();
                }
            } else {
                $AnswerSel.addClass('CswFieldTypeQuestion_OOC');
                correctiveActionLabel.show();
                $CorrectiveActionTextBox.show();
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
