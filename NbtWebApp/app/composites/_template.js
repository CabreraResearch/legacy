(function () {
    'use strict';

    if (false) { //remove this when you're ready to use the template

        Csw.composites.template = Csw.composites.template ||
            Csw.composites.register('template', function (cswParent, options) {

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

    } //if(false)


} ());
