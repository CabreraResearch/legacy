(function () {
    'use strict';

    Csw.domNode = Csw.domNode ||
            Csw.register('domNode', function (cswPrivate) {

                //#region Variables
                
                var cswPublic = { };

                //#endregion Variables

                //#region Pre-ctor
                (function _pre() {
                    cswPrivate.ID = cswPrivate.ID || 'No ID defined!';
                    cswPrivate.tagName = cswPrivate.tagName || 'DIV';
                    cswPrivate.el = cswPrivate.el; //for Intellisense
                }());
                
                //#endregion Pre-ctor
               

                //#region Define Class Members

                //We shouldn't need to extend this instance significantly as the actual literals classes already define their instances appropriately.
                
                //#endregion Define Class Members
                

                //#region Post-ctor

                (function _post() {
                    
                    switch(cswPrivate.ID) {
                        case 'body':
                            cswPublic.$ = $('body');
                            break;
                        default:
                            cswPublic.$ = $('#' + cswPrivate.ID);
                            break;
                    }
                    
                    cswPublic[0] = cswPrivate.el || cswPublic.$[0];

                    switch(cswPrivate.tagName) {
                        case 'A':
                        case 'APPLET':
                        case 'BR':
                        case 'BUTTON':
                        case 'DIV':
                        case 'FIELDSET':
                        case 'FORM':
                        case 'IMG':
                        case 'INPUT':
                        case 'LABEL':
                        case 'OL':
                        case 'P':
                        case 'SELECT':
                        case 'SPAN':
                        case 'TABLE':
                        case 'TEXTAREA':
                        case 'UL':
                            //For ExtJs interoperability, it will be necessary (eventually) to cast DOM nodes into Csw domNodes.
                            //This is too noisy, but we do want to come back and implement this properly (some day).
                            //Csw.debug.warn('Cannot wrap a DOM ' + cswPrivate.tagName + ' with a matching Csw.literal, using the factory instead.');
                            Csw.literals.factory(null, cswPublic);
                            break;
                        default:
                            Csw.debug.error('Cannot wrap a DOM ' + cswPrivate.tagName + ' with a Csw.domNode');
                            break;
                    }
                }());

                //#endregion Post-ctor

                return cswPublic;
            });

    


} ());
