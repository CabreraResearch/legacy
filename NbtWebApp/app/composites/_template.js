(function () {
    'use strict';

    if (false) { //remove this when you're ready to use the template

        Csw.composites.template = Csw.composites.template ||
            Csw.composites.register('template', function (cswParent, cswPrivate) {

                //#region Variables
                
                var cswPublic = { };

                //#endregion Variables

                //#region Pre-ctor
                (function _pre() {
                    //set default values on cswPrivate if none are supplied
                    cswPrivate.name = cswPrivate.name || 'No name';

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
