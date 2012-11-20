(function () {
    'use strict';

    if (false) { //remove this when you're ready to use the template

        Csw.literals.template = Csw.literals.template ||
            Csw.literals.register('template', function (options) {

                //#region Variables
                var cswPrivate = {
                    name: '',
                    $parent: {}
                };
                var cswPublic = { };

                //#endregion Variables

                //#region Pre-ctor
                (function _pre() {
                    Csw.extend(cswPrivate, options);
                }());
                
                //#endregion Pre-ctor
               

                //#region Define Class Members

                //cswPrivate.method = function() {};

                //cswPublic.method = function() {};
                
                //#endregion Define Class Members
                

                //#region Post-ctor

                (function _post() {
                    
                    var $html = $('<html />');

                    Csw.literals.factory($html, cswPublic);
                    cswPrivate.$parent.append(cswPublic.$);

                }());

                //#endregion Post-ctor

                return cswPublic;
            });

    } //if(false)


} ());
