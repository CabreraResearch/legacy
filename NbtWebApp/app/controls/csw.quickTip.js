/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function ($) {
    'use strict';

    Csw.controls.quickTip = Csw.controls.quickTip ||
        Csw.controls.register('quickTip', Csw.method(function (cswParent, options) {
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
                    autoHide: false,
                    closable: true,
                    anchor: 'left',
                    bodyStyle: {
                        background: '#ffff00'
                    }
                };
                Csw.extend(cswPrivate, options);

            }());

            (function _post() {                                             
                'use strict';
                window.Ext.create('Ext.tip.ToolTip', {
                    target: cswParent.getId(),
                    html: cswPrivate.html,
                    autoShow: cswPrivate.autoShow,
                    focusOnToFront: cswPrivate.focusOnToFront,
                    autoHide: cswPrivate.autoHide,
                    closable: cswPrivate.closable,
                    anchor: cswPrivate.anchor,
                    bodyStyle: cswPrivate.bodyStyle
                });
            }());

            return cswPublic;
        }));



})(jQuery);
