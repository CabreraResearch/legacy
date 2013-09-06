/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function ($) {
    'use strict';

    Csw.composites.quickTip = Csw.composites.quickTip ||
        Csw.composites.register('quickTip', Csw.method(function (cswParent, options) {
            /// <summary>
            /// Create a Quick Tip adjacent to an element. The defaults work well. Tinker with care. 
            /// </summary>
            /// <returns type=""></returns>
            'use strict';

            var cswPublic = {};
            var cswPrivate = {};

            (function _preCtor() {
                'use strict';
                cswPrivate = {
                    html: '',
                    autoShow: true,
                    focusOnToFront: true,
                    autoHide: true,
                    autoScroll: true,
                    dismissDelay: 10000,
                    closable: true,
                    anchor: 'left',
                    bodyStyle: {
                        background: '#ffff00'
                    },
                    onBeforeClose: null
                };
                Csw.extend(cswPrivate, options);

            }());

            cswPublic.close = function () {
                //closes the quicktip if it was initialized
                //Note - calling the quickTips close() method here does not trigger the 'beforeclose' listener
                if (cswPrivate.tip) {
                    Csw.tryExec(cswPrivate.onBeforeClose);
                    cswPrivate.tip.close();
                    cswPrivate.tip.destroy();
                    cswPrivate.tip = null;
                }
            };

            (function _post() {
                'use strict';

                if (Csw.isElementInDom(cswParent.getId())) {
                    cswPrivate.tip = window.Ext.create('Ext.tip.ToolTip', {
                        //Case 28232: if this is tied to a button (for example), id will not be guaranteed to be unique for every tip.
                        //id: cswPrivate.ID + 'tooltip',
                        target: cswParent.getId(),
                        html: cswPrivate.html,
                        autoShow: cswPrivate.autoShow,
                        autoScroll: cswPrivate.autoScroll,
                        dismissDelay: cswPrivate.dismissDelay,
                        focusOnToFront: cswPrivate.focusOnToFront,
                        autoHide: cswPrivate.autoHide,
                        closable: cswPrivate.closable,
                        anchor: cswPrivate.anchor,
                        bodyStyle: cswPrivate.bodyStyle,
                        listeners: {
                            beforeclose: function () {
                                Csw.tryExec(cswPrivate.onBeforeClose);
                                return true;
                            }
                        }
                    });
                }
            }());

            return cswPublic;
        }));



})(jQuery);
