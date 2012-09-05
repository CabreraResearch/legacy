///// <reference path="~/app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeQuestion';
//    var multi = false;
//    var methods = {
//        init: function (o) {
//            var propDiv = o.propDiv;
//            propDiv.empty();
//            var propVals = o.propData.values;
//            var answer = (false === o.Multi) ? Csw.string(propVals.answer).trim() : Csw.enums.multiEditDefaultValue;
//            var allowedAnswers = Csw.string(propVals.allowedanswers).trim();
//            var compliantAnswers = Csw.string(propVals.compliantanswers).trim();
//            var comments = (false === o.Multi) ? Csw.string(propVals.comments).trim() : Csw.enums.multiEditDefaultValue;
//            var correctiveAction = (false === o.Multi) ? Csw.string(propVals.correctiveaction).trim() : Csw.enums.multiEditDefaultValue;
//            multi = o.Multi;

//            var dateAnswered = (false === o.Multi) ? Csw.string(propVals.dateanswered.date).trim() : '';
//            var dateCorrected = (false === o.Multi) ? Csw.string(propVals.datecorrected.date).trim() : '';

//            var isActionRequired = Csw.bool(propVals.isactionrequired); //case 25035

//            if (o.ReadOnly) {
//                propDiv.append('Answer: ' + answer);
//                if (dateAnswered !== '') {
//                    propDiv.append(' (' + dateAnswered + ')');
//                }
//                propDiv.br();
//                propDiv.append('Corrective Action: ' + correctiveAction);
//                if (dateCorrected !== '') {
//                    propDiv.append(' (' + dateCorrected + ')');
//                }
//                propDiv.br();
//                propDiv.append('Comments: ' + comments);
//                propDiv.br();
//            } else {
//                var table = propDiv.table({
//                    ID: Csw.makeId(o.ID, 'tbl'),
//                    FirstCellRightAlign: true
//                });

//                table.cell(1, 1).text('Answer');
//                var splitAnswers = allowedAnswers.split(',');
//                if (o.Multi) {
//                    splitAnswers.push(Csw.enums.multiEditDefaultValue);
//                } else {
//                    splitAnswers.push('');
//                }
//                var answerSel = table.cell(1, 2)
//                                      .select({
//                                          ID: o.ID + '_ans',
//                                          onChange: function () {
//                                              checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox, isActionRequired);
//                                              o.onChange();
//                                          },
//                                          values: splitAnswers,
//                                          selected: answer
//                                      });

//                var correctiveActionLabel = table.cell(2, 1).text('Corrective Action');
//                var correctiveActionTextBox = table.cell(2, 2).textArea({
//                    ID: o.ID + '_cor',
//                    text: correctiveAction,
//                    onChange: function () {
//                        checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox, isActionRequired);
//                        o.onChange();
//                    }
//                });

//                table.cell(3, 1).text('Comments');
//                table.cell(3, 2).textArea({
//                    ID: o.ID + '_com',
//                    text: comments,
//                    onChange: o.onChange
//                });

//                checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox, isActionRequired);
//            }
//        },
//        save: function (o) {
//            var attributes = {
//                answer: null,
//                correctiveaction: null,
//                comments: null
//            };
//            var compare = {};
//            var answer = o.propDiv.find('#' + o.ID + '_ans');
//            if (false === Csw.isNullOrEmpty(answer, true)) {
//                attributes.answer = answer.val();
//                compare = attributes;
//            }
//            var correct = o.propDiv.find('#' + o.ID + '_cor');
//            if (false === Csw.isNullOrEmpty(correct, true)) {
//                attributes.correctiveaction = correct.val();
//                compare = attributes;
//            }
//            var comments = o.propDiv.find('#' + o.ID + '_com');
//            if (false === Csw.isNullOrEmpty(comments, true)) {
//                attributes.comments = comments.val();
//                compare = attributes;
//            }
//            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
//        }
//    };

//    function checkCompliance(compliantAnswers, answerSel, correctiveActionLabel, correctiveActionTextBox, isActionRequired) {
//        var defaultText = (false === multi) ? '' : Csw.enums.multiEditDefaultValue;
//        //if (false === multi) {//cases 26445 and 26442
//            var splitCompliantAnswers = compliantAnswers.split(',');
//            var isCompliant = true;
//            var selectedAnswer = answerSel.val();
//            var correctiveAction = correctiveActionTextBox.val();

//            if (correctiveAction === defaultText) {
//                if (selectedAnswer !== defaultText) {
//                    isCompliant = false;
//                    for (var i = 0; i < splitCompliantAnswers.length; i += 1) {
//                        isCompliant = isCompliant || (Csw.string(splitCompliantAnswers[i]).trim().toLowerCase() === Csw.string(selectedAnswer).trim().toLowerCase());
//                    }
//                }
//                correctiveActionLabel.hide();
//                correctiveActionTextBox.hide();
//            }
//            if (isCompliant) {
//                answerSel.removeClass('CswFieldTypeQuestion_Deficient');    
//            } else {
//                answerSel.addClass('CswFieldTypeQuestion_Deficient');
//                if (isActionRequired) {//case 25035
//                    correctiveActionLabel.show();
//                    correctiveActionTextBox.show();
//                }
//            }
//        //}
//    } // checkCompliance()

//    // Method calling logic
//    $.fn.CswFieldTypeQuestion = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }
//    };
//})(jQuery);
