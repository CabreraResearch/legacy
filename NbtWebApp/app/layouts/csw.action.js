/// <reference path="~app/CswApp-vsdoc.js" />


(function () {

    Csw.layouts.action = Csw.layouts.action ||
        Csw.layouts.register('action', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: 'action',
                Title: 'An Action',
                FinishText: 'Finish',
                onFinish: null,
                onCancel: null,
                hasButtonGroup: true
            };
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var cswPublic = {};

            (function () {
                cswPublic.table = cswParent.table({
                    suffix: cswPrivate.ID,
                    TableCssClass: 'CswAction_ActionTable'
                });

                /* Title Cell */
                cswPublic.table.cell(1, 1).text(cswPrivate.Title)
                    .propDom('colspan', 2)
                    .addClass('CswAction_TitleCell');

                cswPrivate.indexCell = cswPublic.table.cell(2, 1)
                    .propDom('rowspan', 2)
                    .addClass('CswAction_IndexCell');

                cswPrivate.contentCell = cswPublic.table.cell(2, 2)
                    .addClass('CswAction_ContentCell');

                cswPublic.actionDiv = cswPrivate.contentCell.div();

                cswPublic.actionDiv.span()
                        .br({ number: 2 })
                        .div({ suffix: 'content' });

                if (Csw.bool(cswPrivate.hasButtonGroup)) {
                    cswPrivate.btnGroup = cswPublic.table.cell(3, 1).buttonGroup({
                        buttons: {
                            next: { enabled: false, hidden: true },
                            previous: { enabled: false, hidden: true },
                            finish: {
                                text: cswPrivate.FinishText,
                                onclick: function() { Csw.tryExec(cswPrivate.onFinish); }
                            }
                        },
                        cancel: {
                            onclick: function() { Csw.tryExec(cswPrivate.onCancel); }
                        }
                    });
                    cswPublic.finish = cswPrivate.btnGroup.finish;
                    cswPublic.cancel = cswPrivate.btnGroup.cancel;
                }
            } ());
            
            return cswPublic;
        });
} ());

