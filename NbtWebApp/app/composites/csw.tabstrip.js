
(function () {
    'use strict';

    Csw.composites.tabStrip = Csw.composites.tabStrip ||
        Csw.composites.register('tabStrip', function (cswParent, options) {

            //#region Variables
            var cswPrivate = {
                name: ''
            };
            var cswPublic = { };

            //#endregion Variables

            //#region Pre-ctor
            (function _pre() {
                Csw.extend(cswPrivate, options);

                cswParent.empty();
                cswPublic = cswParent.div();

            }());
                
            //#endregion Pre-ctor
               

            //#region Define Class Members

            //cswPrivate.method = function() {};

            //cswPublic.method = function() {};
                
            //#endregion Define Class Members
                

            //#region Post-ctor

            (function _post() {
                

            }());

            //#endregion Post-ctor

            return cswPublic;

        });
} ());
