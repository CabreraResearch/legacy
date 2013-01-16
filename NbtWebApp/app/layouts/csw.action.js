/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.layouts.action = Csw.layouts.action ||
        Csw.layouts.register('action', function (cswParent, cswPrivate) {
            'use strict';
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'action';
                cswPrivate.title = cswPrivate.title || 'An Action';
                if (cswPrivate.useFinish !== false) { cswPrivate.useFinish = true; }
                if (cswPrivate.useCancel !== false) { cswPrivate.useCancel = true; }
                cswPrivate.finishText = cswPrivate.finishText || 'Finish';
                cswPrivate.cancelText = cswPrivate.cancelText || 'Cancel';
                cswPrivate.onFinish = cswPrivate.onFinish || function() {};
                cswPrivate.onCancel = cswPrivate.onCancel || function () {};
                if (cswPrivate.hasButtonGroup !== false) { cswPrivate.hasButtonGroup = true; }
            }());

            cswPublic.setTitle = function(title) {
                cswPrivate.titleCell.empty();
                cswPrivate.titleCell.text(title);
            };

            cswPublic.setHeader = function (header) {
                cswPrivate.headerDiv.empty();
                cswPrivate.headerDiv.text(header);
            };

            (function _postCtor() {
                cswPublic.table = cswParent.table({
                    suffix: cswPrivate.name,
                    TableCssClass: 'CswAction_ActionTable'
                });

                /* Title Cell */
                cswPrivate.titleCell = cswPublic.table.cell(1, 1)
                    .propDom('colspan', 2)
                    .addClass('CswAction_TitleCell');
                cswPublic.setTitle(cswPrivate.title);
                
                cswPrivate.indexCell = cswPublic.table.cell(2, 1)
                    .propDom('rowspan', 2)
                    .addClass('CswAction_IndexCell');

                cswPrivate.contentCell = cswPublic.table.cell(2, 2)
                    .addClass('CswAction_ContentCell');
                
                cswPrivate.headerDiv = cswPrivate.contentCell.div({ cssclass: 'CswAction_HeaderText' });
                cswPrivate.contentCell.br({ number: 2 });

                cswPublic.actionDiv = cswPrivate.contentCell.div();
                
                if (Csw.bool(cswPrivate.hasButtonGroup)) {
                    cswPrivate.btnGroup = cswPublic.table.cell(3, 1).buttonGroup({
                        buttons: {
                            next: { enabled: false, hidden: true },
                            previous: { enabled: false, hidden: true },
                            finish: {
                                enabled: cswPrivate.useFinish,
                                hidden: false === cswPrivate.useFinish,
                                text: cswPrivate.finishText,
                                onclick: function() { Csw.tryExec(cswPrivate.onFinish); }
                            }
                        },
                        cancel: {
                            enabled: cswPrivate.useCancel,
                            hidden: false === cswPrivate.useCancel,
                            text: cswPrivate.cancelText,
                            onclick: function () { Csw.tryExec(cswPrivate.onCancel); }
                        }
                    });
                    cswPublic.finish = cswPrivate.btnGroup.finish;
                    cswPublic.cancel = cswPrivate.btnGroup.cancel;
                }
            } ());
            
            return cswPublic;
        });
} ());

